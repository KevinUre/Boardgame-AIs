using System;
using System.Collections.Generic;
using System.Text;

namespace DeadMansDraw.Core
{
    public class Game
    {
        public Deck Deck { get; private set; }
        public DiscardPile DiscardPile { get; private set; }
        public ScoreZone[] ScoreZones { get; private set; }
        public List<Card> Field { get; private set; }
        public Players CurrentPlayersTurn { get; private set; }
        public List<Actions> CurrentAvailableActions { get; private set; }
        public bool IsGameOver { get; private set; }
        private bool KrakenCannonOverride = false;
        private bool DoesCannonNeedTarget = false;
        private List<Card> MapCache = null;

        public Game()
        {
            Initalize();
        }

        private void Initalize()
        {
            Deck = Deck.UnshuffledDeckWithoutTwos();
            Deck.Shuffle();
            DiscardPile = DiscardPile.NewDiscardPile();
            ScoreZones = new ScoreZone[2] { new ScoreZone(), new ScoreZone() };
            Field = new List<Card>();
            CurrentPlayersTurn = Players.PlayerOne;
            CurrentAvailableActions = new List<Actions>() { Actions.Draw };
            IsGameOver = false;
        }

        public void TakeAction(Actions choosenAction, Card targetCard = null)
        {
            if(!CurrentAvailableActions.Contains(choosenAction)) { return; }
            RouteActions(choosenAction, targetCard);
            BustCheck();
            ProcessKraken();
            CheckForEndOfGame();
            DetermineNewActions();
        }

        private void RouteActions(Actions choosenAction, Card targetCard)
        {
            switch(choosenAction)
            {
                case Actions.Draw:
                    PerformDrawAction();
                    break;
                case Actions.TakeAll:
                    PerformTakeAllAction();
                    break;
                case Actions.ChooseSwordTarget:
                    PerformSwordAction(targetCard);
                    break;
                case Actions.ChooseHookTarget:
                    PerformHookAction(targetCard);
                    break;
                case Actions.ChooseCannonTarget:
                    PerformCannonAction(targetCard);
                    break;
                case Actions.ChooseMapTarget:
                    PerformMapAction(targetCard);
                    break;
                case Actions.CannonThenBackToKraken:
                    PerformCannonAction(targetCard);
                    if(!DoesCannonNeedTarget)
                        PerformDrawAction();
                    break;
                default:
                    break;
            }
        }

        private void PerformDrawAction()
        {
            Card drawnCard = Deck.Draw();
            Field.Add(drawnCard);
            if (drawnCard.Suit == Suites.Cannons)
                DoesCannonNeedTarget = true;
        }

        private void PerformTakeAllAction()
        {
            if (ChestAndKeyBothPresentOnField())
            {
                int cardsToTake = Math.Min(Field.Count, DiscardPile.Count);
                ScoreZones[(int)CurrentPlayersTurn].AddMultipleCards(DiscardPile.DrawRandomLump(cardsToTake));
            }
            ScoreZones[(int)CurrentPlayersTurn].AddMultipleCards(Field);
            Field.Clear();
            SwitchPlayers();
        }

        private void PerformSwordAction(Card target)
        {
            int opponentsIndex = GetOpponentPlayerIndex();
            Card cardToSteal = ScoreZones[opponentsIndex].RemoveCard(target);
            if (cardToSteal != null)
            {
                Field.Add(cardToSteal);
                if (cardToSteal.Suit == Suites.Cannons)
                    DoesCannonNeedTarget = true;
            }
        }

        private void PerformHookAction(Card target)
        {
            Card cardToHook = ScoreZones[(int)CurrentPlayersTurn].RemoveCard(target);
            if (cardToHook != null)
            {
                Field.Add(cardToHook);
                if (cardToHook.Suit == Suites.Cannons)
                    DoesCannonNeedTarget = true;
            }
        }

        private void PerformCannonAction(Card target)
        {
            int opponentsIndex = GetOpponentPlayerIndex();
            Card cardToDestroy = ScoreZones[opponentsIndex].RemoveCard(target);
            if (cardToDestroy != null)
            {
                DiscardPile.AddCard(cardToDestroy);
                DoesCannonNeedTarget = false;
            }
        }

        private void PerformMapAction(Card target)
        {
            if(!MapCache.Contains(target)) { return; }
            Card cardTaken = DiscardPile.DrawSelectedCard(target);
            if (cardTaken != null)
            {
                Field.Add(cardTaken);
                MapCache = null;
            }
        }

        private void CheckForEndOfGame()
        {
            if (Deck.RemainingCards == 0 && Field.Count == 0)
                this.IsGameOver = true;

        }

        public bool wouldBust(Card card)
        {
            var matchingSuit = Field.Find(c => c.Suit == card.Suit);
            if (matchingSuit != null)
                return true;
            return false;
        }

        private bool BustCheck()
        {
            if (Field.Count == 0)
                return false;
            Suites lastSuit = Field[Field.Count - 1].Suit;
            List<Card> matchingCards = Field.FindAll(a => a.Suit == lastSuit);
            if (matchingCards.Count > 1)
            {
                processBust();
                return true;
            }
            return false;
        }

        public List<Card> GetMapCards()
        {
            if(Field[Field.Count-1].Suit != Suites.Maps) { return null; }
            if (MapCache == null)
                MapCache = DiscardPile.PeakRandomThree();
            return MapCache;
        }

        private void processBust()
        {
            if(ChestAndKeyBeforeAnchor())
                ScoreZones[(int)CurrentPlayersTurn].AddMultipleCards(
                    DiscardPile.DrawRandomLump(
                        CardsBeforeAnchor()));
            bool anchor = false;
            while(Field.Count > 0)
            {
                Card currentCard = Field[Field.Count - 1];
                if (anchor)
                    ScoreZones[(int)CurrentPlayersTurn].AddCard(currentCard);
                else
                    DiscardPile.AddCard(currentCard);
                Field.Remove(currentCard);
                if (currentCard.Suit == Suites.Anchors)
                    anchor = true;
            }
            SwitchPlayers();
            DoesCannonNeedTarget = false;
            KrakenCannonOverride = false;
        }

        private bool ChestAndKeyBeforeAnchor()
        {
            bool chest = false;
            bool key = false;
            for(int i = 0; i < Field.Count; i++)
            {
                switch (Field[i].Suit)
                {
                    case Suites.Chests:
                        chest = true;
                        break;
                    case Suites.Keys:
                        key = true;
                        break;
                    case Suites.Anchors:
                        return chest && key;
                    default:
                        break;
                }
            }
            return false;
        }

        private int CardsBeforeAnchor()
        {
            for (int i = Field.Count - 1; i >= 0; i--)
                if (Field[i].Suit == Suites.Anchors)
                    return i;
            return 0;
        }

        private void DetermineNewActions()
        {
            CurrentAvailableActions = new List<Actions>();
            if(IsGameOver)
            {
                CurrentAvailableActions.Add(Actions.None);
                return;
            }
            if (KrakenCannonOverride)
            {
                CurrentAvailableActions.Add(Actions.CannonThenBackToKraken);
                return;
            }
            if (Field.Count == 0)
                CurrentAvailableActions.Add(Actions.Draw);
            else
            {
                switch (Field[Field.Count - 1].Suit)
                {
                    case Suites.Cannons:
                        if (ScoreZones[GetOpponentPlayerIndex(CurrentPlayersTurn)].PointsShowing() > 0
                            && DoesCannonNeedTarget)
                            CurrentAvailableActions.Add(Actions.ChooseCannonTarget);
                        else
                        {
                            CurrentAvailableActions.Add(Actions.Draw);
                            CurrentAvailableActions.Add(Actions.TakeAll);
                        }
                        break;
                    case Suites.Swords:
                        if(ScoreZones[GetOpponentPlayerIndex(CurrentPlayersTurn)].PointsShowing() > 0)
                            CurrentAvailableActions.Add(Actions.ChooseSwordTarget);
                        else
                        {
                            CurrentAvailableActions.Add(Actions.Draw);
                            CurrentAvailableActions.Add(Actions.TakeAll);
                        }
                        break;
                    case Suites.Hooks:
                        if (ScoreZones[(int)CurrentPlayersTurn].PointsShowing() > 0)
                            CurrentAvailableActions.Add(Actions.ChooseHookTarget);
                        else
                        {
                            CurrentAvailableActions.Add(Actions.Draw);
                            CurrentAvailableActions.Add(Actions.TakeAll);
                        }
                        break;
                    case Suites.Maps:
                        if(DiscardPile.Count > 0)
                            CurrentAvailableActions.Add(Actions.ChooseMapTarget);
                        else
                        {
                            CurrentAvailableActions.Add(Actions.Draw);
                            CurrentAvailableActions.Add(Actions.TakeAll);
                        }
                        break;
                    default:
                        if(Deck.RemainingCards > 0)
                            CurrentAvailableActions.Add(Actions.Draw);
                        if (Field.Count > 0)
                            CurrentAvailableActions.Add(Actions.TakeAll);
                        break;
                }
            }
        }

        public bool CanPeak()
        {
            if (Field.Count == 0)
                return false;
            if (Field[Field.Count - 1].Suit == Suites.Oracles)
                return true;
            return false;
        }

        public Card PeekTopCardOfDeck()
        {
            if (CanPeak())
                return Deck.PeakTop();
            else
                return null;

        }

        public int GetFieldCountForPlayer(int playerNumber)
        {
            int sumOfPointsOnField = 0;
            foreach(Suites currentSuit in Enum.GetValues(typeof(Suites)))
            {
                Card cardOnField = Field.Find(c => c.Suit == currentSuit);
                if (cardOnField == null)
                    continue;
                Card highestExisting = ScoreZones[playerNumber].HighestOfSuit(currentSuit);
                int existingScoreInSuit = 0;
                if (highestExisting != null)
                    existingScoreInSuit = highestExisting.Value;
                int worthOfCurrentCard = Math.Max(0, cardOnField.Value - existingScoreInSuit);
                sumOfPointsOnField += worthOfCurrentCard;
            }
            return sumOfPointsOnField;
        }

        public int GetFieldCountForPlayer(Players player)
        {
            return GetFieldCountForPlayer((int)player);
        }

        private void SwitchPlayers()
        {
            switch (CurrentPlayersTurn)
            {
                case Players.PlayerOne:
                    CurrentPlayersTurn = Players.PlayerTwo;
                    break;
                case Players.PlayerTwo:
                    CurrentPlayersTurn = Players.PlayerOne;
                    break;
                default:
                    break;
            }
        }

        private int GetOpponentPlayerIndex(Players currentPlayer)
        {
            switch (currentPlayer)
            {
                case Players.PlayerOne:
                    return (int)Players.PlayerTwo;
                case Players.PlayerTwo:
                    return (int)Players.PlayerOne;
                default:
                    throw new Exception("what");
            }
        }

        private int GetOpponentPlayerIndex()
        {
            return GetOpponentPlayerIndex(CurrentPlayersTurn);
        }

        public bool ChestAndKeyBothPresentOnField()
        {
            return ChestOnField() && KeyOnField();
        }

        public bool ChestOnField()
        {
            Card chest = Field.Find(c => c.Suit == Suites.Chests);
            return chest != null;
        }

        public bool KeyOnField()
        {
            Card key = Field.Find(c => c.Suit == Suites.Keys);
            return key != null;
        }

        public void ProcessKraken()
        {
            KrakenCannonOverride = false;
            if (Field.Count == 0)
                return;
            if (Field[Field.Count - 1].Suit != Suites.Krakens)
                return;
            PerformDrawAction();
            bool busted = BustCheck();
            if (busted)
                return;
            if (Field[Field.Count - 1].Suit == Suites.Swords
                && ScoreZones[GetOpponentPlayerIndex()].PointsShowing() > 0)
                return;
            if (Field[Field.Count - 1].Suit == Suites.Hooks
                && ScoreZones[(int)CurrentPlayersTurn].PointsShowing() > 0)
                return;
            if (Field[Field.Count - 1].Suit == Suites.Maps
                && DiscardPile.Count > 0)
                return;
            if(Field[Field.Count - 1].Suit == Suites.Cannons
                && ScoreZones[GetOpponentPlayerIndex()].PointsShowing() > 0)
            {
                KrakenCannonOverride = true;
                DoesCannonNeedTarget = true;
                return;
            }
            PerformDrawAction();
            BustCheck();
        }

    }

    public enum Players
    {
        PlayerOne = 0,
        PlayerTwo = 1
    }

    public enum Actions
    {
        None = 0,
        Draw = 1,
        TakeAll = 2,
        ChooseSwordTarget = 4,
        ChooseHookTarget = 8,
        ChooseCannonTarget = 16,
        ChooseMapTarget = 32,
        CannonThenBackToKraken = 64
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack.Core
{
    public class Game
    {
        private Deck Deck { get; set; }
        private Hand DealersHand { get; set; }
        private Hand PlayersHand { get; set; }
        public GameState State { get; private set; }
        public Game()
        {
            Deck = Deck.UnshuffledDeck();
            Deck.Shuffle();
            DealersHand = new Hand();
            PlayersHand = new Hand();
            DealersHand.AddCard(Deck.Draw());
            DealersHand.AddCard(Deck.Draw());
            PlayersHand.AddCard(Deck.Draw());
            PlayersHand.AddCard(Deck.Draw());
        }

        public void Hit()
        {
            Card topCard = Deck.Draw();
            PlayersHand.AddCard(topCard);
            if (PlayersHand.Points().points > 21)
                State = GameState.Lost;
        }

        public void Stay()
        {
            if (State == GameState.Lost) return;
            while(DealersHand.Points().points < 17)
            {
                Card topCard = Deck.Draw();
                DealersHand.AddCard(topCard);
            }
            if(DealersHand.Points().points > 21)
            {
                State = GameState.Won;
            }
            else if (DealersHand.Points().points < PlayersHand.Points().points)
            {
                State = GameState.Won;
            }
            else
            {
                State = GameState.Lost;
            }
        }

        public Card GetDealerShowingCard()
        {
            return DealersHand.Cards[0];
        }

        public int GetDealerShowingValue()
        {
            switch(DealersHand.Cards[0].Value)
            {
                case Values.Ace:
                    return 11;
                case Values.Jack:
                case Values.Queen:
                case Values.King:
                    return 10;
                default:
                    return (int)DealersHand.Cards[0].Value;
            }
        }

        public List<Card> GetPlayerHandCards()
        {
            return PlayersHand.Cards;
        }

        public (int points, bool soft) GetPlayerHandValue()
        {
            return PlayersHand.Points();
        }
    }

    public enum GameState
    {
        PlayersTurn,
        Won,
        Lost
    }
}

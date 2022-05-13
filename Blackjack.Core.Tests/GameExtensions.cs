using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack.Core.Tests
{
    internal static class GameExtensions
    {
        public static void BringCardToTopOfDeck(this Game game, Card card)
        {
            PropertyInfo deckInfos = typeof(Game).GetProperty("Deck",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Deck deck = deckInfos.GetValue(game) as Deck;
            deck.BringCardToTopDebug(card);
            deckInfos.SetValue(game, deck);
        }

        public static void DealCardToPlayer(this Game game)
        {
            PropertyInfo deckInfos = typeof(Game).GetProperty("Deck",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Deck deck = deckInfos.GetValue(game) as Deck;
            Card newCard = deck.Draw();
            deckInfos.SetValue(game, deck);
            PropertyInfo handInfos = typeof(Game).GetProperty("PlayersHand",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Hand hand = handInfos.GetValue(game) as Hand;
            hand.AddCardDebug(newCard);
            handInfos.SetValue(game, hand);
        }

        public static void DealCardToDeaer(this Game game)
        {
            PropertyInfo deckInfos = typeof(Game).GetProperty("Deck",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Deck deck = deckInfos.GetValue(game) as Deck;
            Card newCard = deck.Draw();
            deckInfos.SetValue(game, deck);
            PropertyInfo handInfos = typeof(Game).GetProperty("DealersHand",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Hand hand = handInfos.GetValue(game) as Hand;
            hand.AddCardDebug(newCard);
            handInfos.SetValue(game, hand);
        }

        public static List<Card> GetDealerCardsDebug(this Game game)
        {
            PropertyInfo handInfos = typeof(Game).GetProperty("DealersHand",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Hand hand = handInfos.GetValue(game) as Hand;
            return hand.Cards;
        }

        public static Hand GetDealersHandDebug(this Game game)
        {
            PropertyInfo handInfos = typeof(Game).GetProperty("DealersHand",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Hand hand = handInfos.GetValue(game) as Hand;
            return hand;
        }

        public static void Uninitialize(this Game game, bool shuffle = false)
        {
            PropertyInfo deckInfos = typeof(Game).GetProperty("Deck",
                BindingFlags.NonPublic | BindingFlags.Instance);
            PropertyInfo playersHandInfos = typeof(Game).GetProperty("PlayersHand",
                BindingFlags.NonPublic | BindingFlags.Instance);
            PropertyInfo dealersHandInfos = typeof(Game).GetProperty("DealersHand",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Deck deck = Deck.UnshuffledDeck();
            if (shuffle)
                deck.Shuffle();
            deckInfos.SetValue(game, deck);
            playersHandInfos.SetValue(game, new Hand());
            dealersHandInfos.SetValue(game, new Hand());
        }

        public static void SetState(this Game game, GameState newState)
        {
            PropertyInfo stateInfos = typeof(Game).GetProperty("State");
            stateInfos.SetValue(game, newState);
        }
    }
}

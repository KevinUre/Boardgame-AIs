using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace DeadMansDraw.Core.Tests
{
    public static class DeckHelpers
    {
        public static void BringCardToPosition(ref Game game, Card card, int position)
        {
            Deck theDeck = game.Deck;
            List<Card> cards = typeof(Deck).GetField("Cards",
                BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(theDeck) as List<Card>;
            int indexOfCard = cards.FindIndex(c => c.Value == card.Value && c.Suit == card.Suit);
            if (indexOfCard == -1)
                throw new Exception("card not in deck");
            cards.RemoveAt(indexOfCard);
            cards.Insert(position, card);

            FieldInfo cardsFieldInfo = typeof(Deck).GetField("Cards",
                BindingFlags.NonPublic | BindingFlags.Instance);
            cardsFieldInfo.SetValue(theDeck, cards);

            PropertyInfo deckPropInfo = typeof(Game).GetProperty("Deck");
            deckPropInfo.SetValue(game, theDeck);
        }
        public static void RemoveCardFromDeck(ref Game game, Card card)
        {
            Deck theDeck = game.Deck;
            List<Card> cards = typeof(Deck).GetField("Cards",
                BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(theDeck) as List<Card>;
            int indexOfCard = cards.FindIndex(c => c.Value == card.Value && c.Suit == card.Suit);
            if (indexOfCard == -1)
                throw new Exception("card not in deck");
            cards.RemoveAt(indexOfCard);

            FieldInfo cardsFieldInfo = typeof(Deck).GetField("Cards",
                BindingFlags.NonPublic | BindingFlags.Instance);
            cardsFieldInfo.SetValue(theDeck, cards);

            PropertyInfo deckPropInfo = typeof(Game).GetProperty("Deck");
            deckPropInfo.SetValue(game, theDeck);
        }
    }
}

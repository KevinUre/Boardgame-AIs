using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack.Core.Tests
{
    internal static class DeckExtensions
    {
        public static List<Card> GetCardsDebug(this Deck deck)
        {
            List<Card> cards = typeof(Deck).GetProperty("Cards",
                BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(deck) as List<Card>;
            return cards;
        }

        public static void SetCardsDebug(this Deck deck, List<Card> cards)
        {
            PropertyInfo cardsFieldInfo = typeof(Deck).GetProperty("Cards",
                BindingFlags.NonPublic | BindingFlags.Instance);
            cardsFieldInfo.SetValue(deck, cards);
        }

        public static void BringCardToTopDebug(this Deck deck, Card card)
        {
            List<Card> cards = deck.GetCardsDebug();
            cards.Remove(card);
            cards = cards.Prepend(card).ToList();
            deck.SetCardsDebug(cards);
        }
    }
}

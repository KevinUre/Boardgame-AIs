using System;
using System.Collections.Generic;
using System.Text;

namespace DeadMansDraw.Core
{
    public class ScoreZone
    {
        List<Card> Cards = new List<Card>();

        public Card HighestOfSuit(Suites suit)
        {
            var allSuitMatches = Cards.FindAll(c => c.Suit == suit);
            if(allSuitMatches.Count == 0) { return null; }
            allSuitMatches.Sort((first, second) => second.Value.CompareTo(first.Value)); //second to first is decending order
            return allSuitMatches[0];
        }

        public int CardsInSuit(Suites suit)
        {
            var allSuitMatches = Cards.FindAll(c => c.Suit == suit);
            return allSuitMatches.Count;
        }

        public int PointsShowing()
        {
            int sum = 0;
            foreach(Suites suit in Enum.GetValues(typeof(Suites)))
            {
                var highestOfSuit = HighestOfSuit(suit);
                if(highestOfSuit != null) { sum += highestOfSuit.Value; }
            }
            return sum;
        }

        internal void AddCard(Card card)
        {
            Cards.Add(card);
        }

        internal Card RemoveCard(Card card)
        {
            Card actual = Cards.Find(a => a == card);
            if (actual != null)
                Cards.Remove(actual);
            return actual;
        }

        internal void AddMultipleCards(List<Card> cards)
        {
            Cards.AddRange(cards);
        }
    }
}

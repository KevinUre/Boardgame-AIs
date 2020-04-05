using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadMansDraw.Core
{
    public class DiscardPile
    {
        private List<Card> Cards;

        public static DiscardPile NewDiscardPile()
        {
            List<Card> newCards = new List<Card>();
            foreach (Suites suit in Enum.GetValues(typeof(Suites)))
            {
                if (suit == Suites.Mermaids)
                    newCards.Add(new Card { Suit = suit, Value = 4 });
                else
                    newCards.Add(new Card { Suit = suit, Value = 2 });
            }
            return new DiscardPile{ Cards = newCards };
        }

        internal List<Card> PeakRandomThree()
        {
            if (Cards.Count == 0) { return new List<Card>(); }
            Cards = Cards.OrderBy(card => StaticHelpers.RandomNumberGenerator.Next()).ToList();
            List<Card> topThree = new List<Card>();
            if (Cards.Count > 0) { topThree.Add(Cards[0]); }
            if (Cards.Count > 1) { topThree.Add(Cards[1]); }
            if (Cards.Count > 2) { topThree.Add(Cards[2]); }
            return topThree;
        }

        internal Card DrawSelectedCard(Card desiredCard)
        {
            if(Cards.Find(c => c == desiredCard) == null) { return null; }
            Cards.Remove(desiredCard);
            return desiredCard;
        }

        internal List<Card> DrawRandomLump(int numberOfCards)
        {
            if (Cards.Count == 0) { return new List<Card>(); }
            Cards = Cards.OrderBy(card => StaticHelpers.RandomNumberGenerator.Next()).ToList();
            List<Card> lump = new List<Card>();
            while( numberOfCards > 0 && Cards.Count > 0)
            {
                lump.Add(DrawSelectedCard(Cards[0]));
                numberOfCards--;
            }
            return lump;
        }

        public int Count { get { return Cards.Count; } }

        internal void AddCard(Card card)
        {
            Cards.Add(card);
        }

    }
}

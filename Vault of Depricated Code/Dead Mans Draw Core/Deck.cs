using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DeadMansDraw.Core
{
    public class Deck
    {
        private List<Card> Cards;

        public static Deck UnshuffledDeckWithoutTwos()
        {
            List<Card> newCards = new List<Card>();
            foreach(Suites suit in Enum.GetValues(typeof(Suites)))
            {
                if(suit == Suites.Mermaids)
                    for (int value = 5; value <= 9; value++)
                        newCards.Add(new Card { Suit = suit, Value = value });
                else
                    for (int value = 3; value <= 7; value++)
                        newCards.Add(new Card { Suit = suit, Value = value });
            }
            return new Deck{ Cards = newCards };
        }

        public Card Draw()
        {
            if(Cards.Count == 0) { return null; }
            Card top = Cards[0];
            Cards.RemoveAt(0);
            return top;
        }

        internal Card PeakTop()
        {
            if (Cards.Count == 0) { return null; }
            return Cards[0];
        }

        public void Shuffle()
        {
            Cards = Cards.OrderBy(card => StaticHelpers.RandomNumberGenerator.Next()).ToList();
        }

        public int RemainingCards { get { return Cards.Count; } }
            
    }
}

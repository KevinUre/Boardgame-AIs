using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack.Core
{
    public class Deck
    {
        private List<Card> Cards { get; set; }

        public static Deck UnshuffledDeck()
        {
            List<Card> newCards = new List<Card>();
            foreach (Suites suit in Enum.GetValues(typeof(Suites)))
            {
                foreach(Values value in Enum.GetValues(typeof(Values)))
                {
                    newCards.Add(new Card { Suit = suit, Value = value });
                }
            }
            return new Deck { Cards = newCards };
        }

        public Card Draw()
        {
            if(Cards.Count == 0) {  return null; }
            Card top = Cards[0];
            Cards.RemoveAt(0);
            return top;
        }

        public void Shuffle()
        {
            Cards = Cards.OrderBy(card => StaticHelpers.RandomNumberGenerator.Next()).ToList();
        }
    }
}

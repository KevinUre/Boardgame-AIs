using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack.Core
{
    public class Hand
    {

        public List<Card> Cards { get; private set; }

        public Hand() 
        {
            Cards = new List<Card>();
        }

        public (int points, bool soft) Points()
        {
            int aces = 0;
            int targetAcesAsOnes = 0;
            foreach (Card card in Cards)
            {
                if (card.Value == Values.Ace)
                    aces++;
            }
            int sum = 0;
            int currentAcesAsOnes = 0;
            while (targetAcesAsOnes <= aces)
            {
                sum = 0;
                currentAcesAsOnes = 0;
                foreach (Card card in Cards)
                {
                    switch(card.Value)
                    {
                        case Values.Ace: 
                            if (currentAcesAsOnes < targetAcesAsOnes)
                            {
                                sum += 1;
                                currentAcesAsOnes++;
                            } else {
                                sum += 11;
                            }
                            break;
                        case Values.Jack:
                        case Values.Queen:
                        case Values.King:
                            sum += 10;
                            break;
                        default:
                            sum += (int)card.Value;
                            break;
                    }
                }
                if (sum <= 21) break;
                targetAcesAsOnes++;
            }
            return (sum, currentAcesAsOnes < aces);
        }

        internal void AddCard(Card card)
        {
            Cards.Add(card);
        }
    }
}

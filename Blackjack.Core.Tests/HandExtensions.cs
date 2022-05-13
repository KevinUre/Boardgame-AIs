using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack.Core.Tests
{
    public static class HandExtensions
    {
        public static void AddCardDebug(this Hand hand, Card card)
        {
            List<Card> cards = new List<Card>(hand.Cards);
            cards.Add(card);
            PropertyInfo cardsPropInfo = typeof(Hand).GetProperty("Cards");
            cardsPropInfo.SetValue(hand, cards);
        }
    }
}

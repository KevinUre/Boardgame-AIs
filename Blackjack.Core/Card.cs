using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack.Core
{
    public class Card
    {
        public Suites Suit { get; set; }

        public Values Value { get; set; }

        public static bool operator ==(Card a, Card b)
        {
            return a?.Suit == b?.Suit && a?.Value == b?.Value;
        }
        public static bool operator !=(Card a, Card b)
        {
            return a?.Suit != b?.Suit || a?.Value != b?.Value;
        }

        public override bool Equals(object other)
        {
            return Suit == ((Card)other)?.Suit && Value == ((Card)other)?.Value;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // Choose large primes to avoid hashing collisions
                const int HashingBase = (int)2166136261;
                const int HashingMultiplier = 16777619;

                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ Suit.GetHashCode();
                hash = (hash * HashingMultiplier) ^ Value.GetHashCode();
                return hash;
            }
        }

    }

    public enum Suites
    {
        Diamonds,
        Clubs,
        Hearts,
        Spades
    }

    public enum Values
    {
        Ace = 1,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King
    }
}

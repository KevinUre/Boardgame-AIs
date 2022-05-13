using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Blackjack.Core;
using FluentAssertions;
using System.Reflection;

namespace Blackjack.Core.Tests
{
    public class DeckTests
    {
        [Fact]
        public void NewDeckHasAllCardsAsExpected()
        {
            var deck = Deck.UnshuffledDeck();
            foreach (FieldInfo cardInfo in typeof(NamedCards).GetFields())
            {
                deck.GetCardsDebug().Should().Contain((Card)cardInfo.GetValue(null));
            }
            deck.GetCardsDebug().Count().Should().Be(52);
        }

        [Fact]
        public void ShuffleWorks()
        {
            Deck deck = Deck.UnshuffledDeck();
            Deck shuffledDeck = Deck.UnshuffledDeck();
            shuffledDeck.Shuffle();
            shuffledDeck.GetCardsDebug().Should()
                .NotBeEquivalentTo(deck.GetCardsDebug(), o => o.WithStrictOrdering());
        }

        [Fact]
        public void DrawRemovesTheTopCard()
        {
            Deck deck = Deck.UnshuffledDeck();
            deck.Shuffle();
            Card topCard = deck.GetCardsDebug()[0];
            Card drawnCard = deck.Draw();
            deck.GetCardsDebug().Count().Should().Be(51);
            deck.GetCardsDebug().Should().NotContain(drawnCard);
            topCard.Should().Be(drawnCard);
        }
    }
}

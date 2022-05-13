using Xunit;
using Blackjack.Core;
using FluentAssertions;
using System.Reflection;
using System.Collections.Generic;

namespace Blackjack.Core.Tests
{
    public class HandTests
    {
        public static TheoryData<bool, int, Card[]> testingData =>
            new TheoryData<bool, int, Card[]>
            {
                { false, 12, new Card[] { NamedCards.FourOfSpades, NamedCards.EightOfSpades } },
                { false, 17, new Card[] { NamedCards.KingOfSpades, NamedCards.SevenOfSpades } },
                { true, 16, new Card[] { NamedCards.AceOfSpades, NamedCards.FiveOfSpades } },
                { false, 15, new Card[] { NamedCards.AceOfSpades, NamedCards.NineOfSpades, NamedCards.FiveOfSpades } },
                { true, 12, new Card[] { NamedCards.AceOfSpades, NamedCards.AceOfHearts } },
                { true, 17, new Card[] { NamedCards.AceOfSpades, NamedCards.AceOfHearts, NamedCards.FiveOfSpades } },
                { true, 20, new Card[] { NamedCards.AceOfSpades, NamedCards.AceOfHearts, NamedCards.AceOfClubs, NamedCards.SevenOfSpades } },
                { false, 22, new Card[] { NamedCards.AceOfSpades, NamedCards.AceOfHearts, NamedCards.KingOfSpades, NamedCards.QueenOfSpades } },
            };

        [Theory]
        [MemberData(nameof(testingData))]
        public void PointsAreCorrect(bool expectedSoftness, int expectedSum, params Card[] cardsInHand)
        {
            Hand hand = new Hand();
            foreach (Card card in cardsInHand)
                hand.AddCardDebug(card);
            hand.Points().points.Should().Be(expectedSum);
            hand.Points().soft.Should().Be(expectedSoftness);
        }

        [Fact]
        public void AddCardWorks()
        {
            Hand hand = new Hand();
            var addCardHandle = typeof(Hand).GetMethod("AddCard", BindingFlags.Instance | BindingFlags.NonPublic);
            addCardHandle.Invoke(hand, new object[] { NamedCards.AceOfSpades });
            hand.Cards.Count.Should().Be(1);
            hand.Cards[0].Should().Be(NamedCards.AceOfSpades);
        }
    }
}
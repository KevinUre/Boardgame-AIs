using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using System.Reflection;

namespace DeadMansDraw.Core.Tests.Card_Tests
{
    public class Oracles
    {

        [Fact]
        public void CanPeekTheDeckIfOracleIsMostRecentCard()
        {
            Game gameFixture = new Game();
            Card FiveOfOracles = new Card() { Suit = Suites.Oracles, Value = 5 };
            Card FiveOfKeys = new Card { Suit = Suites.Keys, Value = 5 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfOracles);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, FiveOfKeys, 0);

            var topCardActual = gameFixture.PeekTopCardOfDeck();

            topCardActual.Should().Be(FiveOfKeys);
        }

        [Fact]
        public void CannotPeekTheDeckIfOracleIsOnFieldButNotLatest()
        {
            Game gameFixture = new Game();
            Card FiveOfOracles = new Card() { Suit = Suites.Oracles, Value = 5 };
            Card FiveOfKeys = new Card { Suit = Suites.Keys, Value = 5 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfOracles);
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfKeys);

            var topCardActual = gameFixture.PeekTopCardOfDeck();

            topCardActual.Should().BeNull();
        }
    }
}

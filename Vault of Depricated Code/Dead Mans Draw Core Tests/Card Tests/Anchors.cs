using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using System.Reflection;

namespace DeadMansDraw.Core.Tests.Card_Tests
{
    public class Anchors
    {
        public Anchors()
        {
            AssertionOptions.AssertEquivalencyUsing(options => options.WithStrictOrdering());
        }

        [Fact]
        public void BustingWithAnAnchorOutProtectsEarlierCards()
        {
            Game gameFixture = new Game();
            Card FiveOfKeys = new Card() { Suit = Suites.Keys, Value = 5 };
            Card FiveOfAnchors = new Card() { Suit = Suites.Anchors, Value = 5 };
            Card SevenOfKeys = new Card() { Suit = Suites.Keys, Value = 7 };
            GameHelpers.ClearDiscardPile(ref gameFixture);
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfKeys);
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfAnchors);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, SevenOfKeys, 0);
            GameHelpers.SetCurrentActions(ref gameFixture,
                new List<Actions> { Actions.Draw });

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.ScoreZones[0].PointsShowing().Should().Be(5);
            gameFixture.ScoreZones[0].HighestOfSuit(Suites.Anchors).Should().BeNull();
            gameFixture.ScoreZones[0].HighestOfSuit(Suites.Keys).Should().NotBeNull();
            gameFixture.ScoreZones[0].HighestOfSuit(Suites.Keys).Value.Should().Be(5);
            gameFixture.DiscardPile.Count.Should().Be(2);
        }

        [Fact]
        public void BustingWithASecondAnchorProtectsEverythingElse()
        {
            Game gameFixture = new Game();
            Card FiveOfKeys = new Card() { Suit = Suites.Keys, Value = 5 };
            Card FiveOfAnchors = new Card() { Suit = Suites.Anchors, Value = 5 };
            Card FiveOfOracles = new Card() { Suit = Suites.Oracles, Value = 5 };
            Card SevenOfAnchors = new Card() { Suit = Suites.Anchors, Value = 7 };
            GameHelpers.ClearDiscardPile(ref gameFixture);
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfKeys);
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfAnchors);
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfOracles);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, SevenOfAnchors, 0);
            GameHelpers.SetCurrentActions(ref gameFixture,
                new List<Actions> { Actions.Draw });

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.ScoreZones[0].PointsShowing().Should().Be(15);
            gameFixture.ScoreZones[0].HighestOfSuit(Suites.Anchors).Value.Should().Be(5);
            gameFixture.ScoreZones[0].HighestOfSuit(Suites.Keys).Value.Should().Be(5);
            gameFixture.ScoreZones[0].HighestOfSuit(Suites.Oracles).Value.Should().Be(5);
            gameFixture.DiscardPile.Count.Should().Be(1);
        }

        [Fact]
        public void BustingWithBothAChestAndAKeyBehindAnAnchorStillGetsABonus()
        {
            Game gameFixture = new Game();
            Card FiveOfKeys = new Card { Suit = Suites.Keys, Value = 5 };
            Card FiveOfChests = new Card { Suit = Suites.Chests, Value = 5 };
            GameHelpers.ClearDiscardPile(ref gameFixture);
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfKeys);
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfChests);
            GameHelpers.BringCardFromDeckToField(ref gameFixture, new Card { Suit = Suites.Anchors, Value = 5 });
            GameHelpers.BringCardFromDeckToField(ref gameFixture, new Card { Suit = Suites.Mermaids, Value = 5 });
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Mermaids, Value = 9 }, 0);
            GameHelpers.BringCardFromDeckToDiscardPile(ref gameFixture, new Card { Suit = Suites.Oracles, Value = 5 });
            GameHelpers.SetCurrentActions(ref gameFixture,
                new List<Actions> { Actions.Draw });

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.ScoreZones[0].PointsShowing().Should().Be(15);
            gameFixture.ScoreZones[0].HighestOfSuit(Suites.Keys).Value.Should().Be(5);
            gameFixture.ScoreZones[0].HighestOfSuit(Suites.Chests).Value.Should().Be(5);
            gameFixture.ScoreZones[0].HighestOfSuit(Suites.Oracles).Value.Should().Be(5);
            gameFixture.DiscardPile.Count.Should().Be(3);
        }

    }
}

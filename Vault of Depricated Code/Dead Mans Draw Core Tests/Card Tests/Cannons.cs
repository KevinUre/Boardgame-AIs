using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using System.Reflection;

namespace DeadMansDraw.Core.Tests.Card_Tests
{
    public class Cannons
    {
        public Cannons()
        {
            AssertionOptions.AssertEquivalencyUsing(options => options.WithStrictOrdering());
        }

        [Fact]
        public void DrawingIntoACannonMakesYouChooseTarget()
        {
            Game gameFixture = new Game();
            Card FiveOfCannons = new Card() { Suit = Suites.Cannons, Value = 5 };
            Card FiveOfKeys = new Card { Suit = Suites.Keys, Value = 5 };
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, FiveOfCannons, 0);
            GameHelpers.BringCardFromDeckToScoreZone(ref gameFixture, FiveOfKeys, 1);

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions>() { Actions.ChooseCannonTarget });
        }

        [Fact]
        public void DrawingIntoACannonButBustingBusts()
        {
            Game gameFixture = new Game();
            Card FiveOfCannons = new Card() { Suit = Suites.Cannons, Value = 5 };
            Card SevenOfCannons = new Card() { Suit = Suites.Cannons, Value = 7 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfCannons);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, SevenOfCannons, 0);

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions>() { Actions.Draw },
                options => options.WithoutStrictOrdering());
            gameFixture.Field.Count.Should().Be(0);
            gameFixture.CurrentPlayersTurn.Should().Be(Players.PlayerTwo);
            gameFixture.ScoreZones[0].PointsShowing().Should().Be(0);
            gameFixture.DiscardPile.Count.Should().Be(12);
        }

        [Fact]
        public void DrawingACannonWhenTheOpponentHasNoCardsDoesNothing()
        {
            Game gameFixture = new Game();
            Card FiveOfCannons = new Card() { Suit = Suites.Cannons, Value = 5 };
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, FiveOfCannons, 0);

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions>() { Actions.Draw, Actions.TakeAll },
                options => options.WithoutStrictOrdering());
        }

        [Fact]
        public void ChoosingATargetPutsThatCardIntoTheDiscard()
        {
            Game gameFixture = new Game();
            Card FiveOfCannons = new Card() { Suit = Suites.Cannons, Value = 5 };
            Card FiveOfKeys = new Card { Suit = Suites.Keys, Value = 5 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfCannons);
            GameHelpers.BringCardFromDeckToScoreZone(ref gameFixture, FiveOfKeys, 1);
            GameHelpers.SetCurrentActions(ref gameFixture,
                new List<Actions> { Actions.ChooseCannonTarget });
            typeof(Game).GetField("DoesCannonNeedTarget",
                BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(gameFixture, true);

            gameFixture.TakeAction(Actions.ChooseCannonTarget, FiveOfKeys);

            gameFixture.DiscardPile.Count.Should().Be(11);
            gameFixture.ScoreZones[1].PointsShowing().Should().Be(0);
            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions>() { Actions.Draw, Actions.TakeAll },
                options => options.WithoutStrictOrdering());
        }

        [Fact]
        public void ChoosingAnInvalidTargetDoesntNothing()
        {
            Game gameFixture = new Game();
            Card FiveOfCannons = new Card() { Suit = Suites.Cannons, Value = 5 };
            Card FiveOfKeys = new Card { Suit = Suites.Keys, Value = 5 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfCannons);
            GameHelpers.BringCardFromDeckToScoreZone(ref gameFixture, FiveOfKeys, 1);
            GameHelpers.SetCurrentActions(ref gameFixture,
                new List<Actions> { Actions.ChooseCannonTarget });
            typeof(Game).GetField("DoesCannonNeedTarget",
                BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(gameFixture, true);

            gameFixture.TakeAction(Actions.ChooseCannonTarget,
                new Card() { Suit = Suites.Chests, Value = 5 });

            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions>() { Actions.ChooseCannonTarget });
            gameFixture.DiscardPile.Count.Should().Be(10);
        }
    }
}

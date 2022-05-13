using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using System.Reflection;

namespace DeadMansDraw.Core.Tests.Card_Tests
{
    public class Hooks
    {
        public Hooks()
        {
            AssertionOptions.AssertEquivalencyUsing(options => options.WithStrictOrdering());
        }

        [Fact]
        public void DrawingIntoAHookMakesYouChooseTarget()
        {
            Game gameFixture = new Game();
            Card FiveOfHooks = new Card() { Suit = Suites.Hooks, Value = 5 };
            Card FiveOfKeys = new Card { Suit = Suites.Keys, Value = 5 };
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, FiveOfHooks, 0);
            GameHelpers.BringCardFromDeckToScoreZone(ref gameFixture, FiveOfKeys, 0);

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions>() { Actions.ChooseHookTarget });
        }

        [Fact]
        public void DrawingIntoAHookButBustingBusts()
        {
            Game gameFixture = new Game();
            Card FiveOfHooks = new Card() { Suit = Suites.Hooks, Value = 5 };
            Card SevenOfHooks = new Card() { Suit = Suites.Hooks, Value = 7 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfHooks);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, SevenOfHooks, 0);

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
        public void DrawingAHookWhenYoutHaveNoCardsDoesNothing()
        {
            Game gameFixture = new Game();
            Card FiveOfHooks = new Card() { Suit = Suites.Hooks, Value = 5 };
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, FiveOfHooks, 0);

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions>() { Actions.Draw, Actions.TakeAll },
                options => options.WithoutStrictOrdering());
        }

        [Fact]
        public void ChoosingATargetBringsThatCardIntoTheField()
        {
            Game gameFixture = new Game();
            Card FiveOfHooks = new Card() { Suit = Suites.Hooks, Value = 5 };
            Card FiveOfKeys = new Card { Suit = Suites.Keys, Value = 5 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfHooks);
            GameHelpers.BringCardFromDeckToScoreZone(ref gameFixture, FiveOfKeys, 0);
            GameHelpers.SetCurrentActions(ref gameFixture,
                new List<Actions> { Actions.ChooseHookTarget });

            gameFixture.TakeAction(Actions.ChooseHookTarget, FiveOfKeys);

            gameFixture.Field.Count.Should().Be(2);
            gameFixture.Field[1].Should().BeEquivalentTo(FiveOfKeys);
            gameFixture.ScoreZones[0].PointsShowing().Should().Be(0);
            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions>() { Actions.Draw, Actions.TakeAll },
                options => options.WithoutStrictOrdering());
        }

        [Fact]
        public void ChoosingAnInvalidTargetDoesntNothing()
        {
            Game gameFixture = new Game();
            Card FiveOfHooks = new Card() { Suit = Suites.Hooks, Value = 5 };
            Card FiveOfKeys = new Card { Suit = Suites.Keys, Value = 5 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfHooks);
            GameHelpers.BringCardFromDeckToScoreZone(ref gameFixture, FiveOfKeys, 0);
            GameHelpers.SetCurrentActions(ref gameFixture,
                new List<Actions> { Actions.ChooseHookTarget });


            gameFixture.TakeAction(Actions.ChooseHookTarget,
                new Card() { Suit = Suites.Chests, Value = 5 });

            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions>() { Actions.ChooseHookTarget });
            gameFixture.Field.Count.Should().Be(1);
        }

        [Fact]
        public void ChoosingATargetWithAnEffectTriggersThatEffect()
        {
            Game gameFixture = new Game();
            Card FiveOfHooks = new Card() { Suit = Suites.Hooks, Value = 5 };
            Card FiveOfCannons = new Card { Suit = Suites.Cannons, Value = 5 };
            Card FiveOfKeys = new Card { Suit = Suites.Keys, Value = 5 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfHooks);
            GameHelpers.BringCardFromDeckToScoreZone(ref gameFixture, FiveOfCannons, 0);
            GameHelpers.BringCardFromDeckToScoreZone(ref gameFixture, FiveOfKeys, 1);
            GameHelpers.SetCurrentActions(ref gameFixture,
                new List<Actions> { Actions.ChooseHookTarget });

            gameFixture.TakeAction(Actions.ChooseHookTarget, FiveOfCannons);

            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions>() { Actions.ChooseCannonTarget });
        }

        [Fact]
        public void ChoosingATargetThatWouldBustBusts()
        {
            Game gameFixture = new Game();
            Card FiveOfHooks = new Card() { Suit = Suites.Hooks, Value = 5 };
            Card SevenOfHooks = new Card() { Suit = Suites.Hooks, Value = 7 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfHooks);
            GameHelpers.BringCardFromDeckToScoreZone(ref gameFixture, SevenOfHooks, 0);
            GameHelpers.SetCurrentActions(ref gameFixture,
                new List<Actions> { Actions.ChooseHookTarget });

            gameFixture.TakeAction(Actions.ChooseHookTarget, SevenOfHooks);

            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions>() { Actions.Draw },
                options => options.WithoutStrictOrdering());
            gameFixture.Field.Count.Should().Be(0);
            gameFixture.CurrentPlayersTurn.Should().Be(Players.PlayerTwo);
            gameFixture.ScoreZones[0].PointsShowing().Should().Be(0);
            gameFixture.ScoreZones[1].PointsShowing().Should().Be(0);
            gameFixture.DiscardPile.Count.Should().Be(12);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using System.Reflection;

namespace DeadMansDraw.Core.Tests.Card_Tests
{
    public class Swords
    {
        public Swords()
        {
            AssertionOptions.AssertEquivalencyUsing(options => options.WithStrictOrdering());
        }

        [Fact]
        public void DrawingIntoASwordMakesYouChooseTarget()
        {
            Game gameFixture = new Game();
            Card FiveOfSwords = new Card() { Suit = Suites.Swords, Value = 5 };
            Card FiveOfKeys = new Card { Suit = Suites.Keys, Value = 5 };
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, FiveOfSwords, 0);
            GameHelpers.BringCardFromDeckToScoreZone(ref gameFixture, FiveOfKeys, 1);

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions>() { Actions.ChooseSwordTarget });
        }

        [Fact]
        public void DrawingIntoASwordButBustingBusts()
        {
            Game gameFixture = new Game();
            Card FiveOfSwords = new Card() { Suit = Suites.Swords, Value = 5 };
            Card SevenOfSwords = new Card() { Suit = Suites.Swords, Value = 7 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfSwords);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, SevenOfSwords, 0);

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
        public void DrawingASwordWhenTheOpponentHasNoCardsDoesNothing()
        {
            Game gameFixture = new Game();
            Card FiveOfSwords = new Card() { Suit = Suites.Swords, Value = 5 };
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, FiveOfSwords, 0);

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions>() { Actions.Draw, Actions.TakeAll },
                options => options.WithoutStrictOrdering());
        }

        [Fact]
        public void ChoosingATargetBringsThatCardIntoTheField()
        {
            Game gameFixture = new Game();
            Card FiveOfSwords = new Card() { Suit = Suites.Swords, Value = 5 };
            Card FiveOfKeys = new Card { Suit = Suites.Keys, Value = 5 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfSwords);
            GameHelpers.BringCardFromDeckToScoreZone(ref gameFixture, FiveOfKeys, 1);
            GameHelpers.SetCurrentActions(ref gameFixture,
                new List<Actions> { Actions.ChooseSwordTarget });

            gameFixture.TakeAction(Actions.ChooseSwordTarget, FiveOfKeys);

            gameFixture.Field.Count.Should().Be(2);
            gameFixture.Field[1].Should().BeEquivalentTo(FiveOfKeys);
            gameFixture.ScoreZones[1].PointsShowing().Should().Be(0);
            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions>() { Actions.Draw, Actions.TakeAll },
                options => options.WithoutStrictOrdering());
        }

        [Fact]
        public void ChoosingAnInvalidTargetDoesntNothing()
        {
            Game gameFixture = new Game();
            Card FiveOfSwords = new Card() { Suit = Suites.Swords, Value = 5 };
            Card FiveOfKeys = new Card { Suit = Suites.Keys, Value = 5 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfSwords);
            GameHelpers.BringCardFromDeckToScoreZone(ref gameFixture, FiveOfKeys, 1);
            GameHelpers.SetCurrentActions(ref gameFixture,
                new List<Actions> { Actions.ChooseSwordTarget });


            gameFixture.TakeAction(Actions.ChooseSwordTarget, 
                new Card() { Suit = Suites.Chests, Value = 5 });

            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions>() { Actions.ChooseSwordTarget });
            gameFixture.Field.Count.Should().Be(1);
        }

        [Fact]
        public void ChoosingATargetWithAnEffectTriggersThatEffect()
        {
            Game gameFixture = new Game();
            Card FiveOfSwords = new Card() { Suit = Suites.Swords, Value = 5 };
            Card FiveOfCannons = new Card { Suit = Suites.Cannons, Value = 5 };
            Card FiveOfKeys = new Card { Suit = Suites.Keys, Value = 5 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfSwords);
            GameHelpers.BringCardFromDeckToScoreZone(ref gameFixture, FiveOfCannons, 1);
            GameHelpers.BringCardFromDeckToScoreZone(ref gameFixture, FiveOfKeys, 1);
            GameHelpers.SetCurrentActions(ref gameFixture,
                new List<Actions> { Actions.ChooseSwordTarget });

            gameFixture.TakeAction(Actions.ChooseSwordTarget, FiveOfCannons);

            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions>() { Actions.ChooseCannonTarget });
        }

        [Fact]
        public void ChoosingATargetThatWouldBustBusts()
        {
            Game gameFixture = new Game();
            Card FiveOfSwords = new Card() { Suit = Suites.Swords, Value = 5 };
            Card SevenOfSwords = new Card() { Suit = Suites.Swords, Value = 7 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfSwords);
            GameHelpers.BringCardFromDeckToScoreZone(ref gameFixture, SevenOfSwords, 1);
            GameHelpers.SetCurrentActions(ref gameFixture,
                new List<Actions> { Actions.ChooseSwordTarget });

            gameFixture.TakeAction(Actions.ChooseSwordTarget, SevenOfSwords);

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

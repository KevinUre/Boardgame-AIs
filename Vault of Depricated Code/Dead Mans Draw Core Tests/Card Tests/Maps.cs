using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using System.Reflection;

namespace DeadMansDraw.Core.Tests.Card_Tests
{
    public class Maps
    {
        public Maps()
        {
            AssertionOptions.AssertEquivalencyUsing(options => options.WithStrictOrdering());
        }

        [Fact]
        public void DrawingIntoAMapMakesYouChooseTarget()
        {
            Game gameFixture = new Game();
            Card FiveOfMaps = new Card() { Suit = Suites.Maps, Value = 5 };
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, FiveOfMaps, 0);

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions>() { Actions.ChooseMapTarget });
        }

        [Fact]
        public void DrawingIntoAMapsButBustingBusts()
        {
            Game gameFixture = new Game();
            Card FiveOfMaps = new Card() { Suit = Suites.Maps, Value = 5 };
            Card SevenOfMaps = new Card() { Suit = Suites.Maps, Value = 7 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfMaps);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, SevenOfMaps, 0);

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
        public void DrawingAMapWhenTheDiscardPileHasNoCardsDoesNothing()
        {
            Game gameFixture = new Game();
            Card FiveOfMaps = new Card { Suit = Suites.Maps, Value = 5 };
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, FiveOfMaps, 0);
            GameHelpers.ClearDiscardPile(ref gameFixture);

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions>() { Actions.Draw, Actions.TakeAll },
                options => options.WithoutStrictOrdering());
        }

        [Fact]
        public void DrawingAMapWhenTheDiscardPileHasOneCardStillWorks()
        {
            Game gameFixture = new Game();
            Card FiveOfMaps = new Card { Suit = Suites.Maps, Value = 5 };
            Card FiveOfKeys = new Card { Suit = Suites.Keys, Value = 5 };
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, FiveOfMaps, 0);
            GameHelpers.ClearDiscardPile(ref gameFixture);
            GameHelpers.BringCardFromDeckToDiscardPile(ref gameFixture, FiveOfKeys);

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions>() { Actions.ChooseMapTarget });
        }

        [Fact]
        public void ChoosingATargetBringsThatCardIntoTheField()
        {
            Game gameFixture = new Game();
            Card FiveOfMaps = new Card { Suit = Suites.Maps, Value = 5 };
            Card FiveOfKeys = new Card { Suit = Suites.Keys, Value = 5 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfMaps);
            GameHelpers.ClearDiscardPile(ref gameFixture);
            GameHelpers.BringCardFromDeckToDiscardPile(ref gameFixture, FiveOfKeys);
            GameHelpers.BringCardFromDeckToDiscardPile(ref gameFixture, new Card() { Suit = Suites.Chests, Value = 5 });
            GameHelpers.BringCardFromDeckToDiscardPile(ref gameFixture, new Card() { Suit = Suites.Cannons, Value = 5 });
            GameHelpers.SetCurrentActions(ref gameFixture,
                new List<Actions> { Actions.ChooseMapTarget });

            gameFixture.GetMapCards();
            gameFixture.TakeAction(Actions.ChooseMapTarget, FiveOfKeys);

            gameFixture.Field.Count.Should().Be(2);
            gameFixture.Field[1].Should().BeEquivalentTo(FiveOfKeys);
            gameFixture.DiscardPile.Count.Should().Be(2);
            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions>() { Actions.Draw, Actions.TakeAll },
                options => options.WithoutStrictOrdering());
        }

        [Fact]
        public void AskingForMapCardsReturnsThree()
        {
            Game gameFixture = new Game();
            Card FiveOfMaps = new Card { Suit = Suites.Maps, Value = 5 };
            Card FiveOfKeys = new Card { Suit = Suites.Keys, Value = 5 };
            Card FiveOfCannons = new Card { Suit = Suites.Cannons, Value = 5 };
            Card FiveOfChests = new Card { Suit = Suites.Chests, Value = 5 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfMaps);
            GameHelpers.ClearDiscardPile(ref gameFixture);
            GameHelpers.BringCardFromDeckToDiscardPile(ref gameFixture, FiveOfKeys);
            GameHelpers.BringCardFromDeckToDiscardPile(ref gameFixture, FiveOfChests);
            GameHelpers.BringCardFromDeckToDiscardPile(ref gameFixture, FiveOfCannons);
            GameHelpers.SetCurrentActions(ref gameFixture,
                new List<Actions> { Actions.ChooseMapTarget });

            var actual = gameFixture.GetMapCards();

            actual.Should().BeEquivalentTo(new List<Card> { FiveOfKeys, FiveOfChests, FiveOfCannons },
                options => options.WithoutStrictOrdering());
        }

        [Fact]
        public void AskingForMapCardsMultipleTimesReturnsTheSameThree()
        {
            Game gameFixture = new Game();
            Card FiveOfMaps = new Card { Suit = Suites.Maps, Value = 5 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfMaps);
            GameHelpers.SetCurrentActions(ref gameFixture,
                new List<Actions> { Actions.ChooseMapTarget });

            var first = gameFixture.GetMapCards();
            var second = gameFixture.GetMapCards();

            first.Should().BeEquivalentTo(second);
        }

        [Fact]
        public void MappingIntoABustBusts()
        {
            Game gameFixture = new Game();
            Card FiveOfMaps = new Card { Suit = Suites.Maps, Value = 5 };
            Card SevenOfMaps = new Card() { Suit = Suites.Maps, Value = 7 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfMaps);
            GameHelpers.ClearDiscardPile(ref gameFixture);
            GameHelpers.BringCardFromDeckToDiscardPile(ref gameFixture, SevenOfMaps);
            GameHelpers.SetCurrentActions(ref gameFixture,
                new List<Actions> { Actions.ChooseMapTarget });

            gameFixture.GetMapCards();
            gameFixture.TakeAction(Actions.ChooseMapTarget, SevenOfMaps);

            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions>() { Actions.Draw },
                options => options.WithoutStrictOrdering());
            gameFixture.Field.Count.Should().Be(0);
            gameFixture.CurrentPlayersTurn.Should().Be(Players.PlayerTwo);
            gameFixture.ScoreZones[0].PointsShowing().Should().Be(0);
            gameFixture.DiscardPile.Count.Should().Be(2);
        }
    }
}

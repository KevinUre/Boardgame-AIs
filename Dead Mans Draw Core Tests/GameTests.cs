using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using System.Reflection;

namespace DeadMansDraw.Core.Tests
{
    public class GameTests
    {

        public GameTests()
        {
            AssertionOptions.AssertEquivalencyUsing(options => options.WithStrictOrdering());
        }

        [Fact]

        public void CardEquivalenceWorksAsExpected()
        {
            Card FiveOfKeys = new Card { Suit = Suites.Keys, Value = 5 };
            Card DifferentFiveOfKeys = new Card { Suit = Suites.Keys, Value = 5 };
            Card SevenOfKeys = new Card { Suit = Suites.Keys, Value = 2 };
            Card FiveOfCannons = new Card { Suit = Suites.Cannons, Value = 5 };

            (FiveOfKeys == DifferentFiveOfKeys).Should().BeTrue();
            (FiveOfKeys == SevenOfKeys).Should().BeFalse();
            (FiveOfKeys == FiveOfCannons).Should().BeFalse();
            (FiveOfKeys == null).Should().BeFalse();
            (FiveOfKeys != DifferentFiveOfKeys).Should().BeFalse();
            (FiveOfKeys != SevenOfKeys).Should().BeTrue();
            (FiveOfKeys != FiveOfCannons).Should().BeTrue();
            (FiveOfKeys != null).Should().BeTrue();
            (FiveOfKeys.Equals(DifferentFiveOfKeys)).Should().BeTrue();
            (DifferentFiveOfKeys.Equals(FiveOfKeys)).Should().BeTrue();
            (FiveOfKeys.Equals(SevenOfKeys)).Should().BeFalse();
            (SevenOfKeys.Equals(FiveOfKeys)).Should().BeFalse();
            (FiveOfKeys.Equals(FiveOfCannons)).Should().BeFalse();
            (FiveOfCannons.Equals(FiveOfKeys)).Should().BeFalse();
            (FiveOfKeys.Equals(null)).Should().BeFalse();
            (FiveOfKeys.GetHashCode() == DifferentFiveOfKeys.GetHashCode()).Should().BeTrue();
            (FiveOfKeys.GetHashCode() == SevenOfKeys.GetHashCode()).Should().BeFalse();
            (FiveOfKeys.GetHashCode() == FiveOfCannons.GetHashCode()).Should().BeFalse();
            (object.ReferenceEquals(FiveOfKeys, DifferentFiveOfKeys)).Should().BeFalse();
        }

        [Fact]
        public void InitalizeBuildsAndShufflesADeck()
        {
            Game gameFixture = new Game();

            MethodInfo initalizeMethod = typeof(Game).GetMethod("Initalize",
                BindingFlags.NonPublic | BindingFlags.Instance);
            initalizeMethod.Invoke(gameFixture, null);

            Deck resultingDeck = gameFixture.Deck;
            Deck unshuffledDeck = Deck.UnshuffledDeckWithoutTwos();
            FieldInfo cardFieldInfo = typeof(Deck).GetField("Cards",
                BindingFlags.NonPublic | BindingFlags.Instance);
            List<Card> resultingCards = cardFieldInfo.GetValue(resultingDeck) as List<Card>;
            List<Card> unshuffledCards = cardFieldInfo.GetValue(unshuffledDeck) as List<Card>;

            resultingDeck.Should().NotBeNull();
            resultingCards.Should().NotBeNull();
            ListsAreDifferentOrder(resultingCards, unshuffledCards).Should().Be(true);
        }

        private bool ListsAreDifferentOrder(List<Card> first, List<Card> second)
        {
            if (first.Count != second.Count)
                return true;
            for(int i = 0; i < first.Count; i++)
                if (first[i] != second[i])
                    return true;
            return false;
        }

        [Fact]
        public void InitalizeSeedsTheDiscardPile()
        {
            Game gameFixture = new Game();

            MethodInfo initalizeMethod = typeof(Game).GetMethod("Initalize",
                BindingFlags.NonPublic | BindingFlags.Instance);
            initalizeMethod.Invoke(gameFixture, null);

            DiscardPile resultingDiscardPile = gameFixture.DiscardPile;
            resultingDiscardPile.Should().NotBeNull();

            FieldInfo cardsFieldInfo = typeof(DiscardPile).GetField("Cards",
                BindingFlags.NonPublic | BindingFlags.Instance);
            List<Card> cardsInDiscard = cardsFieldInfo.GetValue(resultingDiscardPile) as List<Card>;
            cardsInDiscard.Count.Should().Be(10);
        }

        [Fact]
        public void InitalizeMakesTwoScoreZones()
        {
            Game gameFixture = new Game();

            gameFixture.ScoreZones.Length.Should().Be(2);
            gameFixture.ScoreZones[0].Should().NotBeNull();
            gameFixture.ScoreZones[1].Should().NotBeNull();
        }

        [Fact]
        public void InitalizeMakesAField()
        {
            Game gameFixture = new Game();
            gameFixture.Field.Should().NotBeNull();
        }

        [Fact]
        public void InitalizeSetsTheTurnToPlayerOne()
        {
            Game gameFixture = new Game();
            gameFixture.CurrentPlayersTurn.Should().Be(Players.PlayerOne);
        }

        [Fact]
        public void InitalizeSetsTheCurrentAvailableActionsToDraw()
        {
            Game gameFixture = new Game();
            List<Actions> expected = new List<Actions>() { Actions.Draw };
            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(expected,
                options => options.WithoutStrictOrdering());
        }

        [Fact]
        public void InitalizeDoesNotEndTheGame()
        {
            Game gameFixture = new Game();
            gameFixture.IsGameOver.Should().Be(false);
        }

        [Fact]
        public void TakeActionDoesntActIfActionIsInvalid()
        {
            Game gameFixture = new Game();
            Game untouchedGame = new Game();

            gameFixture.TakeAction(Actions.TakeAll);

            gameFixture.Should().BeEquivalentTo(untouchedGame,
                options => options.WithoutStrictOrdering());
        }

        [Fact]
        public void DrawActionDrawsACard()
        {
            Game gameFixture = new Game();
            GameHelpers.SetCurrentActions(ref gameFixture,
                new List<Actions> { Actions.Draw });
            Card FiveOfKeys = new Card { Suit = Suites.Keys, Value = 5 };
            DeckHelpers.BringCardToPosition(ref gameFixture, FiveOfKeys, 0);

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.Deck.RemainingCards.Should().Be(49);
            gameFixture.Field.Count.Should().Be(1);
            gameFixture.Field[0].Should().Be(FiveOfKeys);
            gameFixture.CurrentPlayersTurn.Should().Be(Players.PlayerOne);
            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions> { Actions.Draw, Actions.TakeAll },
                options => options.WithoutStrictOrdering());
        }

        [Fact]
        public void TakeAllActionTakesCardsAndSwitchesTurns()
        {
            Game gameFixture = new Game();
            GameHelpers.SetCurrentActions(ref gameFixture,
                new List<Actions> { Actions.TakeAll, Actions.Draw });
            Card FiveOfKeys = new Card { Suit = Suites.Keys, Value = 5 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfKeys);

            gameFixture.TakeAction(Actions.TakeAll);

            gameFixture.Field.Count.Should().Be(0);
            gameFixture.ScoreZones[0].HighestOfSuit(Suites.Keys).Value.Should().Be(5);
            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions> { Actions.Draw },
                options => options.WithoutStrictOrdering());
            gameFixture.CurrentPlayersTurn.Should().Be(Players.PlayerTwo);
        }

        [Fact]
        public void GameEndsWhenLastCardsAreTaken()
        {
            Game gameFixture = new Game();
            GameHelpers.SetCurrentActions(ref gameFixture,
                new List<Actions> { Actions.TakeAll });
            Card FiveOfKeys = new Card { Suit = Suites.Keys, Value = 5 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfKeys);
            GameHelpers.MoveEntireDeckToDiscardPile(ref gameFixture);

            gameFixture.TakeAction(Actions.TakeAll);

            gameFixture.IsGameOver.Should().Be(true);
        }

        [Fact]
        public void DrawingIntoABustDiscardsTheFieldAndSwitchesTurns()
        {
            Game gameFixture = new Game();
            GameHelpers.SetCurrentActions(ref gameFixture,
                new List<Actions> { Actions.Draw });
            Card FiveOfKeys = new Card { Suit = Suites.Keys, Value = 5 };
            Card SevenOfKeys = new Card { Suit = Suites.Keys, Value = 7 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfKeys);
            DeckHelpers.BringCardToPosition(ref gameFixture, SevenOfKeys, 0);
            GameHelpers.ClearDiscardPile(ref gameFixture);

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.Field.Count.Should().Be(0);
            gameFixture.CurrentPlayersTurn.Should().Be(Players.PlayerTwo);
            gameFixture.ScoreZones[0].PointsShowing().Should().Be(0);
            gameFixture.DiscardPile.Count.Should().Be(2);

            List<Card> discardPileCards = typeof(DiscardPile).GetField("Cards",
                BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(gameFixture.DiscardPile) as List<Card>;
            discardPileCards.Should().BeEquivalentTo(
                new List<Card> { FiveOfKeys, SevenOfKeys },
                options => options.WithoutStrictOrdering());
        }
    }
}

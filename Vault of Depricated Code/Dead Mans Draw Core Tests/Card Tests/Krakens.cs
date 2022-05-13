using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using System.Reflection;

namespace DeadMansDraw.Core.Tests.Card_Tests
{
    public class Krakens
    {
        public Krakens()
        {
            AssertionOptions.AssertEquivalencyUsing(options => options.WithStrictOrdering());
        }

        [Fact]
        public void DrawingAKrakenCausesTwoMoreDraws()
        {
            Game gameFixture = new Game();
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Krakens, Value = 5 }, 0);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Oracles, Value = 5 }, 1);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Keys, Value = 5 }, 2);

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.Field.Count.Should().Be(3);
        }

        [Fact]
        public void SecondDrawWithAnEffectTriggersTheEffect()
        {
            Game gameFixture = new Game();
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Krakens, Value = 5 }, 0);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Oracles, Value = 5 }, 1);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Swords, Value = 5 }, 2);
            GameHelpers.BringCardFromDeckToScoreZone(ref gameFixture, new Card { Suit = Suites.Keys, Value = 5 }, 1);

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(new List<Actions> { Actions.ChooseSwordTarget });
            gameFixture.Field.Count.Should().Be(3);
        }

        [Fact]
        public void DrawingASwordFirstEndsTheKraken()
        {
            Game gameFixture = new Game();
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Krakens, Value = 5 }, 0);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Swords, Value = 5 }, 1);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Oracles, Value = 5 }, 2);
            GameHelpers.BringCardFromDeckToScoreZone(ref gameFixture, new Card { Suit = Suites.Keys, Value = 5 }, 1);

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(new List<Actions> { Actions.ChooseSwordTarget });
            gameFixture.Field.Count.Should().Be(2);
        }

        [Fact]
        public void DrawingASwordFirstButTheOpponentHasNoCardsDoesntEndTheKraken()
        {
            Game gameFixture = new Game();
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Krakens, Value = 5 }, 0);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Swords, Value = 5 }, 1);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Oracles, Value = 5 }, 2);

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions> { Actions.Draw, Actions.TakeAll },
                options => options.WithoutStrictOrdering());
            gameFixture.Field.Count.Should().Be(3);
            gameFixture.Field[2].Should().Match(a => (a as Card).Suit == Suites.Oracles && (a as Card).Value == 5);
        }

        [Fact]
        public void DrawingAHookFirstEndsTheKraken()
        {
            Game gameFixture = new Game();
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Krakens, Value = 5 }, 0);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Hooks, Value = 5 }, 1);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Oracles, Value = 5 }, 2);
            GameHelpers.BringCardFromDeckToScoreZone(ref gameFixture, new Card { Suit = Suites.Keys, Value = 5 }, 0);

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(new List<Actions> { Actions.ChooseHookTarget });
            gameFixture.Field.Count.Should().Be(2);
        }

        [Fact]
        public void DrawingAHookFirstButYouHaveNoCardsDoesntEndTheKraken()
        {
            Game gameFixture = new Game();
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Krakens, Value = 5 }, 0);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Hooks, Value = 5 }, 1);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Oracles, Value = 5 }, 2);

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions> { Actions.Draw, Actions.TakeAll },
                options => options.WithoutStrictOrdering());
            gameFixture.Field.Count.Should().Be(3);
            gameFixture.Field[2].Should().Match(a => (a as Card).Suit == Suites.Oracles && (a as Card).Value == 5);
        }

        [Fact]
        public void DrawingAMapFirstEndsTheKraken()
        {
            Game gameFixture = new Game();
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Krakens, Value = 5 }, 0);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Maps, Value = 5 }, 1);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Oracles, Value = 5 }, 2);

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(new List<Actions> { Actions.ChooseMapTarget });
            gameFixture.Field.Count.Should().Be(2);
        }

        [Fact]
        public void DrawingAMapFirstButTheDiscardIsEmptyDoesntEndTheKraken()
        {
            Game gameFixture = new Game();
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Krakens, Value = 5 }, 0);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Maps, Value = 5 }, 1);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Oracles, Value = 5 }, 2);
            GameHelpers.ClearDiscardPile(ref gameFixture);

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions> { Actions.Draw, Actions.TakeAll },
                options => options.WithoutStrictOrdering());
            gameFixture.Field.Count.Should().Be(3);
            gameFixture.Field[2].Should().Match(a => (a as Card).Suit == Suites.Oracles && (a as Card).Value == 5);
        }

        [Fact]
        public void DrawingACannonFirstPausesTheKrakenButResumesAfterAShotWasTaken()
        {
            Game gameFixture = new Game();
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Krakens, Value = 5 }, 0);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Cannons, Value = 5 }, 1);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Oracles, Value = 5 }, 2);
            GameHelpers.BringCardFromDeckToScoreZone(ref gameFixture, new Card { Suit = Suites.Keys, Value = 5 }, 1);
            GameHelpers.ClearDiscardPile(ref gameFixture);

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(new List<Actions> { Actions.CannonThenBackToKraken });
            gameFixture.Field.Count.Should().Be(2);

            gameFixture.TakeAction(Actions.CannonThenBackToKraken, new Card { Suit = Suites.Keys, Value = 5 });
            gameFixture.DiscardPile.Count.Should().Be(1);
            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions> { Actions.Draw, Actions.TakeAll },
                options => options.WithoutStrictOrdering());
            gameFixture.Field.Count.Should().Be(3);
            gameFixture.Field[2].Should().Match(a => (a as Card).Suit == Suites.Oracles && (a as Card).Value == 5);
        }

        [Fact]
        public void DrawingACannonFirstButTheOppenentHasNothingDoesntPauseTheKraken()
        {
            Game gameFixture = new Game();
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Krakens, Value = 5 }, 0);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Cannons, Value = 5 }, 1);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Oracles, Value = 5 }, 2);

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.CurrentAvailableActions.Should().BeEquivalentTo(
                new List<Actions> { Actions.Draw, Actions.TakeAll },
                options => options.WithoutStrictOrdering());
            gameFixture.Field.Count.Should().Be(3);
            gameFixture.Field[2].Should().Match(a => (a as Card).Suit == Suites.Oracles && (a as Card).Value == 5);
        }

        [Fact]
        public void BustingOnTheFirstDrawEndsTheKraken()
        {
            Game gameFixture = new Game();
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Krakens, Value = 5 }, 0);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Oracles, Value = 7 }, 1);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Keys, Value = 5 }, 2);
            GameHelpers.BringCardFromDeckToField(ref gameFixture, new Card { Suit = Suites.Oracles, Value = 5 });
            GameHelpers.ClearDiscardPile(ref gameFixture);

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.Field.Count.Should().Be(0);
            gameFixture.DiscardPile.Count.Should().Be(3);
            List<Card> deckCards = typeof(Deck).GetField("Cards",
                BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(gameFixture.Deck) as List<Card>;
            deckCards[0].Should().BeEquivalentTo(new Card { Suit = Suites.Keys, Value = 5 });
            gameFixture.CurrentPlayersTurn.Should().Be(Players.PlayerTwo);
        }

        [Fact]
        public void DrawingAKrakenBustsWithoutAdditionalDraws()
        {
            Game gameFixture = new Game();
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Krakens, Value = 7 }, 0);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Keys, Value = 5 }, 1);
            GameHelpers.BringCardFromDeckToField(ref gameFixture, new Card { Suit = Suites.Krakens, Value = 5 });
            GameHelpers.ClearDiscardPile(ref gameFixture);

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.Field.Count.Should().Be(0);
            gameFixture.DiscardPile.Count.Should().Be(2);
            List<Card> deckCards = typeof(Deck).GetField("Cards",
                BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(gameFixture.Deck) as List<Card>;
            deckCards[0].Should().BeEquivalentTo(new Card { Suit = Suites.Keys, Value = 5 });
            gameFixture.CurrentPlayersTurn.Should().Be(Players.PlayerTwo);
        }

        [Fact]
        public void SecondDrawCanTriggerBust()
        {
            Game gameFixture = new Game();
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Krakens, Value = 5 }, 0);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Keys, Value = 5 }, 1);
            GameHelpers.BringCardToPositionInDeck(ref gameFixture, new Card { Suit = Suites.Oracles, Value = 7 }, 2);
            GameHelpers.BringCardFromDeckToField(ref gameFixture, new Card { Suit = Suites.Oracles, Value = 5 });
            GameHelpers.ClearDiscardPile(ref gameFixture);

            gameFixture.TakeAction(Actions.Draw);

            gameFixture.Field.Count.Should().Be(0);
            gameFixture.DiscardPile.Count.Should().Be(4);
            gameFixture.CurrentPlayersTurn.Should().Be(Players.PlayerTwo);
        }
    }
}

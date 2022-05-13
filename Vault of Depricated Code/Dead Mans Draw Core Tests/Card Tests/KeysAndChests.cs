using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using System.Reflection;

namespace DeadMansDraw.Core.Tests.Card_Tests
{
    public class KeysAndChests
    {
        public KeysAndChests()
        {
            AssertionOptions.AssertEquivalencyUsing(options => options.WithStrictOrdering());
        }

        [Fact]
        public void TakingOnlyAKeyDoesNothingSpecial()
        {
            Game gameFixture = new Game();
            Card FiveOfKeys = new Card { Suit = Suites.Keys, Value = 5 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfKeys);
            GameHelpers.SetCurrentActions(ref gameFixture,
                new List<Actions> { Actions.TakeAll });

            gameFixture.TakeAction(Actions.TakeAll);

            gameFixture.ScoreZones[0].PointsShowing().Should().Be(5);
        }

        [Fact]
        public void TakingOnlyAChestDoesNothingSpecial()
        {
            Game gameFixture = new Game();
            Card FiveOfChests = new Card { Suit = Suites.Chests, Value = 5 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfChests);
            GameHelpers.SetCurrentActions(ref gameFixture,
                new List<Actions> { Actions.TakeAll });

            gameFixture.TakeAction(Actions.TakeAll);

            gameFixture.ScoreZones[0].PointsShowing().Should().Be(5);
        }

        [Fact]
        public void TakingBothAChestAndAKeyYeildsBonusCardsFromTheDiscard()
        {
            Game gameFixture = new Game();
            Card FiveOfKeys = new Card { Suit = Suites.Keys, Value = 5 };
            Card FiveOfChests = new Card { Suit = Suites.Chests, Value = 5 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfKeys);
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfChests);
            GameHelpers.ClearDiscardPile(ref gameFixture);
            GameHelpers.BringCardFromDeckToDiscardPile(ref gameFixture, new Card { Suit = Suites.Oracles, Value = 5 });
            GameHelpers.BringCardFromDeckToDiscardPile(ref gameFixture, new Card { Suit = Suites.Krakens, Value = 5 });
            GameHelpers.BringCardFromDeckToDiscardPile(ref gameFixture, new Card { Suit = Suites.Cannons, Value = 5 });
            GameHelpers.SetCurrentActions(ref gameFixture,
                new List<Actions> { Actions.TakeAll });

            gameFixture.TakeAction(Actions.TakeAll);

            gameFixture.ScoreZones[0].PointsShowing().Should().Be(20);
            gameFixture.DiscardPile.Count.Should().Be(1);
        }

        [Fact]
        public void TakingBothAChestAndAKeyWhenTheDiscardIsLowYeildsAsMuchAsPossible()
        {
            Game gameFixture = new Game();
            Card FiveOfKeys = new Card { Suit = Suites.Keys, Value = 5 };
            Card FiveOfChests = new Card { Suit = Suites.Chests, Value = 5 };
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfKeys);
            GameHelpers.BringCardFromDeckToField(ref gameFixture, FiveOfChests);
            GameHelpers.ClearDiscardPile(ref gameFixture);
            GameHelpers.BringCardFromDeckToDiscardPile(ref gameFixture, new Card { Suit = Suites.Oracles, Value = 5 });
            GameHelpers.SetCurrentActions(ref gameFixture,
                new List<Actions> { Actions.TakeAll });

            gameFixture.TakeAction(Actions.TakeAll);

            gameFixture.ScoreZones[0].PointsShowing().Should().Be(15);
            gameFixture.DiscardPile.Count.Should().Be(0);
        }

        
    }
}

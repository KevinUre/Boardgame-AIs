using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Blackjack.Core;
using FluentAssertions;
using System.Reflection;

namespace Blackjack.Core.Tests
{
    public class GameTests
    {
        public static TheoryData<Card, int> dealerShowingData =>
            new TheoryData<Card, int>
            {
                { NamedCards.AceOfSpades, 11 },
                { NamedCards.QueenOfHearts, 10 },
                { NamedCards.SevenOfClubs, 7 },
            };

        [Theory]
        [MemberData(nameof(dealerShowingData))]
        public void GetDealerShowingValueShowsFirstCard(Card firstCard, int expectedValue)
        {
            Game game = new Game();
            game.Uninitialize();
            game.BringCardToTopOfDeck(firstCard);
            game.DealCardToDeaer();
            game.DealCardToDeaer();
            game.GetDealerShowingValue().Should().Be(expectedValue);
        }

        [Theory]
        [MemberData(nameof(dealerShowingData))]
        public void GetDealerShowingCardShowsFirstCard(Card firstCard, int expectedValue)
        {
            Game game = new Game();
            game.Uninitialize();
            game.BringCardToTopOfDeck(firstCard);
            game.DealCardToDeaer();
            game.DealCardToDeaer();
            game.GetDealerShowingCard().Should().Be(firstCard);
        }

        public static TheoryData<(int points, bool soft), Card[]> playerShowingData =>
            new TheoryData<(int points, bool soft), Card[]>
            {
                { (14, true), new Card[]{ NamedCards.AceOfClubs, NamedCards.ThreeOfHearts }  },
                { (14, false), new Card[]{ NamedCards.AceOfClubs, NamedCards.SevenOfHearts, NamedCards.SixOfDiamonds }  },
                { (21, true), new Card[]{ NamedCards.AceOfClubs, NamedCards.KingOfClubs }  },
            };

        [Theory]
        [MemberData(nameof(playerShowingData))]
        public void GetPlayerShowingValueIsCorrect((int points, bool soft) expected, params Card[] cards)
        {
            Game game = new Game();
            game.Uninitialize();
            foreach (Card card in cards)
            {
                game.BringCardToTopOfDeck(card);
                game.DealCardToPlayer();
            }
            game.GetPlayerHandValue().Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(playerShowingData))]
        public void GetPlayerShowingCardsShowsAllCards((int points, bool soft) expected, params Card[] cards)
        {
            Game game = new Game();
            game.Uninitialize();
            foreach (Card card in cards)
            {
                game.BringCardToTopOfDeck(card);
                game.DealCardToPlayer();
            }
            game.GetPlayerHandCards().Should().BeEquivalentTo(cards);
        }

        [Fact]
        public void AFrteshlyInitializedGameDealsTwoCardsToEachPlayer()
        {
            Game game = new Game();
            game.GetPlayerHandCards().Count.Should().Be(2);
            game.GetDealerCardsDebug().Count.Should().Be(2);
            game.State.Should().Be(GameState.PlayersTurn);
        }

        [Fact]
        public void HitDealsACard()
        {
            Game game = new Game();
            game.GetPlayerHandCards().Count.Should().Be(2);
            game.Hit();
            game.GetPlayerHandCards().Count.Should().Be(3);
        }

        [Fact]
        public void HitDealsTheTopCard()
        {
            Game game = new Game();
            game.Uninitialize(true);
            game.BringCardToTopOfDeck(NamedCards.AceOfSpades);
            game.Hit();
            game.GetPlayerHandCards()[0].Should().Be(NamedCards.AceOfSpades);
        }

        [Fact]
        public void ItIsStillThePlayersTurnAfterAHitThatDoesntBust()
        {
            Game game = new Game();
            game.Uninitialize();

            game.BringCardToTopOfDeck(NamedCards.EightOfClubs);
            game.DealCardToPlayer();
            game.BringCardToTopOfDeck(NamedCards.FiveOfSpades);
            game.DealCardToPlayer();

            game.BringCardToTopOfDeck(NamedCards.EightOfHearts);
            game.Hit();

            game.GetPlayerHandValue().points.Should().Be(21);
            game.State.Should().Be(GameState.PlayersTurn);
        }

        [Fact]
        public void TheGameStateSwitchesToLostWhenThePlayerBusts()
        {
            Game game = new Game();
            game.Uninitialize();

            game.BringCardToTopOfDeck(NamedCards.KingOfClubs);
            game.DealCardToPlayer();
            game.BringCardToTopOfDeck(NamedCards.JackOfDiamonds);
            game.DealCardToPlayer();

            game.BringCardToTopOfDeck(NamedCards.TwoOfClubs);
            game.Hit();

            game.GetPlayerHandValue().points.Should().Be(22);
            game.State.Should().Be(GameState.Lost);
        }

        [Fact]
        public void CantStayWhenBusted()
        {
            Game game = new Game();
            game.Uninitialize();
            game.SetState(GameState.Lost);

            game.BringCardToTopOfDeck(NamedCards.KingOfClubs);
            game.DealCardToDeaer();
            game.BringCardToTopOfDeck(NamedCards.SevenOfDiamonds);
            game.DealCardToDeaer();

            game.BringCardToTopOfDeck(NamedCards.JackOfClubs);
            game.DealCardToPlayer();
            game.BringCardToTopOfDeck(NamedCards.AceOfSpades);
            game.DealCardToPlayer();
            // player would win 21 to 17 if this guard clause fails

            game.Stay();
            game.State.Should().Be(GameState.Lost);
        }

        [Fact]
        public void DealerStaysOnSeventeen()
        {
            Game game = new Game();
            game.Uninitialize();
            game.BringCardToTopOfDeck(NamedCards.KingOfClubs);
            game.DealCardToDeaer();
            game.BringCardToTopOfDeck(NamedCards.SevenOfDiamonds);
            game.DealCardToDeaer();

            game.Stay();
            game.GetDealersHandDebug().Points().points.Should().Be(17);
            game.GetDealerCardsDebug().Count.Should().Be(2);
        }

        [Fact]
        public void DealerHitsOnSixteen()
        {
            Game game = new Game();
            game.Uninitialize();
            game.BringCardToTopOfDeck(NamedCards.KingOfClubs);
            game.DealCardToDeaer();
            game.BringCardToTopOfDeck(NamedCards.SixOfHearts);
            game.DealCardToDeaer();
            game.BringCardToTopOfDeck(NamedCards.TwoOfDiamonds);


            game.GetDealersHandDebug().Points().points.Should().Be(16);
            game.Stay();
            game.GetDealersHandDebug().Points().points.Should().Be(18);
            game.GetDealerCardsDebug().Count.Should().Be(3);
        }

        [Fact]
        public void DealerHitsMultipleTimesIfNecessary()
        {
            Game game = new Game();
            game.Uninitialize();
            game.BringCardToTopOfDeck(NamedCards.ThreeOfDiamonds);
            game.DealCardToDeaer();
            game.BringCardToTopOfDeck(NamedCards.SevenOfDiamonds);
            game.DealCardToDeaer();
            game.BringCardToTopOfDeck(NamedCards.ThreeOfHearts); // 3rd from top
            game.BringCardToTopOfDeck(NamedCards.FourOfDiamonds); // 2nd from top
            game.BringCardToTopOfDeck(NamedCards.TwoOfDiamonds);


            game.GetDealersHandDebug().Points().points.Should().Be(10);
            game.Stay();
            game.GetDealersHandDebug().Points().points.Should().Be(19);
            game.GetDealerCardsDebug().Count.Should().Be(5);
        }

        [Fact]
        public void PlayerWinsIfDealerBusts()
        {
            Game game = new Game();
            game.Uninitialize();
            game.BringCardToTopOfDeck(NamedCards.KingOfClubs);
            game.DealCardToDeaer();
            game.BringCardToTopOfDeck(NamedCards.SixOfHearts);
            game.DealCardToDeaer();

            game.BringCardToTopOfDeck(NamedCards.TwoOfDiamonds);
            game.DealCardToPlayer();
            game.BringCardToTopOfDeck(NamedCards.FiveOfSpades);
            game.DealCardToPlayer();

            game.BringCardToTopOfDeck(NamedCards.KingOfHearts); //dealer will draw this

            game.Stay();
            game.GetDealersHandDebug().Points().points.Should().Be(26);
            game.GetPlayerHandValue().points.Should().Be(7);
            game.State.Should().Be(GameState.Won);
        }

        [Fact]
        public void PlayerWinsIfHigherThanDealer()
        {
            Game game = new Game();
            game.Uninitialize();
            game.BringCardToTopOfDeck(NamedCards.KingOfClubs);
            game.DealCardToDeaer();
            game.BringCardToTopOfDeck(NamedCards.EightOfDiamonds);
            game.DealCardToDeaer();

            game.BringCardToTopOfDeck(NamedCards.JackOfClubs);
            game.DealCardToPlayer();
            game.BringCardToTopOfDeck(NamedCards.AceOfSpades);
            game.DealCardToPlayer();

            game.Stay();
            game.GetDealersHandDebug().Points().points.Should().Be(18);
            game.GetPlayerHandValue().points.Should().Be(21);
            game.State.Should().Be(GameState.Won);
        }

        [Fact]
        public void DealerWinsTies()
        {
            Game game = new Game();
            game.Uninitialize();
            game.BringCardToTopOfDeck(NamedCards.KingOfClubs);
            game.DealCardToDeaer();
            game.BringCardToTopOfDeck(NamedCards.EightOfDiamonds);
            game.DealCardToDeaer();

            game.BringCardToTopOfDeck(NamedCards.NineOfClubs);
            game.DealCardToPlayer();
            game.BringCardToTopOfDeck(NamedCards.NineOfHearts);
            game.DealCardToPlayer();

            game.Stay();
            game.GetDealersHandDebug().Points().points.Should().Be(18);
            game.GetPlayerHandValue().points.Should().Be(18);
            game.State.Should().Be(GameState.Lost);
        }

        [Fact]
        public void DealerWinsIfHigher()
        {
            Game game = new Game();
            game.Uninitialize();
            game.BringCardToTopOfDeck(NamedCards.KingOfClubs);
            game.DealCardToDeaer();
            game.BringCardToTopOfDeck(NamedCards.EightOfDiamonds);
            game.DealCardToDeaer();

            game.BringCardToTopOfDeck(NamedCards.NineOfClubs);
            game.DealCardToPlayer();
            game.BringCardToTopOfDeck(NamedCards.SixOfSpades);
            game.DealCardToPlayer();

            game.Stay();
            game.GetDealersHandDebug().Points().points.Should().Be(18);
            game.GetPlayerHandValue().points.Should().Be(15);
            game.State.Should().Be(GameState.Lost);
        }

    }
}

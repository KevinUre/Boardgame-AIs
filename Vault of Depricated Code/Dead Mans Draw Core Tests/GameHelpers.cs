using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace DeadMansDraw.Core.Tests
{
    public static class GameHelpers
    {

        public static void BringCardToPositionInDeck(ref Game game, Card card, int position)
        {
            DeckHelpers.BringCardToPosition(ref game, card, position);
        }

        public static void BringCardFromDeckToField(ref Game game, Card card)
        {
            DeckHelpers.RemoveCardFromDeck(ref game, card);
            List<Card> theField = game.Field;
            theField.Add(card);
            typeof(Game).GetProperty("Field").SetValue(game, theField);
        }

        public static void BringCardFromDeckToScoreZone(ref Game game, Card card, int scoreZoneIndex)
        {
            DeckHelpers.RemoveCardFromDeck(ref game, card);
            var shit = typeof(ScoreZone).GetMethod("AddCard",
                BindingFlags.NonPublic | BindingFlags.Instance);
            ScoreZone[] zonearray = game.ScoreZones;
            ScoreZone zoneToModify = game.ScoreZones[scoreZoneIndex];
            shit.Invoke(zoneToModify, new object[] { card });
            zonearray[scoreZoneIndex] = zoneToModify;
            typeof(Game).GetProperty("ScoreZones").SetValue(game, zonearray);
        }
        public static void BringCardFromDeckToDiscardPile(ref Game game, Card card)
        {
            DeckHelpers.RemoveCardFromDeck(ref game, card);
            DiscardPile discardPile = game.DiscardPile;
            FieldInfo cardsinDiscardFieldInfo = typeof(DiscardPile).GetField("Cards",
                BindingFlags.NonPublic | BindingFlags.Instance);
            List<Card> cardsInDiscard = cardsinDiscardFieldInfo.GetValue(discardPile) as List<Card>;
            cardsInDiscard.Add(card);
            cardsinDiscardFieldInfo.SetValue(discardPile, cardsInDiscard);
            typeof(Game).GetProperty("DiscardPile").SetValue(game, discardPile);
        }

        public static void MoveEntireDeckToDiscardPile(ref Game game)
        {
            Deck theDeck = game.Deck;
            List<Card> cardsInDeck = typeof(Deck).GetField("Cards",
                BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(theDeck) as List<Card>;

            DiscardPile discardPile = game.DiscardPile;
            FieldInfo cardsinDiscardFieldInfo = typeof(DiscardPile).GetField("Cards",
                BindingFlags.NonPublic | BindingFlags.Instance);
            List<Card> cardsInDiscard = cardsinDiscardFieldInfo.GetValue(discardPile) as List<Card>;
            cardsInDiscard.AddRange(cardsInDeck);
            cardsinDiscardFieldInfo.SetValue(discardPile, cardsInDiscard);
            PropertyInfo discardInfo = typeof(Game).GetProperty("DiscardPile");
            discardInfo.SetValue(game, discardPile);

            FieldInfo cardsinDeckFieldInfo = typeof(Deck).GetField("Cards",
                BindingFlags.NonPublic | BindingFlags.Instance);
            cardsinDeckFieldInfo.SetValue(theDeck, new List<Card>());
            PropertyInfo deckPropInfo = typeof(Game).GetProperty("Deck");
            deckPropInfo.SetValue(game, theDeck);
        }

        public static void ClearDiscardPile(ref Game game)
        {
            DiscardPile discardPile = game.DiscardPile;
            FieldInfo cardsinDiscardFieldInfo = typeof(DiscardPile).GetField("Cards",
                BindingFlags.NonPublic | BindingFlags.Instance);
            cardsinDiscardFieldInfo.SetValue(discardPile, new List<Card>());
            PropertyInfo discardInfo = typeof(Game).GetProperty("DiscardPile");
            discardInfo.SetValue(game, discardPile);
        }

        public static void SetCurrentActions(ref Game game, List<Actions> desiredActions)
        {
            typeof(Game).GetProperty("CurrentAvailableActions").SetValue(game, desiredActions);
        }
    }
}

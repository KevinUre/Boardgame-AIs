# Blackjack Core

This repo is a state machine that plays a simplified version of blackjack. No double downs or splits.

## Purpose

The state machine is meant to be implemented by the machine learning library to form a fitness function and/or by a UI library to allow a human to interact with the machine.

## Implementing the Library

Make a new instance of `Blackjack.Core.Game`. You can ask what state the machine is in with `game.State`. you can see the dealer's face up card with `game.GetDealerShowingCard()` or the value of that card with `game.GetDealerShowingValue()`. You can see the player's cards with `game.GetPlayerHandCards()` or the value and softness of the player's hand with `GetPlayerHandValue()`. To work the machine you can use `game.Hit()` and `game.Stay()`. After each action refer to the machine's state to see if you busted, lost, or won.

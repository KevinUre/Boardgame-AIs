# Boardgame-AIs

This is my attempt at an AI that generates boardgame designs, wild i know...

## The Premise

I want an AI that i can play a boardgame against. Lets just pick a game, any game. a neural net is probably best for the complex decision space of gaming, but i don't have any training data. I can use a genetic algorithm-like methodology to evolve a better neural net instead of training. I've played several hundred _hobbiest_ boardgames and (semantically) there are actually very few truly unique game mechanics among them; they almost all draw from a common pool of actors, objects and mechanisms. If i can turn that understanding into a DSL i should be able to make an AI that can learn _any_ game. At that point I can make another genetic algorithm to spit out game DSL and make AI players against the design. It can judge the game _meta_ that the AI players adopt to determine if the game is good or not.

### Phase 1 - Blackjack
Blackjack is a great place to start because it's simple and more importantly it's a solved game. I make my AI learn blackjack and if it comes up with the mathematically correct methods of play then i know my training-less neural net works.

### Phase 2 - DSL
I create a gaming DSL and write the rules for blackjack in the DSL. i make a factory that makes game state machines based on the DSL and re-tool the AI to interact only with the DSL.

### Phase 3 - Dead Mans Draw
Write Dead Man's Draw, another simple card game, in the DSL. See if the system properly adapts.

...
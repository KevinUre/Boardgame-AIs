using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeadMansDraw.Ecosystem
{
    public class DMDEcosystem
    {
        public static int SPECIMEN_PER_GENERATION = 500;
        public static int ROUNDS_OF_FITNESS_TESTING = 10;
        public List<Player> currentGeneration = new List<Player>();

        public List<Player> CreateRandomGeneration()
        {
            List<Player> newGeneration = new List<Player>();
            for(int i = 0; i < SPECIMEN_PER_GENERATION; i++)
                newGeneration.Add(new Player { ID = i });
            return newGeneration;
        }

        public List<Player> TestPopulationFitness(List<Player> incomingPlayers)
        {
            for (int round = 0; round < ROUNDS_OF_FITNESS_TESTING; round++)
            {
                List<Player> playersThatNeedToPlay = new List<Player>(incomingPlayers);
                var gameRunners = new List<Task<(Player, Player)>>();
                while(playersThatNeedToPlay.Count > 0)
                {
                    (Player playerOne, Player playerTwo) combatants = SelectPlayers(playersThatNeedToPlay);
                    gameRunners.Add(BeginMatch(combatants.playerOne,combatants.playerTwo));
                }
                Task.WaitAll(gameRunners.ToArray());
                List<Player> PostGamePlayers = GetResultsFromGames(gameRunners);
                incomingPlayers = new List<Player>(PostGamePlayers);
            }
            var testedPlayers = CalculatePopulationFitness(incomingPlayers);
            return testedPlayers;
        }

        public List<Player> CalculatePopulationFitness(List<Player> population)
        {
            population.ForEach((player) => CalculatePlayerFitnessFromScores(ref player));
            return population;
        }

        public void CalculatePlayerFitnessFromScores(ref Player player)
        {
            int sum = 0;
            player.Scores.ForEach(score => sum += score);
            player.Fitness = (double)sum / (double)ROUNDS_OF_FITNESS_TESTING;
        }

        public Task<(Player,Player)> BeginMatch(Player playerOne, Player playerTwo)
        {
            return Task.Run(() =>
            {
                (Player one, Player two) postGamePlayers = PlayDeadMansDraw(playerOne, playerTwo);
                return postGamePlayers;
            });
        }

        public (Player, Player) SelectPlayers(List<Player> playerPool)
        {
            int firstPlayerIndex = StaticHelpers.RandomNumberGenerator.Next(0, playerPool.Count);
            Player firstPlayer = playerPool[firstPlayerIndex];
            playerPool.RemoveAt(firstPlayerIndex);
            int secondPlayerIndex = StaticHelpers.RandomNumberGenerator.Next(0, playerPool.Count);
            Player secondPlayer = playerPool[secondPlayerIndex];
            playerPool.RemoveAt(secondPlayerIndex);
            return (firstPlayer, secondPlayer);
        }

        public List<Player> GetResultsFromGames(List<Task<(Player, Player)>> gameTasks)
        {
            List<Player> players = new List<Player>();
            gameTasks.ForEach((task) =>
            {
                var result = task.GetAwaiter().GetResult();
                players.Add(result.Item1);
                players.Add(result.Item2);
            });
            return players;
        }

        public (Player,Player) PlayDeadMansDraw(Player playerOne, Player playerTwo)
        {
            playerOne.Scores.Add(StaticHelpers.RandomNumberGenerator.Next(0, 71));
            playerTwo.Scores.Add(StaticHelpers.RandomNumberGenerator.Next(0, 71));
            return (playerOne, playerTwo);
        }
    }
}

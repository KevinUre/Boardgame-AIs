using System;

namespace DeadMansDraw.Ecosystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            DMDEcosystem eco = new DMDEcosystem();
            eco.currentGeneration = eco.CreateRandomGeneration();
            eco.currentGeneration = eco.TestPopulationFitness(eco.currentGeneration);
            Console.ReadLine();
        }
    }
}

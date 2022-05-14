using System;

namespace Blackjack.AI // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            NeuralNet net = new NeuralNet(10,10);
            net.latestInput.DealerValueShowing.Value = 6.0;
            net.latestInput.PlayerPointsShowing.Value = 14.0;
            net.latestInput.Softness.Value = -1.0;

            net.Calculate();
            Console.WriteLine(net.latestOutput.ToString());
        }
    }
}
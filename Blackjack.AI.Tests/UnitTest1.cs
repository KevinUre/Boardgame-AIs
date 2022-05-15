using Blackjack.AI;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Reflection;

namespace Blackjack.AI.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            NeuralNet net = new NeuralNet(10, 10);
            net.latestInput.DealerValueShowing.Value = 6.0;
            net.latestInput.PlayerPointsShowing.Value = 14.0;
            net.latestInput.Softness.Value = -1.0;
            net.Calculate();

            net.latestOutput.Hit.Value.Should().NotBe(0);
        }
    }
}
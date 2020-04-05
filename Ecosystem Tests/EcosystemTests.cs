using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using DeadMansDraw.Ecosystem;
using FluentAssertions;

namespace DeadMansDraw.Ecosystem.Tests
{
    public class EcosystemTests
    {

        public EcosystemTests()
        {

        }

        [Fact]
        public void CreateRandomGeneration_Creates_A_Generation()
        {
            DMDEcosystem ecosystem = new DMDEcosystem();
            var actual = ecosystem.CreateRandomGeneration();
            actual.Count.Should().Be(DMDEcosystem.SPECIMEN_PER_GENERATION);
        }

        [Fact]
        public void TestPopulationFitness_Assigns_All_Specimen_A_Fitness()
        {
            DMDEcosystem ecosystem = new DMDEcosystem();
            var boys = ecosystem.CreateRandomGeneration();
            var men = ecosystem.TestPopulationFitness(boys);
            men.ForEach(player => player.Fitness.Should().NotBe(0));
        }
    }
}

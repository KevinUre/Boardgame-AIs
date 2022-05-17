using Blackjack.AI;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Reflection;
using System.Runtime.CompilerServices;
using Moq;

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

        [Fact]
        public void TestName()
        {
            NeuralNet net = RuntimeHelpers.GetUninitializedObject(typeof(NeuralNet)) as NeuralNet;
            net.latestOutput.Should().BeNull();
            typeof(NeuralNet).GetField("TotalLayers", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(net).Should().Be(0);
        
            // When
        
            // Then
        }

        [Fact]
        public void InitializeDoesItsChecklistOfTasks()
        {
            // Given
            var subject = new Mock<NeuralNet>(10,10);
            subject.CallBase = true;
            subject.Setup(net => net.CreateNeurons(It.IsAny<int>(),It.IsAny<ActivationFunction>()));
            subject.Setup(net => net.WireUpInputNeuronValuePointersToInputBuffer());
            subject.Setup(net => net.WireUpNeuronInputsToPreviousLayerOutputPointers());
            subject.Setup(net => net.WireUpOutputNeuronOutputPointersToOutputBuffer());
            subject.Setup(net => net.RandomizeWeights());
            // When
            subject.Object.Initialize(10,10);
            // Then
            subject.Verify(subject => subject.CreateNeurons(It.IsAny<int>(),It.IsAny<ActivationFunction>()), Times.Once());
            subject.Verify(subject => subject.WireUpInputNeuronValuePointersToInputBuffer(), Times.Once());
            subject.Verify(subject => subject.WireUpNeuronInputsToPreviousLayerOutputPointers(), Times.Once());
            subject.Verify(subject => subject.WireUpOutputNeuronOutputPointersToOutputBuffer(), Times.Once());
            subject.Verify(subject => subject.RandomizeWeights(), Times.Once());
        }
    }
}
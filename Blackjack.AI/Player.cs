using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack.AI
{
    public class Player
    {
        public Guid ID;
    }

    public class NeuralNet
    {
        List<Neuron> Neurons {  get; set; }
        int TotalLayers { get; set; }
        public OutputBuffer latestOutput = new OutputBuffer();
        public InputBuffer latestInput = new InputBuffer();

        public NeuralNet(int deepLayers, int networkWidth)
        {
            TotalLayers = deepLayers + 2;
            Neurons = new List<Neuron>();
            ActivationFunction activationFunction = new ActivationFunction();
            CreateNeurons(networkWidth, activationFunction);
            WireUpInputNeuronValuePointersToInputBuffer();
            WireUpNeuronInputsToPreviousLayerOutputPointers();
            WireUpOutputNeuronOutputPointersToOutputBuffer();
            RandomizeWeights();
        }

        private void CreateNeurons(int networkWidth, ActivationFunction activationFunction)
        {
            for (int layer = 0; layer < TotalLayers - 1; layer++)
            {
                for (int itr = 0; itr < networkWidth; itr++)
                {
                    Neuron newNeuron = new Neuron(Guid.NewGuid(), layer, activationFunction, networkWidth);
                    Neurons.Add(newNeuron);
                }
            }
            for (int itr = 0; itr < typeof(OutputBuffer).GetFields().Count(); itr++)
            {
                Neuron newNeuron = new Neuron(Guid.NewGuid(), TotalLayers - 1, activationFunction, networkWidth);
                Neurons.Add(newNeuron);
            }
        }

        private void WireUpInputNeuronValuePointersToInputBuffer()
        {
            Neurons.FindAll(n => n.NetworkLevel == 0).ForEach(n =>
            {
                n.InputValues.Add(nameof(InputBuffer.DealerValueShowing), latestInput.DealerValueShowing);
                n.InputValues.Add(nameof(InputBuffer.PlayerPointsShowing), latestInput.PlayerPointsShowing);
                n.InputValues.Add(nameof(InputBuffer.Softness), latestInput.Softness);
            });
        }

        private void WireUpNeuronInputsToPreviousLayerOutputPointers()
        {
            for (int layer = 1; layer < TotalLayers; layer++)
            {
                List<Neuron> previousLayer = Neurons.FindAll(n => n.NetworkLevel == layer - 1);
                List<Neuron> currentLayer = Neurons.FindAll(n => n.NetworkLevel == layer);
                currentLayer.ForEach(n =>
                {
                    previousLayer.ForEach(p =>
                    {
                        n.InputValues.Add(p.ID.ToString(), p.OutputPointer);
                    });
                });
            }
        }

        public void WireUpOutputNeuronOutputPointersToOutputBuffer()
        {
            var fields = typeof(OutputBuffer).GetFields().ToList();
            List<Neuron> outputNeurons = Neurons.FindAll(n => n.NetworkLevel == TotalLayers - 1);
            for(int i = 0; i < fields.Count; i++)
            {
                fields[i].SetValue(latestOutput, outputNeurons[i].OutputPointer);
            }
        }

        private void RandomizeWeights()
        {
            Random random = new Random();
            Neurons.ForEach(n =>
            {
                foreach ((string guidString, DoublePointerAnalog valuePointer) in n.InputValues)
                {
                    n.InputWeights.Add(guidString, random.NextDouble() * 2.0 - 1.0);
                }
            });
        }

        public void Calculate()
        {
            for(int layer = 0; layer < TotalLayers; layer++)
            {
                Neurons.FindAll(n => n.NetworkLevel == layer).ForEach(n => n.Calculate());
            }
        }

    }

    public class InputBuffer
    {
        public DoublePointerAnalog DealerValueShowing;
        public DoublePointerAnalog PlayerPointsShowing;
        public DoublePointerAnalog Softness;

        public InputBuffer()
        {
            DealerValueShowing = new DoublePointerAnalog(0);
            PlayerPointsShowing = new DoublePointerAnalog(0);
            Softness = new DoublePointerAnalog(0);
        }
    }

    public class OutputBuffer
    {
        public DoublePointerAnalog Hit;
        public DoublePointerAnalog Stay;

        public OutputBuffer()
        {
            Hit = new DoublePointerAnalog(0);
            Stay = new DoublePointerAnalog(0);
        }
    }

    public class Neuron
    {
        public Guid ID;
        public Dictionary<string, double> InputWeights;
        public Dictionary<string, DoublePointerAnalog> InputValues;
        public int NetworkLevel;
        public DoublePointerAnalog OutputPointer;

        [NonSerialized]
        ActivationFunction ActivationFunction;

        public Neuron(Guid id, int networkLevel, ActivationFunction af, int networkWidth)
        {
            ID = id;
            ActivationFunction = af;
            InputWeights = new Dictionary<string, double>(networkWidth);
            InputValues = new Dictionary<string, DoublePointerAnalog>(networkWidth);
            NetworkLevel = networkLevel;
            OutputPointer = new DoublePointerAnalog(0);
        }

        public void Calculate()
        {
            OutputPointer.Value = ActivationFunction.Calculate(InputWeights, InputValues);
        }
    }

    public class ActivationFunction
    {
        public double Calculate(Dictionary<string, double> inputWeights, Dictionary<string, DoublePointerAnalog> inputValues)
        {
            double sum = 0;
            foreach(string key in inputWeights.Keys)
            {
                sum += inputValues[key].Value * inputWeights[key];
            }
            double average = sum / inputWeights.Keys.Count;
            return average > 0.0 ? 1.0 : -1.0;
        }
    }

    public class DoublePointerAnalog
    {
        public double Value { get; set; }
        public DoublePointerAnalog(double value)
        {
            Value = value;
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum NeuronType
{
    Input,
    Bias,
    Hidden,
    Output,
}

public enum ActivationFunction
{
    Sigmoid,
    TanH,
}

public class Neuron
{
    public NeuronType Type;

    public ActivationFunction Activation;
    public double InputValue;
    public double OutputValue;

    public List<Link> Links;

    public Neuron(NeuronType pType, ActivationFunction pActivation)
    {
        Type = pType;
        Activation = pActivation;
        InputValue = 0;
        OutputValue = 0;
        Links = new List<Link>();
    }

    public double ComputeActivationFunction()
    {
        if (Type == NeuronType.Input || Type == NeuronType.Bias)
            return 0;

        double result = 0;

        switch (Activation)
        {
            case ActivationFunction.Sigmoid:
                result = NeuralNetworkHelper.Sigmoid(InputValue);
                break;

            case ActivationFunction.TanH:
                result = Math.Tanh(InputValue);
                break;
        }

        InputValue = 0;
        OutputValue = result;
        return result;
    }
}

public class Link
{
    public Neuron Output;
    public double Weight;

    public Link(Neuron pOutput, double pW)
    {
        Output = pOutput;
        Weight = pW;
    }
}

public class NeuralNetwork
{
    public uint NumInput;
    uint NumOutput;

    int Bias;

    List<uint> NeuronsInLayers = new List<uint>();

    public float Fitness;
    uint LinkWeightRange;

    float WeightMutationChance;
    float LayerMuationChance;

    List<List<Neuron>> Genome = new List<List<Neuron>>();

    public NeuralNetwork(uint pNumInput, uint pNumOutput, int pBias,
        List<uint> pNeurons, uint pLinkWRange)
    {
        NumInput = pNumInput;
        NumOutput = pNumOutput;
        Bias = pBias;
        NeuronsInLayers = pNeurons;
        LinkWeightRange = pLinkWRange;

        WeightMutationChance = BirdPopulation.Instance.WeightMutationChance;
        LayerMuationChance = BirdPopulation.Instance.LayerMuationChance;

        GenerateBlankNetwork();
    }

    public NeuralNetwork(uint pNumInput, uint pNumOutput, int pBias,
        List<List<Neuron>> pGenome)
    {
        NumInput = pNumInput;
        NumOutput = pNumOutput;
        Bias = pBias;
        Genome = pGenome;

        WeightMutationChance = BirdPopulation.Instance.WeightMutationChance;
        LayerMuationChance = BirdPopulation.Instance.LayerMuationChance;
    }

    public void GenerateBlankNetwork()
    {
        List<Neuron> lastLayer = new List<Neuron>();

        for (int i = 0; i < NumInput; ++i)
            lastLayer.Add(new Neuron(NeuronType.Input, ActivationFunction.TanH));

        Genome.Add(new List<Neuron>(lastLayer));
        lastLayer.Clear();

        for (uint i = 0; i < NeuronsInLayers.Count; i++)
        {
            uint numNeuron = NeuronsInLayers[(int)i];

            for (int j = 0; j < numNeuron; ++j)
                lastLayer.Add(new Neuron(NeuronType.Hidden, ActivationFunction.TanH));

            Genome.Add(new List<Neuron>(lastLayer));
            lastLayer.Clear();
        }

        for (int i = 0; i < NumOutput; ++i)
            lastLayer.Add(new Neuron(NeuronType.Output, ActivationFunction.TanH));

        Genome.Add(new List<Neuron>(lastLayer));

        GenerateGenomeConnections();
    }

    private void GenerateGenomeConnections()
    {
        for (int i = 0; i < Genome.Count - 1; ++i) 
        {
            List<Neuron> inputLayer = Genome[i];
            List<Neuron> outputLayer = Genome[i + 1];

            foreach (Neuron input in inputLayer)
                foreach (Neuron output in outputLayer)
                    input.Links.Add(new Link(output, RngHelper.RandomInRange(LinkWeightRange)));
        }
    }

    public double[] ComputeNetwork(float[] pInputs)
    {
        //Assign Inputs
        for (int i = 0; i < NumInput; ++i)
            Genome.First()[i].OutputValue = pInputs[i];

        //Go through all layers
        for (int i = 0; i < Genome.Count; ++i)
        {
            List<Neuron> inputLayer = Genome[i];

            //Go trough input layer
            for (int j = 0; j < inputLayer.Count; ++j)
            {
                Neuron input = inputLayer[j];
                input.ComputeActivationFunction();

                //Go through links
                foreach (Link link in input.Links)
                    link.Output.InputValue += (input.OutputValue * link.Weight);                       
            }
        }

        //Retreive all outputs
        double[] outputs = new double[NumOutput];
        for (int i = 0; i < NumOutput; ++i)
            outputs[i] = Genome.Last()[i].OutputValue;

        return outputs;
    }

    #region Mutation
    private void AddNeuron(int layer)
    {
        Neuron newNeuron = new Neuron(NeuronType.Hidden, ActivationFunction.TanH);
        Genome[layer].Add(newNeuron);
        GenerateNeuronConnections(newNeuron, layer);
    }

    private void RemoveNeuron(Neuron pNeuron, int layer)
    {
        RemoveConnections(pNeuron, layer);
        Genome[layer].Remove(pNeuron);
    }

    private void GenerateNeuronConnections(Neuron pNewNeuron, int layer)
    {
        List<Neuron> inputLayer = Genome[layer - 1];
        List<Neuron> outputLayer = Genome[layer + 1];

        foreach (Neuron neuron in inputLayer)
            neuron.Links.Add(new Link(pNewNeuron, RngHelper.RandomInRange(LinkWeightRange)));

        foreach (Neuron neuron in outputLayer)
            pNewNeuron.Links.Add(new Link(neuron, RngHelper.RandomInRange(LinkWeightRange)));
    }

    private void RemoveConnections(Neuron deletedNeuron, int layer)
    {
        List<Neuron> inputLayer = Genome[layer - 1];

        foreach (Neuron neuron in inputLayer)
        {
            int index = neuron.Links.FindIndex(item => item.Output == deletedNeuron);
            if (index < 0)
                continue;

            neuron.Links.RemoveAt(index);
        }
    }

    private void TryMutateRandomLayer(List<List<Neuron>> pGenome)
    {
        float rand = UnityEngine.Random.Range(0f, 1f);
        if (rand < LayerMuationChance)
            return;

        int randLayer = UnityEngine.Random.Range(1, pGenome.Count() - 2);
        rand = UnityEngine.Random.Range(0f, 1f);

        if (rand < 0.5f)
        {
            if (pGenome[randLayer].Count > 1)
            {
                int randNeuron = UnityEngine.Random.Range(0, pGenome[randLayer].Count() - 1);
                RemoveNeuron(pGenome[randLayer][randNeuron], randLayer);
            }
        }
        else
            AddNeuron(randLayer);
    }

    private void TryMutateWeight(List<List<Neuron>> pGenome)
    {
        float rand = UnityEngine.Random.Range(0f, 1f);

        if (rand < WeightMutationChance)
            return;

        foreach (List<Neuron> neuron in pGenome)
        {
            int randBis = UnityEngine.Random.Range(0, neuron.Count - 1);
            if (neuron[randBis].Type == NeuronType.Output)
                continue;

            List<Link> links = neuron[randBis].Links;
            
            randBis = UnityEngine.Random.Range(0, links.Count - 1);
            links[randBis].Weight = RngHelper.RandomInRange(LinkWeightRange);
        }
    }
    #endregion

    #region CrossOver
    public NeuralNetwork Mate(NeuralNetwork pRecissive)
    {
        List<List<Neuron>> newGenome = new List<List<Neuron>>();
        for (int i = 0; i < Genome.Count; ++i)
            newGenome.Add(CrossOver_Layer(Genome[i], pRecissive.Genome[i]));

        int bias = RngHelper.Choose(0.5f, ref Bias, ref pRecissive.Bias);

        TryMutateRandomLayer(newGenome);
        ReconnectNeurons(newGenome);
        TryMutateWeight(newGenome);

        return new NeuralNetwork(NumInput, NumOutput, bias, newGenome);
    }

    private void ReconnectNeurons(List<List<Neuron>> pGenome)
    {
        for (int i = 0; i < pGenome.Count - 1; ++i)
        {
            List<Neuron> inputLayer = pGenome[i];
            List<Neuron> outputLayer = pGenome[i + 1];

            foreach (Neuron neuron in inputLayer)
            {
                List<Link> links = neuron.Links;
                if (links.Count > outputLayer.Count)
                {
                    int diff = links.Count - outputLayer.Count;
                    links.RemoveRange(outputLayer.Count, diff);
                }

                for (int j = 0; j < outputLayer.Count; ++j)
                {
                    if (j > links.Count - 1)
                    {
                        links.Add(new Link(outputLayer[j], RngHelper.RandomInRange(LinkWeightRange)));
                        continue;
                    }

                    links[j].Output = outputLayer[j];
                }
            }
        }
    }

    private List<Neuron> CrossOver_Layer(List<Neuron> p1, List<Neuron> p2)
    {
        List<Neuron> layerPassedOn = RngHelper.Choose(0.5f, ref p1, ref p2);
        return layerPassedOn;
    }
    #endregion
    public static bool operator <(NeuralNetwork lhs, NeuralNetwork rhs)
    {
        return lhs.Fitness < rhs.Fitness;
    }

    public static bool operator >(NeuralNetwork lhs, NeuralNetwork rhs)
    {
        return lhs.Fitness > rhs.Fitness;
    }
}

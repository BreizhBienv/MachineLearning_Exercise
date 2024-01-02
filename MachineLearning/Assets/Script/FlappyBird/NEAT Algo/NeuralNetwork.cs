using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

enum NeuronType
{
    Input,
    Bias,
    Hidden,
    Output,
}

enum ActivationFunction
{
    Sigmoid,
    TanH,
}

class Neuron
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

class Link
{
    public Neuron Output;
    public double Weight;

    public Link(Neuron pOutput, double pW)
    {
        Output = pOutput;
        Weight = pW;
    }
}

[Serializable]
public class NeuralNetwork
{
    [Header("Network params")]
    [SerializeField] public uint NumInput;
    [SerializeField] uint NumOutput;

    [SerializeField] int Bias;

    [SerializeField] uint[] NeuronsInLayers;

    [Header("NEAT")]
    [SerializeField] public float Fitness;
    [SerializeField] uint LinkWeightRange;
    public NEAT Neat = new NEAT();

    List<List<Neuron>> Neurons = new List<List<Neuron>>();

    public NeuralNetwork()
    {
        List<Neuron> lastLayer = new List<Neuron>();

        for (int i = 0; i < NumInput; ++i)
            lastLayer.Add(new Neuron(NeuronType.Input, ActivationFunction.TanH));

        Neurons.Add(new List<Neuron>(lastLayer));
        lastLayer.Clear();

        for (uint i = 0; i < NeuronsInLayers.Length; i++)
        {
            uint numNeuron = NeuronsInLayers[i];

            for (int j = 0; j < numNeuron; ++j)
                lastLayer.Add(new Neuron(NeuronType.Hidden, ActivationFunction.TanH));

            Neurons.Add(new List<Neuron>(lastLayer));
            lastLayer.Clear();
        }

        for (int i = 0; i < NumOutput; ++i)
            lastLayer.Add(new Neuron(NeuronType.Output, ActivationFunction.TanH));

        Neurons.Add(new List<Neuron>(lastLayer));

        GenerateBlankNetworkConnections();
    }

    private void GenerateBlankNetworkConnections()
    {
        for (int i = 0; i < Neurons.Count - 1; ++i) 
        {
            List<Neuron> inputLayer = Neurons[i];
            List<Neuron> outputLayer = Neurons[i + 1];

            foreach (Neuron input in inputLayer)
                foreach (Neuron output in outputLayer)
                    input.Links.Add(new Link(output, RngHelper.RandomInRange(LinkWeightRange)));
        }
    }

    public double[] ComputeNetwork(float[] pInputs)
    {
        //Assign Inputs
        for (int i = 0; i < NumInput; ++i)
            Neurons.First()[i].OutputValue = pInputs[i];

        //Go through all layers
        for (int i = 0; i < Neurons.Count; ++i)
        {
            List<Neuron> inputLayer = Neurons[i];

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
            outputs[i] = Neurons.Last()[i].OutputValue;

        return outputs;
    }

    #region Mutation
    private void AddNeuron(int layer)
    {
        Neuron newNeuron = new Neuron(NeuronType.Hidden, ActivationFunction.TanH);
        Neurons[layer].Add(newNeuron);
        GenerateNewConnections(newNeuron, layer);
    }

    private void RemoveNeuron(Neuron pNeuron, int layer)
    {
        RemoveConnections(pNeuron, layer);
        Neurons[layer].Remove(pNeuron);
    }

    private void GenerateNewConnections(Neuron pNewNeuron, int layer)
    {
        List<Neuron> inputLayer = Neurons[layer - 1];
        List<Neuron> outputLayer = Neurons[layer + 1];

        foreach (Neuron neuron in inputLayer)
            neuron.Links.Add(new Link(pNewNeuron, RngHelper.RandomInRange(LinkWeightRange)));

        foreach (Neuron neuron in outputLayer)
            pNewNeuron.Links.Add(new Link(neuron, RngHelper.RandomInRange(LinkWeightRange)));
    }

    private void RemoveConnections(Neuron deletedNeuron, int layer)
    {
        List<Neuron> inputLayer = Neurons[layer - 1];

        foreach (Neuron neuron in inputLayer)
        {
            int index = neuron.Links.FindIndex(item => item.Output == deletedNeuron);
            if (index < 0)
                continue;

            neuron.Links.RemoveAt(index);
        }
    }

    private void TryMutateRandomLayer()
    {
        float rand = UnityEngine.Random.Range(0f, 1f);
        if (rand < Neat.LayerMuationChance)
            return;

        int randLayer = UnityEngine.Random.Range(1, NeuronsInLayers.Count());
        rand = UnityEngine.Random.Range(0f, 1f);

        if (rand < 0.5f)
        {
            int randNeuron = UnityEngine.Random.Range(0, Neurons[randLayer].Count() - 1);
            RemoveNeuron(Neurons[randLayer][randNeuron], randLayer);
        }
        else
            AddNeuron(randLayer);
    }

    private void TryMutateWeight(Link pLink)
    {
        float rand = UnityEngine.Random.Range(0f, 1f);

        if (rand < Neat.WeightMutationChance)
            return;

        pLink.Weight = RngHelper.RandomInRange(LinkWeightRange);
    }
    #endregion

    #region CrossOver

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

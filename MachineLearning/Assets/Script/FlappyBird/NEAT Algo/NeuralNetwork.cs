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
    public uint ID;

    public ActivationFunction Activation;
    public double InputValue;
    public double OutputValue;

    public Neuron(NeuronType pType, uint pID, ActivationFunction pActivation)
    {
        Type = pType;
        ID = pID;
        Activation = pActivation;
        InputValue = 0;
        OutputValue = 0;
    }

    public double ComputeActivationFunction()
    {
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
    public LinkID ID;

    public double Weight;

    public Link(LinkID pID, double pW)
    {
        ID = pID;
        Weight = pW;
    }
}

struct LinkID
{
    public uint Input;
    public uint Output;

    public LinkID(uint pInput, uint pOutput)
    {
        Input = pInput;
        Output = pOutput;
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
    List<Link> Links = new List<Link>();
    List<uint> FreeIds = new List<uint>();
    uint IdCounter;

    public void GenerateBlankNetwork()
    {
        List<Neuron> lastLayer = new List<Neuron>();
        uint currID = 0;

        for (; currID < NumInput; ++currID)
            lastLayer.Add(new Neuron(NeuronType.Input, currID, ActivationFunction.TanH));

        Neurons.Add(new List<Neuron>(lastLayer));
        lastLayer.Clear();

        for (uint i = 0; i < NeuronsInLayers.Length; i++)
        {
            uint numNeuron = NeuronsInLayers[i];
            uint maxID = numNeuron + currID;

            for (; currID < maxID; ++currID)
                lastLayer.Add(new Neuron(NeuronType.Hidden, currID, ActivationFunction.TanH));

            Neurons.Add(new List<Neuron>(lastLayer));
            lastLayer.Clear();
        }

        uint maxIDbis = NumOutput + currID;
        for (; currID < maxIDbis; ++currID)
            lastLayer.Add(new Neuron(NeuronType.Output, currID, ActivationFunction.TanH));

        Neurons.Add(new List<Neuron>(lastLayer));
        IdCounter = currID;

        GenerateNetworkConnections();
    }

    private void GenerateNetworkConnections()
    {
        for (int i = 0; i < Neurons.Count - 1; ++i) 
        {
            List<Neuron> inputLayer = Neurons[i];
            List<Neuron> outputLayer = Neurons[i + 1];

            foreach (Neuron input in inputLayer)
            {
                uint id = input.ID;

                foreach (Neuron output in outputLayer)
                {
                    Links.Add(new Link(new LinkID(id, output.ID),
                        RngHelper.RandomInRange(LinkWeightRange)));
                }
            }
        }
    }

    public double[] ComputeNetwork(float[] pInputs)
    {
        //Assign Inputs
        for (int i = 0; i < NumInput; ++i)
            Neurons.First()[i].OutputValue = pInputs[i];

        //Go through all layers
        for (int i = 0; i < Neurons.Count - 1; ++i)
        {
            List<Neuron> inputLayer = Neurons[i];
            List<Neuron> outputLayer = Neurons[i + 1];

            //Go through output layer
            for (int j = 0; j < outputLayer.Count; ++j)
            {
                uint outID = outputLayer[j].ID;

                //Go through input layer
                for (int k = 0; k < inputLayer.Count; ++k)
                {
                    uint inID = inputLayer[k].ID;

                    Link link = Links.Find(item =>
                         item.ID.Input == inID &&
                         item.ID.Output == outID);

                    if (link == null)
                        continue;

                    double weight = link.Weight;
                    outputLayer[j].InputValue += (inputLayer[k].OutputValue * weight);
                }

                //Comput OutputNode
                outputLayer[j].ComputeActivationFunction();
            }
        }

        //Retreive all outputs
        double[] outputs = new double[NumOutput];
        for (int i = 0; i < NumOutput; ++i)
            outputs[i] = Neurons.Last()[i].OutputValue;

        return outputs;
    }

    #region Mutation
    private void RemoveNeuron(Neuron pNeuron, int layer)
    {
        FreeIds.Add(pNeuron.ID);
        
        Links.RemoveAll(item => 
        item.ID.Input == pNeuron.ID ||
        item.ID.Output == pNeuron.ID);

        Neurons[layer].Remove(pNeuron);
    }

    private void AddNeuron(int layer)
    {
        uint id;

        if (FreeIds.Count <= 0)
        {
            IdCounter++;
            id = IdCounter;
        }
        else
        {
            id = FreeIds[0];
            FreeIds.RemoveAt(0);
        }

        Neurons[layer].Add(new Neuron(NeuronType.Hidden, id, ActivationFunction.TanH));
        GenerateNewConnections(id, layer);
    }

    private void GenerateNewConnections(uint id, int layer)
    {
        List<Neuron> inputLayer = Neurons[layer - 1];
        List<Neuron> outputLayer = Neurons[layer + 1];

        foreach (Neuron neuron in inputLayer)
            Links.Add(new Link(new LinkID(neuron.ID, id), RngHelper.RandomInRange(LinkWeightRange)));

        foreach (Neuron neuron in outputLayer)
            Links.Add(new Link(new LinkID(id, neuron.ID), RngHelper.RandomInRange(LinkWeightRange)));
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

    private void TryMutateRandomWeight()
    {
        float rand = UnityEngine.Random.Range(0f, 1f);

        if (rand < Neat.WeightMutationChance)
            return;

        int randLink = UnityEngine.Random.Range(0, Links.Count() - 1);
        Links[randLink].Weight = RngHelper.RandomInRange(LinkWeightRange);
    }
    #endregion

    #region CrossOver

    private Neuron CrossOver_Neuron(Neuron dominant, Neuron recissive)
    {
        return RngHelper.Choose(0.5f, ref dominant, ref recissive);
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

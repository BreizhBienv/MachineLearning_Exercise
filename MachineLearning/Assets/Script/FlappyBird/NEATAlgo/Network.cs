using System.Collections.Generic;
using System.Linq;

public class Network
{
    public uint NumInput;
    uint NumOutput;

    int Bias;

    List<uint> DefaultNetwork;

    public float Fitness;
    uint LinkWeightRange;

    float WeightMutationChance;
    float LayerMuationChance;

    List<Layer> Genome;

    public Network()
    {
        RetrieveDefaultParameters();

        Genome = new List<Layer>(DefaultNetwork.Count);

        GenerateDefaultNetwork();
    }

    public Network(List<Layer> pGenome)
    {
        RetrieveDefaultParameters();
        
        Genome = pGenome;
    }

    private void RetrieveDefaultParameters()
    {
        NumInput = NetworkHelper.Instance.NumInput;
        NumOutput = NetworkHelper.Instance.NumOutput;
        Bias = NetworkHelper.Instance.Bias;
        DefaultNetwork = NetworkHelper.Instance.DefaultNetwork;

        LinkWeightRange = NetworkHelper.Instance.LinkWeightRange;
        WeightMutationChance = NetworkHelper.Instance.WeightMutationChance;
        LayerMuationChance = NetworkHelper.Instance.LayerMuationChance;
    }

    public void GenerateDefaultNetwork()
    {
        Layer newLayer = null;
        Layer lastLayer = null;

        //Input layer
        lastLayer = new Layer(NeuronType.Input, NumInput);
        Genome.Add(lastLayer);

        //TODO Input Bias

        //Hidden layers
        for (uint i = 0; i < DefaultNetwork.Count; i++)
        {
            newLayer = new Layer(NeuronType.Hidden, DefaultNetwork[(int)i]);
            Genome.Add(newLayer);

            newLayer.GenerateLayerConnections(lastLayer, false);
            lastLayer = newLayer;
        }

        //Output layer
        newLayer = new Layer(NeuronType.Output, NumOutput);
        Genome.Add(newLayer);

        newLayer.GenerateLayerConnections(lastLayer, false);
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
        if (rand > LayerMuationChance)
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

        if (rand > WeightMutationChance)
            return;

        int randBis = UnityEngine.Random.Range(0, pGenome.Count - 1);
        List<Neuron> neuron = pGenome[randBis];

        randBis = UnityEngine.Random.Range(0, neuron.Count - 1);
        if (neuron[randBis].Type == NeuronType.Output || neuron[randBis].Type == NeuronType.Bias)
            return;

        List<Link> links = neuron[randBis].Links;
            
        randBis = UnityEngine.Random.Range(0, links.Count - 1);
        links[randBis].Weight = RngHelper.RandomInRange(LinkWeightRange);
    }
    #endregion

    #region CrossOver
    public Network Mate(Network pRecissive)
    {
        List<List<Neuron>> newGenome = new List<List<Neuron>>();
        for (int i = 0; i < Genome.Count; ++i)
            newGenome.Add(CrossOver_Layer(Genome[i], pRecissive.Genome[i]));

        int bias = RngHelper.Choose(0.5f, ref Bias, ref pRecissive.Bias);

        TryMutateRandomLayer(newGenome);
        ReconnectNeurons(newGenome);
        TryMutateWeight(newGenome);

        return new Network(NumInput, NumOutput, bias, newGenome);
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
    public static bool operator <(Network lhs, Network rhs)
    {
        return lhs.Fitness < rhs.Fitness;
    }

    public static bool operator >(Network lhs, Network rhs)
    {
        return lhs.Fitness > rhs.Fitness;
    }
}

using System.Collections.Generic;
using System.Linq;

public class Network
{
    public uint NumInput;
    uint NumOutput;

    int Bias;

    List<uint> DefaultNetwork;

    public float Fitness;

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
            Genome.First().Neurons[i].OutputValue = pInputs[i];

        //Go through all layers
        for (int i = 1; i < Genome.Count; ++i)
            Genome[i].ComputLayer();

        //Retreive all outputs
        double[] outputs = new double[NumOutput];
        for (int i = 0; i < NumOutput; ++i)
            outputs[i] = Genome.Last().Neurons[i].OutputValue;

        return outputs;
    }

    #region Mutation
    private void TryMutateRandomLayer(List<Layer> pGenome)
    {
        float rand = UnityEngine.Random.Range(0f, 1f);
        if (rand >= LayerMuationChance)
            return;

        rand = UnityEngine.Random.Range(0f, 1f);
        int randLayer = UnityEngine.Random.Range(1, pGenome.Count - 1);

        //lower than .5 rmv neuron, higher -> add neuron
        if (rand < 0.5f)
        {
            //test if presence of hidden layer
            if (pGenome.Count - 2 > 0)
            {

            }
        }
        else
        {

        }
    }

    private void TryMutateWeight(List<Layer> pGenome)
    {
        float rand = UnityEngine.Random.Range(0f, 1f);
        if (rand >= WeightMutationChance)
            return;

        int randLayer = UnityEngine.Random.Range(1, pGenome.Count - 1);
        pGenome[randLayer].MutateRandomNeuronLink();

    }
    #endregion

    #region CrossOver
    public Network Mate(Network pRecissive)
    {
        return new();
    }

    private void ReconnectNeurons(List<List<Neuron>> pGenome)
    {

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

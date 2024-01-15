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
            Genome[i].ComputeLayer();

        //Retreive all outputs
        double[] outputs = new double[NumOutput];
        for (int i = 0; i < NumOutput; ++i)
            outputs[i] = Genome.Last().Neurons[i].OutputValue;

        return outputs;
    }

    private int GetNumHiddenLayer()
    {
        return Genome.FindAll(l => l.LayerType == NeuronType.Hidden).Count;
    }

    #region Mutation
    private void TryMutateRandomLayer(List<Layer> pGenome)
    {
        float rand = UnityEngine.Random.Range(0f, 1f);
        if (rand >= LayerMuationChance)
            return;

        rand = UnityEngine.Random.Range(0f, 1f);

        int numHiddenLayer = GetNumHiddenLayer();
        int randLayer = UnityEngine.Random.Range(1, numHiddenLayer);

        //lower than .5 rmv neuron, higher -> add neuron
        if (rand < 0.5f)
        {
            //test if there are hidden layers
            if (numHiddenLayer > 0)
            {
                pGenome[randLayer].RemoveRandomNeuron(pGenome[randLayer + 1]);

                //remove layer if layer.Neurons.count <= 0
                RemoveLayer(pGenome[randLayer], randLayer);
            }
        }
        else
        {
            //if only input and output layer -> create new hidden layer
            if (numHiddenLayer <= 0)
                pGenome.Insert(1, new Layer(NeuronType.Hidden, 1));
            else
                pGenome[randLayer].AddNeuron(pGenome[randLayer - 1]);
        }
    }

    private void RemoveLayer(Layer pToRemove, int pLayerIndex)
    {
        if (pToRemove.Neurons.Count > 0)
            return;

        Layer intputL = Genome[pLayerIndex - 1];
        Layer outputL = Genome[pLayerIndex + 1];

        outputL.ClearNeuronsLinks();
        outputL.GenerateLayerConnections(intputL, false);

        Genome.Remove(pToRemove);
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
        List<Layer> newGenome = new List<Layer>();

        int dominantLayers = Genome.Count - 2;
        int recissiveLayers = pRecissive.Genome.Count - 2;
        int layerNumber = RngHelper.Choose(0.5f, ref dominantLayers, ref recissiveLayers);

        Layer inputLayer = null;
        for (int i = 0; i < layerNumber; ++i)
        {
            Layer dominant = null;
            Layer recissive = null;

            if (i < Genome.Count - 1)
                dominant = Genome[i];

            if (i < pRecissive.Genome.Count - 1)
                recissive = pRecissive.Genome[i];
                
            Layer choosed = RngHelper.Choose(0.5f, ref dominant, ref recissive);

            if (choosed == null)
                continue;

            newGenome.Add(choosed);

            choosed.GenerateLayerConnections(inputLayer, true);
            inputLayer = choosed;
        }

        Layer outputDominant = Genome.Last();
        Layer outputRecissive = pRecissive.Genome.Last();

        newGenome.Add(RngHelper.Choose(0.5f, ref outputDominant, ref outputRecissive));

        TryMutateRandomLayer(newGenome);
        TryMutateWeight(newGenome);

        return new(newGenome);
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

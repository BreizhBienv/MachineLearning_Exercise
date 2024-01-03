using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

public class Layer
{
    public List<Neuron> Neurons;

    public Layer(NeuronType pType, uint pNumNeurons)
    {
        Neurons = new List<Neuron>((int)pNumNeurons);

        for (int i = 0; i < pNumNeurons; ++i)
            Neurons.Add(new Neuron(pType, ActivationFunction.TanH));
    }

    public void GenerateLayerConnections(Layer pInputLayer, bool pShouldKeepOldW)
    {
        foreach (Neuron neuron in Neurons)
            neuron.GenerateLayerLinks(pInputLayer, pShouldKeepOldW);
    }

    public void ComputLayer()
    {
        foreach (Neuron n in Neurons)
            n.ComputeActivationFunction();
    }

    public void MutateRandomNeuronLink()
    {
        int randN = UnityEngine.Random.Range(0, Neurons.Count - 1);
        Neurons[randN].MutaterandomLink();
    }
}

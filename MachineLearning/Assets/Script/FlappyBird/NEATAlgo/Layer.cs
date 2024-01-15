using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

public class Layer
{
    public NeuronType LayerType;
    public List<Neuron> Neurons;

    public Layer(NeuronType pType, uint pNumNeurons)
    {
        LayerType = pType;
        Neurons = new List<Neuron>((int)pNumNeurons);

        for (int i = 0; i < pNumNeurons; ++i)
            Neurons.Add(new Neuron(pType, ActivationFunction.TanH));
    }

    public void GenerateLayerConnections(Layer pInputLayer, bool pShouldKeepOldW)
    {
        if (pInputLayer == null)
            return;

        foreach (Neuron neuron in Neurons)
            neuron.GenerateInputLinks(pInputLayer, pShouldKeepOldW);
    }

    public void ComputeLayer()
    {
        foreach (Neuron n in Neurons)
            n.ComputeActivationFunction();
    }

    public void MutateRandomNeuronLink()
    {
        int randN = UnityEngine.Random.Range(0, Neurons.Count - 1);
        Neurons[randN].MutaterandomLink();
    }

    public void ClearNeuronsLinks()
    {
        foreach (var neuron in Neurons)
            neuron.ClearLinks();
    }

    public void AddNeuron(Layer pInputLayer)
    {
        Neuron newNeuron = new Neuron(NeuronType.Hidden, ActivationFunction.TanH);
        Neurons.Add(newNeuron);
        newNeuron.GenerateInputLinks(pInputLayer, false);
    }

    public void RemoveRandomNeuron(Layer pOutputlayer)
    {
        int randN = UnityEngine.Random.Range(0, Neurons.Count - 1);
        RemoveNeuron(Neurons[randN], pOutputlayer);
    }

    private void RemoveNeuron(Neuron pToRemove, Layer pOutputLayer)
    {
        Neurons.Remove(pToRemove);
        foreach (Neuron neuron in pOutputLayer.Neurons)
            neuron.RemoveLink(pToRemove);
    }
}

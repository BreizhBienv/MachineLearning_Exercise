using System.Collections;
using System.Collections.Generic;

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
            neuron.GenerateNewLinks(pInputLayer, pShouldKeepOldW);
    }
}

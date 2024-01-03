using System.Collections.Generic;

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

public class Link
{
    public Neuron Input;
    public double Weight;

    public Link(Neuron pInput, double pW)
    {
        Input = pInput;
        Weight = pW;
    }
}

public class Neuron
{
    public NeuronType           Type;
    public ActivationFunction   Activation;

    public double               OutputValue;
    private uint                LinkWeightRange;

    public List<Link>           InputLinks;

    public Neuron(NeuronType pType, ActivationFunction pActivation)
    {
        Type = pType;
        Activation = pActivation;
        LinkWeightRange = NetworkHelper.Instance.LinkWeightRange;
        InputLinks = new List<Link>();
    }

    public void ComputeActivationFunction()
    {
        if (Type == NeuronType.Input || Type == NeuronType.Bias)
            return;

        double result = 0;
        double weightedSum = ComputeWeightedSum();

        switch (Activation)
        {
            case ActivationFunction.Sigmoid:
                result = NetworkHelper.Sigmoid(weightedSum);
                break;

            case ActivationFunction.TanH:
                result = NetworkHelper.TanH(weightedSum);
                break;
        }

        OutputValue = result;
    }

    private double ComputeWeightedSum()
    {
        double weightedSum = 0;

        foreach (Link link in InputLinks)
            weightedSum += (link.Input.OutputValue * link.Weight);

        return weightedSum;
    }

    #region Link
    public void GenerateLayerLinks(Layer pInputLayer, bool pShouldKeepOldW)
    {
        if (pShouldKeepOldW)
            GenerateLinkWithOldW(pInputLayer);
        else
            GenerateNewLink(pInputLayer);
    }

    private void GenerateNewLink(Layer pInputLayer)
    {
        foreach (Neuron neuron in pInputLayer.Neurons)
            InputLinks.Add(new Link(neuron, RngHelper.RandomHalfExtent(LinkWeightRange)));
    }

    private void GenerateLinkWithOldW(Layer pInputLayer)
    {
        List<Link> links = new List<Link>(pInputLayer.Neurons.Count);
        for (int i = 0; i < pInputLayer.Neurons.Count; ++i)
        {
            bool newLink = i > InputLinks.Count - 1;

            Neuron input = pInputLayer.Neurons[i];
            double oldW = newLink ?
                RngHelper.RandomHalfExtent(LinkWeightRange) :
                InputLinks[i].Weight;

            links.Add(new Link(input, oldW));
        }
        InputLinks = links;
    }

    public void MutaterandomLink()
    {
        int randLink = UnityEngine.Random.Range(0, InputLinks.Count - 1);
        InputLinks[randLink].Weight = RngHelper.RandomHalfExtent(LinkWeightRange);
    }
    #endregion
}
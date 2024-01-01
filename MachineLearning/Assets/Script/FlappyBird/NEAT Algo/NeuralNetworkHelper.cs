using System;

public static class NeuralNetworkHelper
{
    public static double Sigmoid(double value)
    {
        double k = Math.Exp(value);
        return k / (1.0f + k);
    }
}

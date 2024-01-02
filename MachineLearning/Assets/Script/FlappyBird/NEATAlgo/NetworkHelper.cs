using System;
using System.Collections.Generic;
using UnityEngine;

public class NetworkHelper : MonoBehaviour
{
    public static NetworkHelper Instance { get; private set; }

    [Header("Network params")]
    [SerializeField] public uint        NumInput;
    [SerializeField] public uint        NumOutput;
    [SerializeField] public int         Bias;
    [SerializeField] public List<uint>  DefaultNetwork = new List<uint>();

    [Header("Algo param")]
    [SerializeField] public uint LinkWeightRange = 10;
    [Range(0.0f, 1.0f)]
    [SerializeField] public float WeightMutationChance;
    [Range(0.0f, 1.0f)]
    [SerializeField] public float LayerMuationChance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public static double Sigmoid(double pValue)
    {
        double k = Math.Exp(pValue);
        return k / (1.0f + k);
    }

    public static double TanH(double pValue)
    {
        return Math.Tanh(pValue);
    }
}

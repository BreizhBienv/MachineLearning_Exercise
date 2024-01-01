using System;
using UnityEngine;

[Serializable]
public class NEAT
{
    [Header("Mutation")]
    [Range(0.0f, 1.0f)]
    [SerializeField] public float WeightMutationChance;

    [Range(0.0f, 1.0f)]
    [SerializeField] public float LayerMuationChance;
}

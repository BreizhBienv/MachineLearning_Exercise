using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdIndividual
{
    //Time alive
    public float Fitness;

    public float TopPipeWheight; //Weight of distance between top pipe and bird
    public float BottomPipeWheight; //Weight of distance between bottom pipe and bird
    public float BirdHeightWheight;

    public float Bias;

    float RangeOfRandom;

    public BirdIndividual()
    {
        RangeOfRandom = BirdPopulation.Instance.RangeOfRandom;

        TopPipeWheight =    Random.Range(-RangeOfRandom, RangeOfRandom);
        BottomPipeWheight = Random.Range(-RangeOfRandom, RangeOfRandom);
        BirdHeightWheight = Random.Range(-RangeOfRandom, RangeOfRandom);
        Bias =              Random.Range(-RangeOfRandom, RangeOfRandom);
    }

    public BirdIndividual(float TopPipe, float BottomPipe, float BirdHeight, float BiasP)
    {
        RangeOfRandom = BirdPopulation.Instance.RangeOfRandom;

        TopPipeWheight = TopPipe;
        BottomPipeWheight = BottomPipe;
        BirdHeightWheight = BirdHeight;
        Bias = BiasP;
    }

    private float CrossGenes(float Parent1, float Parent2)
    {
        float proba = Random.Range(0, 100);

        //Mutation to keep diversity if proba > 90f 
        float gene = Random.Range(-RangeOfRandom, RangeOfRandom);

        if (proba < 45f)    //Take self gene
        {
            gene = Parent1;
        }
        else if (45f <= proba && proba < 90f)   //Take other gene
        {
            gene = Parent2;
        }

        return gene;
    }

    public BirdIndividual Mate(BirdIndividual Other)
    {
        // create new Individual(offspring) using  
        return new BirdIndividual(
            CrossGenes(TopPipeWheight, Other.TopPipeWheight),
            CrossGenes(BottomPipeWheight, Other.BottomPipeWheight),
            CrossGenes(BirdHeightWheight, Other.BirdHeightWheight),
            CrossGenes(Bias, Other.Bias));
    }

    public static bool operator <(BirdIndividual lhs, BirdIndividual rhs)
    {
        return lhs.Fitness < rhs.Fitness;
    }

    public static bool operator >(BirdIndividual lhs, BirdIndividual rhs)
    {
        return lhs.Fitness > rhs.Fitness;
    }
}

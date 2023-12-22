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

    //Raycast send backward to see if safe to jump or fall
    //x is toward top and y toward bottom
    public Vector2 BackRay;

    public float Bias;

    float RangeOfRandom;
    float RangeRandBias;

    public BirdIndividual()
    {
        RangeOfRandom = BirdPopulation.Instance.RangeOfRandom;
        RangeRandBias = BirdPopulation.Instance.RangeRandBias;

        TopPipeWheight      = Random.Range(-RangeOfRandom, RangeOfRandom);
        BottomPipeWheight   = Random.Range(-RangeOfRandom, RangeOfRandom);
        BirdHeightWheight   = Random.Range(-RangeOfRandom, RangeOfRandom);

        BackRay.x           = Random.Range(-RangeOfRandom, RangeOfRandom);
        BackRay.y           = Random.Range(-RangeOfRandom, RangeOfRandom);

        Bias                = Random.Range(-RangeRandBias, RangeRandBias);
    }

    public BirdIndividual(float pTopPipe, float pBottomPipe, float pBirdHeight, Vector2 pBackRay, float pBiasP)
    {
        RangeOfRandom = BirdPopulation.Instance.RangeOfRandom;
        RangeRandBias = BirdPopulation.Instance.RangeRandBias;

        TopPipeWheight      = pTopPipe;
        BottomPipeWheight   = pBottomPipe;
        BirdHeightWheight   = pBirdHeight;

        BackRay             = pBackRay;

        Bias                = pBiasP;
    }

    private float CrossGenes(float pParent1, float pParent2)
    {
        float proba = Random.Range(0, 100);

        //Mutation to keep diversity if proba > 90f 
        float gene = Random.Range(-RangeOfRandom, RangeOfRandom);

        if (proba < 45f)    //Take self gene
        {
            gene = pParent1;
        }
        else if (45f <= proba && proba < 90f)   //Take other gene
        {
            gene = pParent2;
        }

        return gene;
    }

    public BirdIndividual Mate(BirdIndividual pOther)
    {
        float proba = Random.Range(0, 100);

        //Mutation to keep diversity if proba > 90f 
        float bias = Random.Range(-RangeRandBias, RangeRandBias);

        if (proba < 45f)    //Take self gene
        {
            bias = Bias;
        }
        else if (45f <= proba && proba < 90f)   //Take other gene
        {
            bias = pOther.Bias;
        }

        // create new Individual(offspring) using  
        return new BirdIndividual(
            CrossGenes(TopPipeWheight, pOther.TopPipeWheight),
            CrossGenes(BottomPipeWheight, pOther.BottomPipeWheight),
            CrossGenes(BirdHeightWheight, pOther.BirdHeightWheight),
            new Vector2(CrossGenes(BackRay.x, pOther.BackRay.x), CrossGenes(BackRay.y, pOther.BackRay.y)),
            bias);
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

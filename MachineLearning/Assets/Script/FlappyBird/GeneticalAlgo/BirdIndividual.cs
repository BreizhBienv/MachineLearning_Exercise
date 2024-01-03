using UnityEngine;

public class BirdIndividual
{
    //Time alive
    public float Fitness;

    public float TopHeightW;    //Weight top pipe/bird distance on y-axis
    public float BotHeightW;    //Weight bottom pipe/bird distance on y-axis

    public float LastDistW;     //Weight of last pipe/bird distance on x-axis 
    public float NextDistW;     //Weight of next pipe/bird distance on x-axis

    float RangeOfRandom;

    public BirdIndividual()
    {
        RangeOfRandom = NetworkHelper.Instance.LinkWeightRange;

        TopHeightW = Random.Range(-RangeOfRandom, RangeOfRandom);
        BotHeightW = Random.Range(-RangeOfRandom, RangeOfRandom);

        LastDistW = Random.Range(-RangeOfRandom, RangeOfRandom);
        NextDistW = Random.Range(-RangeOfRandom, RangeOfRandom);
    }

    public BirdIndividual(float pTopPipe, float pBotPipe, float pLastDist, float pNextDist)
    {
        RangeOfRandom = NetworkHelper.Instance.LinkWeightRange;

        TopHeightW = pTopPipe;
        BotHeightW = pBotPipe;

        LastDistW = pLastDist;
        NextDistW = pNextDist;
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
        // create new Individual(offspring) using  
        return new BirdIndividual(
            CrossGenes(TopHeightW, pOther.TopHeightW),
            CrossGenes(BotHeightW, pOther.BotHeightW),
            CrossGenes(LastDistW, pOther.LastDistW),
            CrossGenes(NextDistW, pOther.NextDistW));
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

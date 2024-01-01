using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XORIndividual
{
    public float[] Chromosome = { 0.0f, 0.0f, 0.0f, 0.0f };
    public float Fitness;

    public XORIndividual()
    {
        for (int i = 0; i < Chromosome.Length; ++i)
        {
            Chromosome[i] = Random.Range(
                -XORPopulation.Instance.RandomRange,
                XORPopulation.Instance.RandomRange);
        }

        Fitness = CalculFitness();
    }

    public XORIndividual(float[] chromosome)
    {
        Chromosome = chromosome;
        Fitness = CalculFitness();
    }

    public XORIndividual Mate(XORIndividual Other)
    {
        float[] target = XORPopulation.Instance.Target;
        int len = target.Length;

        //Chromosome of offspring
        float[] childChromosome = new float[len];

        for (int i = 0; i < len; ++i)
        {
            float rand = Random.Range(0.0f, 100.0f);
            float proba = rand / 100.0f;

            if (proba < 0.45f)                          //Take self gene
            {
                childChromosome[i] = Chromosome[i];
            }
            else if (0.45f <= proba && proba < 0.90f)   //Take other gene
            {
                childChromosome[i] = Other.Chromosome[i];
            }
            else if (0.90f <= proba)                    //Mutation to keep diversity
            {
                childChromosome[i] = Random.Range(
                    -XORPopulation.Instance.RandomRange,
                    XORPopulation.Instance.RandomRange);
            }
        }

        // create new Individual(offspring) using  
        return new XORIndividual(childChromosome);
    }

    float CalculFitness()
    {
        float value = 0.0f;

        float[] target = XORPopulation.Instance.Target;

        for (int i = 0; i < target.Length; ++i)
        {
            float targetGene = target[i];
            float gene = Chromosome[i];
            value += Mathf.Abs(targetGene - gene);
        }

        return value / target.Length;
    }

    public static bool operator <(XORIndividual lhs, XORIndividual rhs)
    {
        return lhs.Fitness < rhs.Fitness;
    }

    public static bool operator >(XORIndividual lhs, XORIndividual rhs)
    {
        return lhs.Fitness > rhs.Fitness;
    }
}

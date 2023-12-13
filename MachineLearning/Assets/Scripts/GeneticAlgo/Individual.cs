using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Individual
{
    List<float> Chromosome;
    public float Fitness;

    public Individual()
    {
        Chromosome = new List<float>(Population.Instance.Target.Count);

        for (int i = 0; i < Chromosome.Capacity; ++i)
        {
            Chromosome[i] = Random.Range(
                -Population.Instance.RandomRange,
                Population.Instance.RandomRange);
        }

        Fitness = CalculFitness();
    }

    public Individual(List<float> chromosome)
    {
        Chromosome = chromosome;
        Fitness = CalculFitness();
    }

    public Individual Mate(Individual Other)
    {
        List<float> target = Population.Instance.Target;
        int len = target.Count;

        //Chromosome of offspring
        List<float> childChromosome = new List<float>(len);

        for (int i = 0; i < len; ++i)
        {
            float proba = Random.Range(0, 100) / 100;

            //Take self gene
            if (proba < 0.45f)
                childChromosome[i] = Chromosome[i];

            //Take other gene
            if (0.45f <= proba && proba < 0.90f)
                childChromosome[i] = Other.Chromosome[i];

            //Mutation to keep diversity
            if (proba <= 0.90f)
                childChromosome[i] = Random.Range(float.MinValue, float.MaxValue);
        }

        // create new Individual(offspring) using  
        return new Individual(childChromosome);
    }

    float CalculFitness()
    {
        float value = float.MaxValue;

        List<float> target = Population.Instance.Target;

        for (int i = 0; i < target.Count; ++i)
        {
            float targetGene = target[i];
            float gene = Mathf.Abs(Chromosome[i]);
            value += (targetGene - gene);
        }

        return value /target.Count;
    }

    public static bool operator <(Individual lhs, Individual rhs)
    {
        return lhs.Fitness < rhs.Fitness;
    }

    public static bool operator >(Individual lhs, Individual rhs)
    {
        return lhs.Fitness > rhs.Fitness;
    }
}

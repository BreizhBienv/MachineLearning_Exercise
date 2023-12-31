using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Population : MonoBehaviour
{
    public static Population Instance { get; private set; }

    public readonly float[] Target = 
    {
        0.0f,   //00
        1.0f,   //10
        1.0f,   //01
        0.0f    //11
    };

    int currentGeneration = -1;

    List<Individual> population;

    [Range(0.0f, 500.0f)]
    public float RandomRange = 500.0f;

    [Range(0, 100)]
    public int MaxPopulation = 100;

    [Range(1, 99)]
    public int TopSurvivorPercent = 10;

    [Range(1, 99)]
    public int TopMatingPercent = 50;

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

    private void Start()
    {
        population = new List<Individual>(MaxPopulation);

        ProgenitorGeneration();
    }

    public void ProgenitorGeneration()
    {
        for (int i = 0; i < MaxPopulation; ++i)
            population.Add(new Individual());

        //Sort population by ascending fitness
        population = population.OrderBy(o => o.Fitness).ToList();
        currentGeneration++;
    }

    public void NewGeneration()
    {
        List<Individual> newGen = new List<Individual>(MaxPopulation);

        //Perform elitist selction from population
        //only top 'TopSurvivorPercent' of the population will be part of the new generation
        int s = (TopSurvivorPercent * MaxPopulation) / 100;
        for (int i = 0; i < s; ++i)
            newGen.Add(population[i]);


        //perform mating
        //only top 50% of fittest individual from current generation will mate
        for (int i = s; i < MaxPopulation; ++i)
        {
            int populationSample = (TopMatingPercent / 100) * MaxPopulation;

            int r = Random.Range(0, populationSample);
            Individual parent1 = population[r];

            r = Random.Range(0, populationSample);
            Individual parent2 = population[r];

            Individual offspring = parent1.Mate(parent2);
            newGen.Add(offspring);
        }

        population = newGen;

        //Sort population by ascending fitness
        population = population.OrderBy(o => o.Fitness).ToList();
        currentGeneration++;
    }

    Individual GetChosenOne()
    {
        return population[0];
    }

    public float GetOutput(int input1, int input2)
    {
        float output = 0;

        if(input1 == 1)
        {
            if (input2 == 1)
                output = GetChosenOne().Chromosome[0];
            else if (input2 == 0)
                output = GetChosenOne().Chromosome[1];
        }
        else if(input1 == 0)
        {
            if (input2 == 1)
                output = GetChosenOne().Chromosome[2];
            else if (input2 == 0)
                output = GetChosenOne().Chromosome[3];
        }

        return output;
    }
}

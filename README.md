# MachineLearning_Exercise

The goal of this exercise is to learn the ins and outs of machine learning, and in this project, of genetic algorithms. <br>
The exercise present itself in two step: <br>
> Step 1 - XOR Training <br>
> Step 2 - FlappyBird <br>


### XOR Training
I had a program that could display a result depending of imputs, the goal was to use a genetic algorithm to train the AI to display the closest value possible to a list of targets we gave it. <br>
To check which AI was the closest to the set of targets, I calculated the average of all the distances between the gene and the target it is assaciated with.<br>
     
    float CalculFitness()
    {
        float value = 0.0f;
        float[] target = Population.Instance.Target;
        for (int i = 0; i < target.Length; ++i)
        {
            float targetGene = target[i];
            float gene = Chromosome[i];
            value += Mathf.Abs(targetGene - gene);
        }
        return value / target.Length;
    }

![XOR](ReadMeAssets/XOR.PNG)

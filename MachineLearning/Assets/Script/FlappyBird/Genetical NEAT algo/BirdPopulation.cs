using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BirdPopulation : MonoBehaviour
{
    public static BirdPopulation Instance { get; private set; }

    [Header("UI/Population")]
    [SerializeField] private TMP_InputField InputMaxPop;

    [Header("UI/Survivor")]
    [SerializeField] private TMP_InputField InputS;
    [SerializeField] private Scrollbar      ScrollBarS;

    [Header("UI/Mating")]
    [SerializeField] private TMP_InputField InputM;
    [SerializeField] private  Scrollbar     ScrollBarM;

    [Header("Algo param")]
    [SerializeField] public float RangeOfRandom = 500f;
    [SerializeField] public float RangeRandBias = 500f;

    [SerializeField] private GameObject BirdPrefab;

    int MaxPopulation = 10;
    int TopSurvivorPercent = 10;
    int TopMatingPercent = 50;

    int CurrentGen = -1;

    List<BirdIndividual> Population = new List<BirdIndividual>();

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

    #region UI
    public void ChangeValue_MaxPop_InputField()
    {
        int.TryParse(InputMaxPop.text, out MaxPopulation);
    }

    public void ChangeValue_Survivor_InputField()
    {
        int tmp;
        int.TryParse(InputS.text, out tmp);

        tmp = Mathf.Clamp(tmp, 1, 99);
        InputS.text = tmp.ToString();

        TopSurvivorPercent = tmp;

        float toScrollB = tmp / 100f;
        ScrollBarS.value = toScrollB;
    }

    public void ChangeValue_Survivor_ScrollBar()
    {
        float value = Mathf.Clamp(ScrollBarS.value, 0.01f, 0.99f);
        ScrollBarS.value = value;

        value *= 100f;
        int valueI = (int)value;

        TopSurvivorPercent = valueI;
        InputS.text = valueI.ToString();
    }

    public void ChangeValue_Mating_InputField()
    {
        int tmp;
        int.TryParse(InputM.text, out tmp);

        tmp = Mathf.Clamp(tmp, 1, 99);
        InputM.text = tmp.ToString();

        TopMatingPercent = tmp;

        float toScrollB = tmp / 100f;
        ScrollBarM.value = toScrollB;
    }

    public void ChangeValue_Mating_ScrollBar()
    {
        float value = Mathf.Clamp(ScrollBarM.value, 0.01f, 0.99f);
        ScrollBarM.value = value;

        value *= 100f;
        int valueI = (int)value;

        TopMatingPercent = valueI;
        InputM.text = valueI.ToString();
    }

    public void StartGame()
    {
        GenerateNewPopulation();
    }
    #endregion

    #region Genetical Algo

    private void GenerateNewPopulation()
    {
        if (Population.Count <= 0)
            ProgenitorGen();
        else
            NewGen();

        InstantiatePopulation();
        CurrentGen++;
        GameManager.Instance.UpdateGen(CurrentGen);
        GameManager.Instance.UpdateAlive();
    }

    private void ProgenitorGen()
    {
        for (int i = 0; i < MaxPopulation; ++i)
            Population.Add(new BirdIndividual());
    }

    private void NewGen()
    {
        //Sort population by descending fitness
        Population = Population.OrderByDescending(o => o.Fitness).ToList();

        List<BirdIndividual> newGen = new List<BirdIndividual>(MaxPopulation);

        //Perform elitist selction from population
        //only top 'TopSurvivorPercent' of the population will be part of the new generation
        int s = (TopSurvivorPercent * MaxPopulation) / 100;
        for (int i = 0; i < s; ++i)
        {
            BirdIndividual indiv = Population[i];
            indiv.Fitness = 0;
            newGen.Add(indiv);
        }

        //perform mating
        //only top 50% of fittest individual from current generation will mate
        for (int i = s; i < MaxPopulation; ++i)
        {
            int populationSample = (TopMatingPercent / 100) * MaxPopulation;

            int r = Random.Range(0, populationSample);
            BirdIndividual parent1 = Population[r];

            r = Random.Range(0, populationSample);
            BirdIndividual parent2 = Population[r];

            BirdIndividual offspring = parent1.Mate(parent2);
            newGen.Add(offspring);
        }

        Population.Clear();
        Population = newGen;
    }

    private void InstantiatePopulation()
    {
        for (int i = 0; i < MaxPopulation; ++i)
        {
            GameObject go = Instantiate(BirdPrefab, (Vector3.up * 1.28f), Quaternion.identity);
            go.GetComponent<FlappyBehaviour>().Individual = Population[i];
            GameManager.Instance.BirdsAlive.Add(go);
        }
    }

    #endregion
}

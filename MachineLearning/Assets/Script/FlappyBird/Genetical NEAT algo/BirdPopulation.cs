using System;
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
    [SerializeField] private Toggle         NewGenToggle;

    [Header("UI/Survivor")]
    [SerializeField] private TMP_InputField InputS;
    [SerializeField] private Scrollbar      ScrollBarS;

    [Header("UI/Mating")]
    [SerializeField] private TMP_InputField InputM;
    [SerializeField] private  Scrollbar     ScrollBarM;

    [Header("Algo param")]
    [SerializeField] public float RangeOfRandom = 10f;
    [SerializeField] private GameObject BirdPrefab;

    int MaxPopulation = 10;
    int TopSurvivorPercent = 10;
    int TopMatingPercent = 50;

    [NonSerialized] public int CurrentGen = 0;

    List<BirdIndividual> Population = new List<BirdIndividual>();

    bool IsGeneratingNewGen = true;

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

    public void ChangeValue_NewGen_Toggle()
    {
        IsGeneratingNewGen = NewGenToggle.isOn;
    }

    public void ResetSpecies_Button()
    {
        Population.Clear();
        CurrentGen = 0;
        GameManager.Instance.ResetScores();
    }

    public void StartGame()
    {
        GenerateNewPopulation();
    }
    #endregion

    #region Genetical Algo

    private void GenerateNewPopulation()
    {
        int addToGen = 0;

        if (Population.Count <= 0)
        {
            NewSpecies(null);
            addToGen++;
        }
        else if (IsGeneratingNewGen)
        {
            NewGen();
            addToGen++;
        }

        CurrentGen += addToGen;
        GameManager.Instance.UpdateGen(CurrentGen);

        InstantiatePopulation();
        GameManager.Instance.UpdateAlive();
    }

    public void NewSpecies(BirdIndividual baseCopie)
    {
        Population.Clear();

        if (baseCopie == null)
        {
            for (int i = 0; i < MaxPopulation; ++i)
                Population.Add(new BirdIndividual());

            return;
        }

        for (int i = 0; i < MaxPopulation; ++i)
            Population.Add(new BirdIndividual(
                baseCopie.TopHeightW, baseCopie.BotHeightW,
                baseCopie.LastDistW, baseCopie.NextDistW));
    }

    private void NewGen()
    {
        //Sort population by descending fitness
        Population = GetSortedPopulation();

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

            int r = UnityEngine.Random.Range(0, populationSample);
            BirdIndividual parent1 = Population[r];

            r = UnityEngine.Random.Range(0, populationSample);
            BirdIndividual parent2 = Population[r];

            BirdIndividual offspring = parent1.Mate(parent2);
            newGen.Add(offspring);
        }

        Population.Clear();
        Population = newGen;
    }

    public List<BirdIndividual> GetSortedPopulation()
    {
        return Population.OrderByDescending(o => o.Fitness).ToList();
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

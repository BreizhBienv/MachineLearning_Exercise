using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Canvas")]
    [SerializeField] private GameObject GameOverCanvas;
    [SerializeField] private GameObject InGameCanvas;

    [Header("Scores")]
    [SerializeField] private TextMeshProUGUI ScoreTxt;
    [SerializeField] private TextMeshProUGUI HighScoreTxt;

    [Header("Population Info")]
    [SerializeField] private TextMeshProUGUI GenTxt;
    [SerializeField] private TextMeshProUGUI AliveTxt;

    [Header("SceneObjects")]
    [SerializeField] private PipeManager PipeSpawner;

    private int Score = 0;
    private int HighScore = 0;

    [NonSerialized] public List<GameObject> BirdsAlive = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        Time.timeScale = 0f;
    }

    private void Start()
    {
        ScoreTxt.text = Score.ToString();
    }

    private void Update()
    {
        KillPopulation();
    }

    public void GameOver()
    {
        if (BirdsAlive.Count > 0)
            return;

        InGameCanvas.SetActive(false);
        GameOverCanvas.SetActive(true);

        Time.timeScale = 0f;
    }

    public void Play()
    {
        Time.timeScale = 1f;

        GameOverCanvas.SetActive(false);
        InGameCanvas.SetActive(true);

        Score = 0;
        ScoreTxt.text = Score.ToString();

        PipeSpawner.OnResetGame();
    }

    private void KillPopulation()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (GameObject obj in BirdsAlive)
                Destroy(obj);

            BirdsAlive.Clear();
            GameOver();
        }
    }

    public void RemoveAlive(GameObject obj)
    {
        BirdsAlive.Remove(obj);
        UpdateAlive();
    }

    #region UI
    public void UpdateScore()
    {
        Score++;
        ScoreTxt.text = Score.ToString();

        if (Score > HighScore)
            UpdateHighScore(Score);
    }

    private void UpdateHighScore(int NewHighScore)
    {
        HighScore = NewHighScore;
        HighScoreTxt.text = HighScore.ToString();
    }

    public void UpdateGen(int newGen)
    {
        GenTxt.text = newGen.ToString();
    }

    public void UpdateAlive()
    {
        AliveTxt.text = BirdsAlive.Count.ToString();
    }

    public void ResetScores()
    {
        Score = 0;
        ScoreTxt.text = Score.ToString();

        UpdateHighScore(Score);
    }

    public void SaveAI()
    {
        List<Network> pop = BirdPopulation.Instance.GetSortedPopulation();

        if (pop.Count <= 0)
            return;

        SerializeData.SaveGame(pop[0], BirdPopulation.Instance.CurrentGen, HighScore);
    }

    public void LoadAI()
    {
        BirdIndividual loadedBird = null;
        int loadedGen = -1;
        int loadedScore = 0;

        SerializeData.LoadData(out loadedBird, out loadedGen, out loadedScore);

        UpdateHighScore(loadedScore);
        BirdPopulation.Instance.CurrentGen = loadedGen;
        BirdPopulation.Instance.NewSpecies(loadedBird);
    }
    #endregion
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject GameOverCanvas;
    [SerializeField] private GameObject InGameCanvas;

    [SerializeField] private TextMeshProUGUI ScoreTxt;

    [SerializeField] private TextMeshProUGUI GenTxt;
    [SerializeField] private TextMeshProUGUI AliveTxt;

    [SerializeField] private PipeManager PipeSpawner;

    private int Score = 0;

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
    }

    public void UpdateGen(int newGen)
    {
        GenTxt.text = newGen.ToString();
    }

    public void UpdateAlive()
    {
        AliveTxt.text = BirdsAlive.Count.ToString();
    }
    #endregion
}

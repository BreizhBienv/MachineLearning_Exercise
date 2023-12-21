using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject GameOverCanvas;
    [SerializeField] private GameObject InGameCanvas;

    [SerializeField] private TextMeshProUGUI ScoreTxt;
    [SerializeField] private TextMeshProUGUI HighScoreTxt;

    [SerializeField] private PipeManager PipeSpawner;

    [SerializeField] private FlappyBehaviour Flappy;

    private int Score = 0;

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

        HighScoreTxt.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
        UpdateHighScore();
    }

    public void GameOver()
    {
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

        Flappy.OnResetGame();
        PipeSpawner.OnResetGame();
    }

    public void UpdateScore()
    {
        Score++;
        ScoreTxt.text = Score.ToString();

        UpdateHighScore();
    }

    void UpdateHighScore()
    {
        if (Score < PlayerPrefs.GetInt("HighScore"))
            return;

        PlayerPrefs.SetInt("HighScore", Score);
        HighScoreTxt.text = Score.ToString();
    }
}

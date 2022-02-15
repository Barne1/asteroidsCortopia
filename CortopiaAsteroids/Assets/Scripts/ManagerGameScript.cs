using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManagerGameScript : MonoBehaviour //Cannot be Called GameManager
{
    public bool GameOver = false;

    public static ManagerGameScript Singleton;

    [SerializeField] private GameObject GameOverScreen;
    [SerializeField] private Text HighScoreText;

    public Player player { get; protected set; }
    public LivesUIManager livesUIManager { get; protected set; }
    public AsteroidSpawner asteroidSpawner { get; protected set; }
    public CollisionManager collisionManager { get; protected set; }
    public WorldBox worldBox { get; protected set; }
    public ScoreUIManager scoreUIManager { get; protected set; }

    private void Awake()
    {
        Singleton = this;

        player = FindObjectOfType<Player>();
        livesUIManager = FindObjectOfType<LivesUIManager>();
        asteroidSpawner = FindObjectOfType<AsteroidSpawner>();
        collisionManager = FindObjectOfType<CollisionManager>();
        worldBox = FindObjectOfType<WorldBox>();
        scoreUIManager = FindObjectOfType<ScoreUIManager>();
        
        GameOverScreen.SetActive(false);
    }

    private void Start()
    {
        player.OnDeath.AddListener(SetGameOver);
    }

    public void SetGameOver()
    {
        GameOver = true;
        GameOverScreen.SetActive(true);

        int HighScore = PlayerPrefs.GetInt("highscore", 0);
        if (scoreUIManager.Score > HighScore)
        {
            HighScore = scoreUIManager.Score;
            PlayerPrefs.SetInt("highscore", HighScore);
            PlayerPrefs.Save();
        }

        HighScoreText.text = "Current Highscore: " + HighScore;
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}

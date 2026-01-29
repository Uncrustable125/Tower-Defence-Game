using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int lives, money;

    [SerializeField] UIManager uiManager;
    public static GameManager Instance;
    bool gameover = false;
    private bool gameStarted = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    // Async Start ensures levels are loaded before game starts
    private async void Start()
    {
        // Wait for LevelDatabase to finish loading all levels
        await LevelDatabase.Instance.EnsureLoadedAsync();

        Debug.Log("Levels ready!");
        StartGame();
    }



    void StartGame()
    {
        if (gameStarted) return;
        gameStarted = true;

        UpdateUI();

    }
    private void Update()
    {
        if(lives <= 0 && !gameover)
        {
            gameover = true;
            GameOver();
        }
    }

    public void PlayerTakeDamage(int amt)
    {
        lives -= amt;
        UpdateUI();

        if (lives <= 0)
            Debug.Log("Game Over!"); // Replace with actual game over logic
    }

    public void AddMoney(int amt)
    {
        money += amt;
        UpdateUI();
    }

    public void SpendMoney(int amt)
    {
        money -= amt;
        UpdateUI();
    }

    void UpdateUI()
    {
        uiManager.UpdateLivesAndMoney(lives, money);
    }
    public void GameOver()
    {
        uiManager.StartGameOver();
    }
    public void UpdateStage(int stage)
    {
        uiManager.UpdateStage(stage);
    }

    public void RestartScene()
    {
        // Get current active scene
        Scene currentScene = SceneManager.GetActiveScene();
        // Reload it
        SceneManager.LoadScene(currentScene.name);
    }
}

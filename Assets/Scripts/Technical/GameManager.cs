using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int lives, money;

    [SerializeField] UIManager uiManager;
    public static GameManager Instance;
    bool gameOver;
    public GameState gameState;
    public float timeScale;

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
        gameOver = false;
        StartGame();
    }



    void StartGame()
    {
        gameState = GameState.Play;


        UpdateUI();

    }
    private void Update()
    {
        if(lives <= 0 && !gameOver)
        {
            gameOver = true;
            GameOver();
        }
        else if (!gameOver)
            Time.timeScale = timeScale; // For Debugging

    }

    public void Pause()
    {
        gameState = GameState.Pause;
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        gameState = GameState.Play;
        Time.timeScale = 1f;
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
        Time.timeScale = 0f;

        uiManager.StartGameOver();
    }
    public void UpdateStage(int stage)
    {
        uiManager.UpdateStage(stage);
    }

    public void RestartScene()
    {
        EventSystem.current.SetSelectedGameObject(null);
        StartCoroutine(ReloadNextFrame());
    }

    private IEnumerator ReloadNextFrame()
    {
        yield return new WaitForEndOfFrame();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void AutoStartToggle()
    {
        autoStart = !autoStart;
    }

}

public  enum GameState { Play, Pause, BetweenStages }
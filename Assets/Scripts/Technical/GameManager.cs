using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int lives, money;
    [SerializeField] TextMeshProUGUI healthText, goldText;
    [SerializeField] UIManager uiManager;
    public static GameManager Instance;
    bool gameover = false;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        EnemyDatabase db = EnemyDatabase.Instance;
    }
    void Start()
    {
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
        healthText.text = lives.ToString();
        goldText.text = money.ToString();
    }
    public void GameOver()
    {
        uiManager.StartGameOver();
    }

    public void RestartScene()
    {
        // Get current active scene
        Scene currentScene = SceneManager.GetActiveScene();
        // Reload it
        SceneManager.LoadScene(currentScene.name);
    }
}

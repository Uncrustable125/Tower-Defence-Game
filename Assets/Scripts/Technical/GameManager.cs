using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int lives = 20;
    public int money = 300;
    public TextMeshProUGUI healthText, goldText;

    public static GameManager Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {
        UpdateUI();
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
}

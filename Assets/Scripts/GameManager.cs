using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int lives = 20;
    public int money = 300;

    public void PlayerTakeDamage(int amt)
    {
        lives -= amt;
        if (lives <= 0)
            Debug.Log("Game Over!");
    }

    public void AddMoney(int amt)
    {
        money += amt;
    }
}

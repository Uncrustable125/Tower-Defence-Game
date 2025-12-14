using UnityEngine;

[CreateAssetMenu(fileName = "NewTower", menuName = "Tower Defense/Tower")]
public class TowerData : ScriptableObject
{
    [Header("Stats")]
    public float range = 5f;
    public float fireRate = 1f;
    public float damage = 10f;
    public float AOE = 0f;

    [Header("Cost & Visuals")]
    public int cost = 100;
    public Sprite topSprite, midSprite, botSprite, Lv2, Lv3, magicSprite;
    public Animation towerAnimation;


    public float GetDPS()
    {
        return damage * fireRate;
    }
}

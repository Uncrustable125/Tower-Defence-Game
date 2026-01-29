using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTower", menuName = "Tower Defense/Tower")]
public class TowerData : ScriptableObject
{
    public string towerName, description;
    public List<UpgradeData> upgrades;
    [Header("Stats")]
    public float range = 5f;
    public float fireRate = 1f;
    public float damage = 10f;
    public float projectileSpeed = 15f;
    public float projectileScale;
    //public int size

    [Header("Cost & Visuals")]
    public int cost = 10;
    public Sprite topSprite, midSprite2, midSprite, botSprite, Lv2, Lv3, magicSprite;
    public Sprite[] baseSprites;
    public Animation towerAnimation;

    [Header("AOE")]
    public bool hasAOE;
    public float aoeRadius;

    [Header("Debuffs")]
    public bool appliesSlow;
    [Range(0f, 1f)] public float slowMultiplier; // 0.5 = 50% speed
    public float slowDuration;

    [Header("Buff (Optional)")]
    public bool isBuffTower;
    public float buffRange;
    public float fireRateMultiplier; // e.g. 1.25 = +25%
    private void OnValidate()
    {
        List<Sprite> tempList = new List<Sprite>();
        if (botSprite != null) tempList.Add(botSprite);
        if (midSprite2 != null) tempList.Add(midSprite2);
        if (midSprite != null) tempList.Add(midSprite);
        if (topSprite != null) tempList.Add(topSprite);

        baseSprites = tempList.ToArray();
    }
    public float GetDPS()
    {
        return damage * fireRate;
    }
}

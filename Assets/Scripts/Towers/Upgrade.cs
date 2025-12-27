using System.Collections.Generic;
using UnityEngine;
public class Upgrade
{
    public string upgradeName, description;
    [Header("Stats")]
    public float range = 5f;
    public float fireRate = 1f;
    public float damage = 10f;
    public float projectileSpeed = 15f;
    public float projectileScale;

    [Header("Cost & Visuals")]
    public int cost = 10;
    public Sprite sprite;

    [Header("AOE")]
    public bool hasAOE;
    public float aoeRadius;

    [Header("Debuffs")]
    public bool appliesSlow;
    [Range(0f, 1f)] public float slowMultiplier;
    public float slowDuration;

    [Header("Buff (Optional)")]
    public bool isBuffTower;
    public float buffRange;
    public float fireRateMultiplier;

    public bool aquired, available;
    public List<UpgradeData> prerequisites;
    public Upgrade(UpgradeData uData)
    {
        if (uData == null)
        {
            Debug.LogError("UpgradeData is null!");
            return;
        }

        // Basic info
        upgradeName = uData.upgradeName;
        description = uData.description;

        // Stats
        range = uData.range;
        fireRate = uData.fireRate;
        damage = uData.damage;
        projectileSpeed = uData.projectileSpeed;
        projectileScale = uData.projectileScale;

        // Cost & visuals
        cost = uData.cost;
        sprite = uData.sprite;

        // AOE
        hasAOE = uData.hasAOE;
        aoeRadius = uData.aoeRadius;

        // Debuffs
        appliesSlow = uData.appliesSlow;
        slowMultiplier = uData.slowMultiplier;
        slowDuration = uData.slowDuration;

        // Buff (optional)
        isBuffTower = uData.isBuffTower;
        buffRange = uData.buffRange;
        fireRateMultiplier = uData.fireRateMultiplier;

        // Runtime state
        aquired = false;
        available = true;

        // Prerequisites
        prerequisites = new List<UpgradeData>();

        if (uData.prerequisites != null)
        {
            foreach (var pre in uData.prerequisites)
            {
                if (pre != null)
                    prerequisites.Add(pre); // use existing ScriptableObject
            }
        }

    }
}

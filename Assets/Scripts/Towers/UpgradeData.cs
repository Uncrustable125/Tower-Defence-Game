using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Tower Defense/Upgrade")]
public class UpgradeData : ScriptableObject
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
    public float aoeRadius;

    [Header("Debuffs")]
    [Range(0f, 1f)] public float slowMultiplier;
    public float slowDuration;

    [Header("Buff (Optional)")]
    public float buffRange;
    public float fireRateMultiplier;


    public OptionalBool hasAOE;
    public OptionalBool appliesSlow;
    public OptionalBool isBuffTower;


    public bool aquired, available;
    public List<UpgradeData> prerequisites;
    private void OnValidate()
    {

    }

}

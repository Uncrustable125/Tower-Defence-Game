using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class Tower : MonoBehaviour
{
    [Header("Stats")]
    public float range;
    public float fireRate;
    private float fireCooldown;
    public float damage;

    public bool hasAOE;
    public float aoeRadius;

    public bool appliesSlow;
    [Range(0f, 1f)] public float slowMultiplier;
    public float slowDuration;

    public bool isBuffTower;
    public float buffRange;
    public float fireRateMultiplier;
    public float projectileScale;

    private float baseFireRate;

    [Header("References")]
    public Transform firePoint;
    public GameObject projectilePrefab;
    public SpriteRenderer topSprite, midSprite, midSprite2, botSprite;

    [Header("Range Display")]
    [SerializeField] LineRenderer rangeRenderer;
    [SerializeField] int circleSegments = 64;

    [SerializeField] private TowerData towerData;
    public List<Upgrade> upgrades;

    private bool isSelected;
    private bool first = true;

    // =============================
    // Slow aura tracking
    // =============================
    private HashSet<Enemy> auraSlowedEnemies = new HashSet<Enemy>();

    // -----------------------------
    // Selection
    // -----------------------------
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (rangeRenderer != null)
            rangeRenderer.enabled = selected;
    }

    // -----------------------------
    // Initialization
    // -----------------------------
    public void Init(TowerData towerData)
    {
        this.towerData = towerData;

        range = towerData.range;
        baseFireRate = towerData.fireRate;
        fireRate = baseFireRate;
        damage = towerData.damage;

        hasAOE = towerData.hasAOE;
        aoeRadius = towerData.aoeRadius;

        appliesSlow = towerData.appliesSlow;
        slowMultiplier = towerData.slowMultiplier;
        slowDuration = towerData.slowDuration;

        isBuffTower = towerData.isBuffTower;
        buffRange = towerData.buffRange;
        fireRateMultiplier = towerData.fireRateMultiplier;
        projectileScale = towerData.projectileScale;

        upgrades = new List<Upgrade>();
        for (int i = 0; i < towerData.upgrades.Count; i++)
            upgrades.Add(new Upgrade(towerData.upgrades[i]));
        topSprite.sprite = towerData.topSprite;
        midSprite.sprite = towerData.midSprite;
        midSprite2.sprite = towerData.midSprite2;
        botSprite.sprite = towerData.botSprite;

        // Adjust positions if midSprite2 is missing
        if (midSprite2.sprite == null)
        {
            topSprite.transform.localPosition = midSprite2.transform.localPosition;
        }


        SetupRangeRenderer();
    }

    // -----------------------------
    // Update
    // -----------------------------
    void Update()
    {
        if (first)
        {
            SetupRangeRenderer();
            first = false;
        }

        if (isBuffTower)
        {
            ApplySlowAura();
            ApplyBuffAura();
            return;
        }

        Enemy target = GetNearestEnemy();
        if (target != null)
            TryShoot(target);
    }

    // =============================
    // Slow Aura Logic (Buff Towers)
    // =============================
    void ApplySlowAura()
    {
        if (!appliesSlow) return;

        Enemy[] enemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        // Remove enemies that left range
        foreach (Enemy e in new List<Enemy>(auraSlowedEnemies))
        {
            if (e == null || e.IsDead ||
                Vector3.Distance(transform.position, e.transform.position) > range)
            {
                e.RemoveSlow(this);
                auraSlowedEnemies.Remove(e);
            }
        }

        // Apply slow to enemies in range
        foreach (Enemy e in enemies)
        {
            if (e.IsDead) continue;

            float dist = Vector3.Distance(transform.position, e.transform.position);
            if (dist <= range && !auraSlowedEnemies.Contains(e))
            {
                e.SetSlow(this, slowMultiplier); // no duration
                auraSlowedEnemies.Add(e);
            }
        }
    }

    // =============================
    // Buff Aura (Fire Rate)
    // =============================
    void ApplyBuffAura()
    {
        Tower[] towers = Object.FindObjectsByType<Tower>(FindObjectsSortMode.None);

        foreach (Tower source in towers)
        {
            if (!source.isBuffTower) continue;

            foreach (Tower t in towers)
            {
                if (t == source) continue;

                float d = Vector3.Distance(source.transform.position, t.transform.position);
                if (d <= source.buffRange)
                    t.RecalculateFireRate(source.fireRateMultiplier);
            }
        }
    }

    // -----------------------------
    // Shooting
    // -----------------------------
    Enemy GetNearestEnemy()
    {
        Enemy[] enemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        Enemy nearest = null;
        float shortest = Mathf.Infinity;

        foreach (Enemy e in enemies)
        {
            float d = Vector3.Distance(transform.position, e.transform.position);
            if (d <= range && d < shortest)
            {
                shortest = d;
                nearest = e;
            }
        }

        return nearest;
    }

    void TryShoot(Enemy target)
    {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            Shoot(target);
            fireCooldown = 1f / fireRate;
        }
    }

    void Shoot(Enemy target)
    {
        GameObject bullet = Instantiate(
            projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );

        bullet.GetComponent<Projectile>().Init(target, this);
    }

    private void RecalculateFireRate(float multiplier)
    {
        fireRate = baseFireRate * multiplier;
    }

    // -----------------------------
    // Upgrades
    // -----------------------------
    public void ApplyUpgrade(Upgrade upgrade)
    {
        upgrade.aquired = true;
        upgrade.available = false;

        range += upgrade.range;
        fireRate += upgrade.fireRate;
        damage += upgrade.damage;
        aoeRadius += upgrade.aoeRadius;
        slowMultiplier += upgrade.slowMultiplier;
        fireRateMultiplier += upgrade.fireRateMultiplier;
        buffRange += upgrade.buffRange;

        if (upgrade.hasAOE.willOverrideBoolValue) hasAOE = upgrade.hasAOE.onForTrue;
        if (upgrade.appliesSlow.willOverrideBoolValue) appliesSlow = upgrade.appliesSlow.onForTrue;
        if (upgrade.isBuffTower.willOverrideBoolValue) isBuffTower = upgrade.isBuffTower.onForTrue;
    }

    // -----------------------------
    // Range Visualization
    // -----------------------------
    void SetupRangeRenderer()
    {
        if (rangeRenderer == null) return;

        rangeRenderer.positionCount = circleSegments + 1;
        rangeRenderer.loop = true;
        rangeRenderer.useWorldSpace = true;
        rangeRenderer.startWidth = 0.05f;
        rangeRenderer.endWidth = 0.05f;

        if (rangeRenderer.material == null)
            rangeRenderer.material = new Material(Shader.Find("Sprites/Default"));

        rangeRenderer.startColor = Color.green;
        rangeRenderer.endColor = Color.green;

        DrawRangeCircle();
        rangeRenderer.enabled = false;
    }

    void DrawRangeCircle()
    {
        float step = 360f / circleSegments;

        for (int i = 0; i <= circleSegments; i++)
        {
            float angle = Mathf.Deg2Rad * step * i;
            Vector3 pos = transform.position + new Vector3(
                Mathf.Cos(angle) * range,
                Mathf.Sin(angle) * range,
                0f
            );
            rangeRenderer.SetPosition(i, pos);
        }
    }
}

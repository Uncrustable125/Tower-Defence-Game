using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] public TowerData towerData;
    public List<Upgrade> upgrades;

    private bool isSelected;
    private bool first = true;

    private const float MAX_FIRERATE_BONUS = 2.0F; // 200% max BUFF 300% FR total

    [SerializeField] private int orderMultiplier = 100;
    private SpriteRenderer[] renderers;
    private LocalOrder[] orders;

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



    void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
        orders = GetComponentsInChildren<LocalOrder>();

        
    }

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
       
        SetupSprites();
        SetupRangeRenderer();
    }

    // -----------------------------
    // Sprites and Sorting Order
    // -----------------------------
    public void SetupSprites()
    {
        int baseOrder = Mathf.RoundToInt(-transform.position.y * orderMultiplier);
        int iterator = 0;
        foreach (var sr in renderers)
        {
            int localOffset = 0;
            LocalOrder lo = sr.GetComponent<LocalOrder>();
            if (lo != null)
            {
                lo.value = iterator;
                localOffset = lo.value;
            }
            sr.sortingOrder = baseOrder + localOffset;
            if (iterator < towerData.baseSprites.Length && towerData.baseSprites[iterator] != null)
            {
                sr.sprite = towerData.baseSprites[iterator];
            }

            //Not sure if I will need this or not
            /*else
            {
                topSprite.transform.localPosition = midSprite2.transform.localPosition;
            }*/
            iterator++;
        }
    }


    // -----------------------------
    // Update
    // -----------------------------
    void Update()
    {
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

            foreach (Tower recipient in towers)
            {
                if (recipient == source) continue;

                float d = Vector2.Distance(
                    source.transform.position,
                    recipient.transform.position
                );

                if (d <= source.buffRange)
                    recipient.SetBuff(source, 0.25f); // must be > refresh interval
            }
        }
    }


    private class ActiveBuff
    {
        public Tower source;
        public Coroutine coroutine;
    }

    private Dictionary<object, ActiveBuff> activeBuffs = new();

    // ---------------------------
    // Buff API (used by towers / projectiles)
    // ---------------------------

    public void SetBuff(Tower source, float duration)
    {
        if (activeBuffs.TryGetValue(source, out ActiveBuff existing))
        {
            if (existing.coroutine != null)
                StopCoroutine(existing.coroutine);

            existing.coroutine = StartCoroutine(RemoveBuffAfter(source, duration));
            RecalculateFireRate();
            return;
        }

        ActiveBuff buff = new ActiveBuff
        {
            source = source
        };

        activeBuffs[source] = buff;
        RecalculateFireRate();
        buff.coroutine = StartCoroutine(RemoveBuffAfter(source, duration));
    }

    public void RemoveBuff(object source)
    {
        if (!activeBuffs.TryGetValue(source, out ActiveBuff slow))
            return;

        if (slow.coroutine != null)
            StopCoroutine(slow.coroutine);

        activeBuffs.Remove(source);
        RecalculateFireRate();
    }

    private IEnumerator RemoveBuffAfter(object source, float duration)
    {
        yield return new WaitForSeconds(duration);
        RemoveBuff(source);
    }
    private void RecalculateFireRate()
    {
        float combined = 1f;

        foreach (var buff in activeBuffs.Values)
        {
            // Pull CURRENT value from the source tower
            combined *= (1f + buff.source.fireRateMultiplier);
        }

        float totalBonus = combined - 1f;
        totalBonus = Mathf.Min(totalBonus, MAX_FIRERATE_BONUS);

        fireRateMultiplier = 1f + totalBonus;
        fireRate = baseFireRate * fireRateMultiplier;
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
        DrawRangeCircle();
        ApplyBuffAura();
    }

    // -----------------------------
    // Range Visualization
    // -----------------------------
    void SetupRangeRenderer()
    {
        if (rangeRenderer == null) return;

        rangeRenderer.positionCount = circleSegments + 1;
        rangeRenderer.loop = true;
        rangeRenderer.useWorldSpace = false;
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
        if (!rangeRenderer) return;

        int points = circleSegments + 1;
        rangeRenderer.positionCount = points;

        float step = 2f * Mathf.PI / circleSegments;

        for (int i = 0; i < points; i++)
        {
            float angle = step * i;
            Vector3 pos = new Vector3(
                Mathf.Cos(angle) * range,
                Mathf.Sin(angle) * range,
                0f
            );

            rangeRenderer.SetPosition(i, pos);
        }
    }


}

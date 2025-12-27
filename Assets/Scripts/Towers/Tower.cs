using UnityEngine;
using System.Collections.Generic;


public class Tower : MonoBehaviour
{
    [Header("Stats")]
    public float range;
    public float fireRate;
    public float damage;
    private float fireCooldown;
    public bool hasAOE;
    public float aoeRadius;
    public bool appliesSlow;
    [Range(0f, 1f)] public float slowMultiplier; 
    public float slowDuration;
    public float projectileSpeed;
    public float projectileScale;
    bool isBuffTower;
    public float buffRange; 
    public float fireRateMultiplier;


    private float baseFireRate;
    private int buffCount;

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
    bool first = true;

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (rangeRenderer != null)
            rangeRenderer.enabled = selected;
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
        projectileSpeed = towerData.projectileSpeed;
        projectileScale = towerData.projectileScale;
        isBuffTower = towerData.isBuffTower;
        buffRange = towerData.buffRange;
        fireRateMultiplier = towerData.fireRateMultiplier;
        upgrades = new List<Upgrade>();
        for (int i = 0; i < towerData.upgrades.Count; i++)
        {
            Upgrade u = new Upgrade(towerData.upgrades[i]);
            upgrades.Add(u);
        }

        topSprite.sprite = towerData.topSprite;
        midSprite.sprite = towerData.midSprite;
        if (towerData.midSprite2 != null)
            midSprite2.sprite = towerData.midSprite2;
        else
        {
            //Will Update for taller towers
            midSprite2.sprite = null;
            topSprite.transform.localPosition = new Vector2(0, 0.646f);
        }
        botSprite.sprite = towerData.botSprite;
        SetupRangeRenderer();
        ApplyBuffAura();
    }

    void Update()
    {
        if (first)
        {
            SetupRangeRenderer();
            first = false;
        }
        if (isBuffTower)
        {

            return; // Buff towers do not shoot
        }
        Enemy target = GetNearestEnemy();
        if (target != null)
        {
           // RotateToward(target);
            TryShoot(target);
        }
    }

    void ApplyBuffAura()
    {

        Tower[] towers = FindObjectsByType<Tower>(FindObjectsSortMode.None);

        foreach (Tower x in towers)
        {
            if (!x.isBuffTower) continue;
            foreach (Tower t in towers)
            {
                if (t == x) continue;
                float d = Vector3.Distance(x.transform.position, t.transform.position);
                if (d <= x.buffRange)
                    t.RecalculateFireRate(x.fireRateMultiplier);
            }
        }
    }
    public void ApplyUpgrade(Upgrade upgrade)
    {
        for(int i=0; i<upgrades.Count; i++)
        {
            if (upgrades[i] == upgrade)
            {
                upgrades[i].aquired = true;
                upgrades[i].available = false;
            }
        }
    }
    private void RecalculateFireRate(float multiplier)
    {
        fireRate = baseFireRate * multiplier;//Mathf.Pow(multiplier, buffCount);
    }

    Enemy GetNearestEnemy()
    {
        Enemy[] enemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        Enemy nearest = null;
        float shortestDist = Mathf.Infinity;

        foreach (Enemy e in enemies)
        {
            float d = Vector3.Distance(transform.position, e.transform.position);
            if (d < shortestDist && d <= range)
            {
                shortestDist = d;
                nearest = e;
            }
        }

        return nearest;
    }

    void TryShoot(Enemy target)
    {
        //Extra checks for safety
        if (target == null) return;
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            if (target == null) return;
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

    // =============================
    // Range Visualization
    // =============================
    void SetupRangeRenderer()
    {
        if (rangeRenderer == null)
            return;

        rangeRenderer.positionCount = circleSegments + 1;
        rangeRenderer.loop = true;

        // Use world space for simplicity
        rangeRenderer.useWorldSpace = true;

        rangeRenderer.startWidth = 0.05f;
        rangeRenderer.endWidth = 0.05f;

        // Assign material if not assigned
        if (rangeRenderer.material == null)
            rangeRenderer.material = new Material(Shader.Find("Sprites/Default"));

        rangeRenderer.startColor = Color.green;
        rangeRenderer.endColor = Color.green;

        DrawRangeCircle();

        rangeRenderer.enabled = false; // will toggle via SetSelected
    }

    void DrawRangeCircle() //ON UPGRADE TO TOWER RE-CALL
    {
        float angleStep = 360f / circleSegments;

        for (int i = 0; i <= circleSegments; i++)
        {
            float angle = Mathf.Deg2Rad * angleStep * i;
            Vector3 pos = transform.position + new Vector3(
                Mathf.Cos(angle) * range,
                Mathf.Sin(angle) * range,
                0f
            );

            rangeRenderer.SetPosition(i, pos);
        }
    }



}

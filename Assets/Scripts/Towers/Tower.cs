using TMPro;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Stats")]
    public float range;
    public float fireRate;
    public float damage;
    bool first = true;
    [Header("References")]
    public Transform firePoint;
    public GameObject projectilePrefab;

    public SpriteRenderer topSprite, midSprite, botSprite;

    [Header("Range Display")]
    [SerializeField] LineRenderer rangeRenderer;
    [SerializeField] int circleSegments = 64;

    private TowerData towerData;
    private float fireCooldown;
    private bool isSelected;

    // =============================
    // Public method for selection
    // =============================
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (rangeRenderer != null)
            rangeRenderer.enabled = selected;
    }

    // =============================
    // Initialization
    // =============================
    public void Init(TowerData towerData)
    {
        this.towerData = towerData;

        range = towerData.range;
        fireRate = towerData.fireRate;
        damage = towerData.damage;
        topSprite.sprite = towerData.topSprite;
        midSprite.sprite = towerData.midSprite;
        botSprite.sprite = towerData.botSprite;
        SetupRangeRenderer();
    }

    // =============================
    // Targeting & Shooting
    // =============================
    void Update()
    {
        if (first)
        {
            SetupRangeRenderer();
            first = false;
        }
        Enemy target = GetNearestEnemy();
        if (target != null)
        {
           // RotateToward(target);
            TryShoot(target);
        }
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

   /* void RotateToward(Enemy target)
    {
        Vector3 dir = target.transform.position - turretHead.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        turretHead.rotation = Quaternion.Euler(0, 0, angle);
    }*/

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

        bullet.GetComponent<Projectile>().Init(target, damage);
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

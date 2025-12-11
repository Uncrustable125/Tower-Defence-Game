using UnityEngine;

public class Tower : MonoBehaviour
{
    public float range = 5f;
    public float fireRate = 1f;
    public float damage = 5f;
    public Transform turretHead;
    public Transform firePoint;
    public GameObject projectilePrefab;

    private float fireCooldown;

    void Update()
    {
        Enemy target = GetNearestEnemy();
        if (target == null) return;

        RotateToward(target);
        TryShoot(target);
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

    void RotateToward(Enemy target)
    {
        Vector3 dir = target.transform.position - turretHead.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        turretHead.rotation = Quaternion.Euler(0, 0, angle);
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
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Projectile>().Init(target, damage);
    }
}

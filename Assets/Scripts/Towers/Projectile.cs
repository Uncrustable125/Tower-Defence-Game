using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 25f;

    private Enemy target;
    private Transform targetTransform, lastTransform;
    private Enemy targetEnemy;

    private float damage;

    // AOE + Slow
    private float aoeRadius;
    private float slowMultiplier;
    private float slowDuration;
    private bool hasAOE, appliesSlow;
    GameObject temp;
    Tower tower;

    // New init for AOE / slow towers (ice tower)
    public void Init(
        Enemy target, Tower currentTowerData)
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        this.target = target;
        targetTransform = target.transform;
        targetEnemy = target;
        temp = new GameObject("TempTransform");
        lastTransform = temp.transform;
        this.damage = currentTowerData.damage;
        this.aoeRadius = currentTowerData.aoeRadius;
        this.slowMultiplier = currentTowerData.slowMultiplier;
        this.slowDuration = currentTowerData.slowDuration;
        this.hasAOE = currentTowerData.hasAOE;
        this.appliesSlow = currentTowerData.appliesSlow;
        transform.localScale = transform.localScale * currentTowerData.projectileScale;
        tower = currentTowerData;
    }

    void Update()
    {
        if (targetTransform == null || targetEnemy == null || targetEnemy.IsDead)
        {
            targetTransform = lastTransform;
          //  Destroy(gameObject);
           // return;
        }
        lastTransform.position = targetTransform.position;
        Vector3 dir = targetTransform.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    void HitTarget()
    {
        if (hasAOE)
        {
            ApplyAOE();
        }
        else
        {
            if(targetEnemy != null)
            ApplySingle(targetEnemy);
        }
        Destroy(temp);
        Destroy(gameObject);
    }

    void ApplySingle(Enemy enemy)
    {
        if (enemy == null || enemy.IsDead) return;

        enemy.TakeDamage(damage);

        if (appliesSlow)
            enemy.SetSlow(tower, slowMultiplier, slowDuration);
    }

    void ApplyAOE()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, aoeRadius);

        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy == null || enemy.IsDead) continue;

            enemy.TakeDamage(damage);

            if (appliesSlow)
                enemy.SetSlow(tower, slowMultiplier, slowDuration);
        }
    }
}

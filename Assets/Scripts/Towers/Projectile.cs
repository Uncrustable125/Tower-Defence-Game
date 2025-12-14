using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;

    private Enemy target;
    private float damage;
    private Transform targetTransform;   // Cached transform
    private Enemy targetEnemy;           // Cached Enemy component


    public void Init(Enemy target, float dmg)
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        targetTransform = target.transform;
        targetEnemy = target;
        damage = dmg;
    }

    void Update()
    {
        if (targetTransform == null || targetEnemy == null || targetEnemy.IsDead)
        {
            Destroy(gameObject);
            return;
        }

        // Move projectile toward cached transform
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
        if (targetEnemy != null && !targetEnemy.IsDead)
        {
            targetEnemy.TakeDamage(damage);
        }
        Destroy(gameObject);
    }


}



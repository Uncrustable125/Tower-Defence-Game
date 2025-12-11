using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;

    private Enemy target;
    private float damage;

    public void Init(Enemy t, float dmg)
    {
        target = t;
        damage = dmg;
        Destroy(gameObject, 3f); // auto cleanup
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.transform.position) < 0.1f)
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}

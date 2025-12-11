using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 3f;
    public float health = 20f;

    private float t;
    private int currentSegment;

    private SplinePath path;
    private GameManager gm;

    public void Init(SplinePath spline, GameManager gameManager)
    {
        path = spline;
        gm = gameManager;

        currentSegment = 0;
        t = 0;
        transform.position = path.points[0].position;
    }

    void Update()
    {
        MoveAlongSpline();
    }

    void MoveAlongSpline()
    {
        if(path != null)
        {
            if (currentSegment >= path.SegmentCount)
            {
                gm.PlayerTakeDamage(1);
                Destroy(gameObject);
                return;
            }
            float segmentLength = Vector3.Distance(
                path.GetPoint(currentSegment, 0f),
                path.GetPoint(currentSegment, 1f)
            );

            t += (speed * Time.deltaTime) / segmentLength;

            if (t >= 1f)
            {
                t = 0f;
                currentSegment++;
                return;
            }

            Vector3 pos = path.GetPoint(currentSegment, t);
            transform.position = pos;
        }

    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        if (health <= 0) Die();
    }

    void Die()
    {
        gm.AddMoney(10);
        Destroy(gameObject);
    }
}

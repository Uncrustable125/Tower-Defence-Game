using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 3f;
    public float health = 20f;

    private float t;
    private int currentSegment;

    private SplinePath path;
    private GameManager gm;

    public bool IsDead { get; private set; }

    private float baseSpeed;
    private float slowTimer;
    private float slowMultiplier = 1f;

    void Start()
    {
        baseSpeed = speed;
    }

    public void Init(SplinePath spline, GameManager gameManager)
    {
        path = spline;
        gm = gameManager;
        currentSegment = 0;
        t = 0;
        transform.position = path.points[0].position;
    }

    public void ApplySlow(float multiplier, float duration)
    {
        if (IsDead) return;

        slowMultiplier = Mathf.Min(slowMultiplier, multiplier);
        slowTimer = Mathf.Max(slowTimer, duration);
    }

    void Update()
    {
        if (IsDead) return;

        // Handle slow decay
        if (slowTimer > 0f)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0f)
                slowMultiplier = 1f;
        }

        MoveAlongSpline();
    }

    void MoveAlongSpline()
    {
        if (path == null) return;

        float currentSpeed = baseSpeed * slowMultiplier;

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

        t += (currentSpeed * Time.deltaTime) / segmentLength;

        if (t >= 1f)
        {
            t = 0f;
            currentSegment++;
            return;
        }

        transform.position = path.GetPoint(currentSegment, t);
    }

    public void TakeDamage(float dmg)
    {
        if (IsDead) return;

        health -= dmg;
        if (health <= 0)
            Die();
    }

    void Die()
    {
        if (IsDead) return;

        IsDead = true;
        GameManager.Instance.AddMoney(10);
        Destroy(gameObject);
    }
}

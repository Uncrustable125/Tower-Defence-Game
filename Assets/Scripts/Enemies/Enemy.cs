using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public int level;
    private float t;
    private int currentSegment;

    private SplinePath path;
    private GameManager gm;

    public bool IsDead { get; private set; }

    private float baseSpeed;
    private float slowMultiplier = 1f;


    private void OnEnable()
    {
        WaveManager.Instance.RegisterEnemy();
    }

    private void OnDestroy()
    {
        WaveManager.Instance.UnregisterEnemy();
    }


    // ===========================
    // Slow System
    // ===========================

    private const float MAX_SLOW = 0.8f; // 80% max slow

    EnemyData enemyData;
    private SpriteRenderer sr;

    // 5 animation frames → map to 3 sprites
    private static readonly int[] frameMap = { 0, 1, 2, 1, 0 };

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    void ScaleByLevel()
    {
        /*float healthMultiplier = 1f + (level - 1) * 0.25f;
        float speedMultiplier = 1f + (level - 1) * 0.05f;

        health *= healthMultiplier;
        speed *= speedMultiplier;*/
        health *= level;
        float speedMultiplier = 1f + (level - 1) * 0.25f;
        speed *= speedMultiplier;

    }

    // CALLED BY ANIMATION EVENTS
    public void SetFrame(int animFrame)
    {
        int spriteIndex = frameMap[animFrame];
        Sprite sprite = enemyData.GetSprite(spriteIndex, level);

        if (sprite != null)
            sr.sprite = sprite;
    }

    public void Init(EnemyData enemyData, SplinePath spline, GameManager gameManager, int level)
    {
        this.enemyData = enemyData;
        this.level = level;
        this.speed = enemyData.speed;
        this.health = enemyData.health;
        baseSpeed = speed;
        path = spline;
        gm = gameManager;
        currentSegment = 0;
        t = 0;
        transform.position = path.points[0].position;
        ScaleByLevel();

    }

    private class ActiveSlow
    {
        public float multiplier;
        public Coroutine coroutine;
    }

    private Dictionary<object, ActiveSlow> activeSlows = new();


    // ---------------------------
    // Slow API (used by towers / projectiles)
    // ---------------------------

    public void SetSlow(object source, float multiplier, float? duration = null)
    {
        if (IsDead) return;

        // If this source already applied a slow, refresh duration only
        if (activeSlows.TryGetValue(source, out ActiveSlow existing))
        {
            if (existing.coroutine != null)
                StopCoroutine(existing.coroutine);

            if (duration.HasValue)
                existing.coroutine = StartCoroutine(RemoveSlowAfter(source, duration.Value));

            return;
        }

        // Add new slow
        ActiveSlow slow = new ActiveSlow
        {
            multiplier = multiplier
        };

        activeSlows[source] = slow;
        RecalculateSlow();

        if (duration.HasValue)
            slow.coroutine = StartCoroutine(RemoveSlowAfter(source, duration.Value));
    }

    public void RemoveSlow(object source)
    {
        if (!activeSlows.TryGetValue(source, out ActiveSlow slow))
            return;

        if (slow.coroutine != null)
            StopCoroutine(slow.coroutine);

        activeSlows.Remove(source);
        RecalculateSlow();
    }

    private IEnumerator RemoveSlowAfter(object source, float duration)
    {
        yield return new WaitForSeconds(duration);
        RemoveSlow(source);
    }

    private void RecalculateSlow()
    {
        float combined = 1f;

        foreach (var slow in activeSlows.Values)
        {
            combined *= (1f - slow.multiplier);
        }

        // Apply cap
        float appliedSlow = 1f - combined;
        appliedSlow = Mathf.Min(appliedSlow, MAX_SLOW);

        slowMultiplier = 1f - appliedSlow;
    }

    // ===========================
    // Movement
    // ===========================


    void Update()
    {
        if (IsDead) return;
        MoveAlongSpline();
    }

    void MoveAlongSpline()
    {
        if (path == null || currentSegment >= path.SegmentCount) return;

        float distanceToMove = speed * slowMultiplier * Time.deltaTime;

        while (distanceToMove > 0f && currentSegment < path.SegmentCount)
        {
            // Approximate the segment length
            float segmentLength = ApproximateSegmentLength(currentSegment);

            // Calculate how much 't' to advance to move the distance
            float deltaT = distanceToMove / segmentLength;
            t += deltaT;

            if (t >= 1f)
            {
                // Move leftover distance to next segment
                distanceToMove = (t - 1f) * segmentLength;
                t = 0f;
                currentSegment++;

                if (currentSegment >= path.SegmentCount)
                {
                    transform.position = path.GetPoint(path.SegmentCount - 1, 1f);
                    gm.PlayerTakeDamage(1);
                    Destroy(gameObject);
                    return;
                }
            }
            else
            {
                distanceToMove = 0f; // We've moved the full distance
            }

            transform.position = path.GetPoint(currentSegment, t);
        }
    }

    // Approximate the length of a segment (from t=0 to t=1)
    float ApproximateSegmentLength(int segment, int steps = 10)
    {
        float length = 0f;
        Vector3 prev = path.GetPoint(segment, 0f);

        for (int i = 1; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector3 next = path.GetPoint(segment, t);
            length += Vector3.Distance(prev, next);
            prev = next;
        }

        return length;
    }

    // ===========================
    // Damage
    // ===========================

    public void TakeDamage(float dmg)
    {
        if (IsDead) return;

        health -= dmg;
        if (health <= 0f)
            Die();
    }

    private void Die()
    {
        if (IsDead) return;

        IsDead = true;
        GameManager.Instance.AddMoney(10);
        Destroy(gameObject);
    }
}

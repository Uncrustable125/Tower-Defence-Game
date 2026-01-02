using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    private float slowMultiplier = 1f;

    // ===========================
    // Slow System
    // ===========================

    private const float MAX_SLOW = 0.8f; // 80% max slow

    private class ActiveSlow
    {
        public float multiplier;
        public Coroutine coroutine;
    }

    private Dictionary<object, ActiveSlow> activeSlows = new();

    void Start()
    {
        baseSpeed = speed;
    }

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
        if (IsDead) return;
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

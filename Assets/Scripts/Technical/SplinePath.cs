using UnityEngine;

public class SplinePath : MonoBehaviour
{
    [Header("Spline Points (at least 4)")]
    public Transform[] points;

    // Catmull-Rom interpolation between 4 points
    public Vector3 GetPoint(int segment, float t)
    {
        if (points == null || points.Length < 4)
        {
            Debug.LogError("SplinePath requires at least 4 points!");
            return Vector3.zero;
        }

        // Clamp segment to valid range
        segment = Mathf.Clamp(segment, 0, points.Length - 2);

        Transform p0 = points[Mathf.Clamp(segment - 1, 0, points.Length - 1)];
        Transform p1 = points[segment];
        Transform p2 = points[Mathf.Clamp(segment + 1, 0, points.Length - 1)];
        Transform p3 = points[Mathf.Clamp(segment + 2, 0, points.Length - 1)];

        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * p1.position) +
            (-p0.position + p2.position) * t +
            (2f * p0.position - 5f * p1.position + 4f * p2.position - p3.position) * t2 +
            (-p0.position + 3f * p1.position - 3f * p2.position + p3.position) * t3
        );
    }

    // Total number of segments the spline has
    public int SegmentCount
    {
        get
        {
            if (points == null || points.Length < 2)
                return 0;
            return points.Length - 1;
        }
    }

#if UNITY_EDITOR
    // Draw spline in Scene view for visualization
    private void OnDrawGizmos()
    {
        if (points == null || points.Length < 2) return;

        Gizmos.color = Color.cyan;

        int resolution = 20; // how many points per segment
        for (int i = 0; i < SegmentCount; i++)
        {
            Vector3 prev = GetPoint(i, 0f);
            for (int j = 1; j <= resolution; j++)
            {
                float t = j / (float)resolution;
                Vector3 next = GetPoint(i, t);
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
        }
    }
#endif
}

using UnityEngine;

public class IsoSort : MonoBehaviour
{
    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        sr.sortingOrder = -(int)(transform.position.y * 100);
    }
}

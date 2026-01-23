using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public float speed = 3f;
    public float health = 20f;
    public float damage = 1f;
    public Sprite[] enemySprites;

    public string enemyId;          
    public string displayName;      
    public EnemyType type;          

    public Sprite GetSprite(int frameIndex, int level)
    {
        int start = level * 3;
        int index = start + frameIndex;

        if (index < 0 || index >= enemySprites.Length)
            return null;

        return enemySprites[index];
    }
}
public enum EnemyType
    {
        Basic,
        Fast,
        Tank,
        Flying,
        Boss
    }




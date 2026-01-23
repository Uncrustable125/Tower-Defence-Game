using System.Collections.Generic;
using UnityEngine;

public class EnemyDatabase
{
    private static EnemyDatabase _instance;
    public static EnemyDatabase Instance
    {
        get
        {
            if (_instance == null)
                _instance = new EnemyDatabase();
            return _instance;
        }
    }

    private List<EnemyData> allEnemies = new();
    private Dictionary<string, EnemyData> enemiesById = new();
    private Dictionary<EnemyType, List<EnemyData>> enemiesByType = new();

    private EnemyDatabase()
    {
        LoadEnemies();
    }

    private void LoadEnemies()
    {
        allEnemies.Clear();
        enemiesById.Clear();
        enemiesByType.Clear();

        EnemyData[] loaded = Resources.LoadAll<EnemyData>("EnemyData");

        foreach (var enemy in loaded)
        {
            // Store all
            allEnemies.Add(enemy);

            // By ID
            if (!string.IsNullOrEmpty(enemy.enemyId))
            {
                if (!enemiesById.ContainsKey(enemy.enemyId))
                    enemiesById.Add(enemy.enemyId, enemy);
                else
                    Debug.LogWarning($"Duplicate Enemy ID: {enemy.enemyId}");
            }

            // By Type
            if (!enemiesByType.ContainsKey(enemy.type))
                enemiesByType[enemy.type] = new List<EnemyData>();

            enemiesByType[enemy.type].Add(enemy);
        }

        Debug.Log($"[EnemyDatabase] Loaded {allEnemies.Count} enemies");
    }

    // -------------------------
    // Query API
    // -------------------------

    public EnemyData GetById(string id)
    {
        enemiesById.TryGetValue(id, out var enemy);
        return enemy;
    }

    public List<EnemyData> GetByType(EnemyType type)
    {
        if (enemiesByType.TryGetValue(type, out var list))
            return list;

        return new List<EnemyData>();
    }

    public EnemyData GetRandom()
    {
        if (allEnemies.Count == 0) return null;
        return allEnemies[Random.Range(0, allEnemies.Count)];
    }

    public EnemyData GetRandomByType(EnemyType type)
    {
        var list = GetByType(type);
        if (list.Count == 0) return null;

        return list[Random.Range(0, list.Count)];
    }
}

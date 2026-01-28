using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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

    private EnemyDatabase() { }

    // -------------------------
    // Async Load
    // -------------------------
    public AsyncOperationHandle<IList<EnemyData>> LoadEnemiesAsync(System.Action onComplete = null)
    {
        allEnemies.Clear();
        enemiesById.Clear();
        enemiesByType.Clear();

        // Assuming all EnemyData ScriptableObjects are in an Addressables group
        var handle = Addressables.LoadAssetsAsync<EnemyData>("EnemyData", enemy =>
        {
            // Called for each loaded EnemyData
            allEnemies.Add(enemy);

            if (!string.IsNullOrEmpty(enemy.enemyId))
            {
                if (!enemiesById.ContainsKey(enemy.enemyId))
                    enemiesById.Add(enemy.enemyId, enemy);
                else
                    Debug.LogWarning($"Duplicate Enemy ID: {enemy.enemyId}");
            }

            if (!enemiesByType.ContainsKey(enemy.type))
                enemiesByType[enemy.type] = new List<EnemyData>();

            enemiesByType[enemy.type].Add(enemy);
        });

        handle.Completed += _ =>
        {
            Debug.Log($"[EnemyDatabase] Loaded {allEnemies.Count} enemies");
            onComplete?.Invoke();
        };

        return handle;
    }

    // -------------------------
    // Query API
    // -------------------------
    public EnemyData GetById(string id)
    {
        enemiesById.TryGetValue(id, out var enemy);
        return enemy;
    }



    

    // -------------------------
    // Optional: Cleanup
    // -------------------------
    public void ReleaseEnemies()
    {
        Addressables.Release<IList<EnemyData>>(allEnemies);
        allEnemies.Clear();
        enemiesById.Clear();
        enemiesByType.Clear();
    }
}

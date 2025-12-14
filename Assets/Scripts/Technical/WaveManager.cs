using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [Header("Enemy & Path")]
    public GameObject enemyPrefab;
    public SplinePath splinePath;       // Spline path for enemies
    public GameManager gameManager;
    EnemyList enemyList;
    [Header("Wave Settings")]
    public float timeBetweenWaves = 5f;
    public float timeBetweenEnemies = 0.5f;

    private int waveIndex = 0;

    private void Start()
    {
        // Start spawning waves
        StartCoroutine(StartWaves());
    }

    IEnumerator StartWaves()
    {
        while (true) // You can change this to a finite number of waves if desired
        {
            waveIndex++;
            yield return StartCoroutine(SpawnWave(waveIndex));
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    IEnumerator SpawnWave(int count)
    {
        int enemiesToSpawn = count * 5; // Simple formula: waveIndex * 5
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(timeBetweenEnemies);
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null || splinePath == null || gameManager == null)
        {
            Debug.LogWarning("WaveManager: Missing references!");
            return;
        }

        GameObject e = Instantiate(enemyPrefab);
        Enemy enemyScript = e.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.Init(splinePath, gameManager);
        }
        else
        {
            Debug.LogWarning("Enemy prefab is missing the Enemy script!");
        }
    }
}

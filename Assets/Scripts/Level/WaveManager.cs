using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [Header("Enemy & Path")]
    public GameObject enemyPrefab;
    public SplinePath splinePath;     
    public GameManager gameManager;
    EnemyList enemyList;
    LevelData currentLevel;
    [Header("Wave Settings")]
    public float timeBetweenWaves = .5f;
    public float timeBetweenStages = 5f; //Will be replaced with a button
    public float timeBetweenEnemies = 0.5f;


    private async void Start()
    {
        await LevelDatabase.Instance.EnsureLoadedAsync();

        currentLevel = LevelDatabase.Instance.GetLevel(World.World0, 0);
        if (currentLevel == null)
        {
            Debug.LogError("Level not found!");
            return;
        }

        if (enemyPrefab == null || splinePath == null || gameManager == null)
        {
            Debug.LogWarning("WaveManager: Missing references");
            return;
        }

        StartCoroutine(StartStage());
    }

    IEnumerator StartStage()
    {
        foreach (var stage in currentLevel.Stages)
        {
            yield return StartCoroutine(StartWave(stage));
            yield return new WaitForSeconds(timeBetweenStages);
        }
    }

    IEnumerator StartWave(Stage stage)
    {
        foreach (var wave in stage.waves)
        {
            yield return StartCoroutine(StartSpawn(wave));
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    IEnumerator StartSpawn(Wave wave)
    {
        foreach (var spawn in wave.spawns)
        {
            yield return StartCoroutine(SpawnEnemy(spawn));
        }
    }



    IEnumerator SpawnEnemy(SpawnData spawn)
    {
        for (int i = 0; i < spawn.quantity; i++)
        {
            GameObject enemyObject = Instantiate(enemyPrefab);
            Enemy enemy = enemyObject.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.Init(spawn.enemyData, splinePath, gameManager, spawn.level);
            }
            else
            {
                Debug.LogWarning("Enemy prefab is missing Enemy component");
            }

            yield return new WaitForSeconds(timeBetweenEnemies);
        }
    }

}

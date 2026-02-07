using UnityEngine;
using System.Collections;
using System;

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
    public bool stageReady;
    public bool autoStart;
    bool stageFinished;
    bool allDead;
    private int aliveEnemies = 0;

    public static WaveManager Instance;
    public event Action OnStageFinished;

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterEnemy()
    {
        aliveEnemies++; 
        allDead = false;
    }

    public void UnregisterEnemy()
    {
        aliveEnemies--;

        if (aliveEnemies <= 0)
        {
            allDead = true;         
        }
    }



    private async void Start()
    {
        await LevelDatabase.Instance.EnsureLoadedAsync();
        stageReady = true;
        currentLevel = LevelDatabase.Instance.GetLevel(World.World0, 0); 
        StartCoroutine(StartLevel());
    }

     
    IEnumerator StartLevel()
    {
        int stageIndex = 0;
        foreach (var stage in currentLevel.Stages)
        {
            yield return new WaitUntil(() => stageReady);
            stageReady = false;
            allDead = false;
            GameManager.Instance.UpdateStage(stageIndex + 1);
            yield return StartCoroutine(StartStage(stage));
            Debug.Log($"Stage {stageIndex} complete");
            stageIndex++;
            stageReady = true; //Replace with User control and UI
        }
    }

    IEnumerator StartStage(Stage stage) //This is one stage
    {
        foreach (var wave in stage.waves)
        {
            yield return StartCoroutine(StartWave(wave));
            yield return new WaitUntil(() => allDead);
            yield return new WaitForSeconds(timeBetweenWaves);

                       
        }
    }

    IEnumerator StartWave(Wave wave)
    {
        int remaining = wave.spawns.Count;

        foreach (var spawn in wave.spawns)
        {
            StartCoroutine(SpawnEnemies(spawn, () =>
            {
                remaining--;
            }));
        }

        // Wait until ALL SpawnEnemies coroutines finish
        yield return new WaitUntil(() => remaining == 0);
    }



    IEnumerator SpawnEnemies(SpawnData spawn, Action onComplete)
    {
        for (int enemyIndex = 0; enemyIndex < spawn.quantity; enemyIndex++)
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
        onComplete?.Invoke();
    }

}

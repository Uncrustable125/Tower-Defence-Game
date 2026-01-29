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

        StartCoroutine(Stages());
    }



    //ADD USER CONTROL FOR NEXT STAGE
    IEnumerator Stages()
    {
        int stageIndex = 0;

        foreach (var stage in currentLevel.Stages)
        {
            GameManager.Instance.UpdateStage(stageIndex + 1);
            yield return StartCoroutine(Waves(stage, stageIndex));
            yield return new WaitForSeconds(timeBetweenStages);

           // Debug.Log($"Stage {stageIndex} complete");
            stageIndex++;
        }
    }

    IEnumerator Waves(Stage stage, int stageIndex)
    {
        int waveIndex = 0;

        foreach (var wave in stage.waves)
        {
            Spawns(wave, stageIndex, waveIndex);
            yield return new WaitForSeconds(timeBetweenWaves);

            //Debug.Log($"Stage {stageIndex} - Wave {waveIndex} complete");
            waveIndex++;
        }
    }

    void Spawns(Wave wave, int stageIndex, int waveIndex) //No coroutine I think
    {
        int spawnIndex = 0;

        foreach (var spawn in wave.spawns)
        {
            StartCoroutine(SpawnEnemy(spawn, stageIndex, waveIndex, spawnIndex));
            spawnIndex++;
        }
    }

    IEnumerator SpawnEnemy(SpawnData spawn, int stageIndex, int waveIndex, int spawnIndex)
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
            //enemies are spawning one Spawn at a time and I want them to come together
            //So will need multiple coroutines
            //Debug.Log(
          //      $"Stage {stageIndex}, Wave {waveIndex}, Spawn {spawnIndex}, Enemy {enemyIndex}"
          //  );

            yield return new WaitForSeconds(timeBetweenEnemies);
        }
    }

}
//Why are enemies so slow
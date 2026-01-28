using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LevelDatabase
{
    private static LevelDatabase _instance;
    public static LevelDatabase Instance => _instance ??= new LevelDatabase();

    private bool isLoaded = false;

    private readonly List<LevelData> allLevels = new();
    private readonly Dictionary<World, List<LevelData>> levelsByWorld = new();

    private LevelDatabase() { }

    // -------------------------
    // Load all LevelData assets asynchronously
    // -------------------------
    public async Task EnsureLoadedAsync()
    {
        if (isLoaded) return;

        allLevels.Clear();
        levelsByWorld.Clear();

        // Load ALL LevelData assets that are Addressable (no labels needed)
        await Addressables.LoadAssetsAsync<LevelData>(
            "Levels", // label, not folder path
            level =>
            {
                allLevels.Add(level);
                if (!levelsByWorld.ContainsKey(level.World))
                    levelsByWorld[level.World] = new List<LevelData>();
                levelsByWorld[level.World].Add(level);
            }).Task;


        isLoaded = true;
        Debug.Log($"[LevelDatabase] Loaded {allLevels.Count} levels");
    }

    // -------------------------
    // Query API
    // -------------------------
    public LevelData GetLevel(World world, int levelNumber)
    {
        if (!isLoaded)
        {
            Debug.LogError("[LevelDatabase] Levels not loaded yet!");
            return null;
        }

        if (!levelsByWorld.TryGetValue(world, out var list))
            return null;

        return list.Find(l => l.LevelNumber == levelNumber);
    }

    public List<LevelData> GetLevelsByWorld(World world)
    {
        if (!isLoaded) return new List<LevelData>();
        return levelsByWorld.TryGetValue(world, out var list) ? list : new List<LevelData>();
    }
}

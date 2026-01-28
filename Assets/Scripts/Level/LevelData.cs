using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Level/LevelData")]
public class LevelData : ScriptableObject
{
    [SerializeField] string levelName;
    [SerializeField] List<Stage> stages;
    [SerializeField] int levelNumber;
    [SerializeField] World world;

    public int LevelNumber => levelNumber;
    public string LevelName => levelName;
    public List<Stage> Stages => stages;
    public World World => world;
}
public enum World { World0, World1, World2 }
using System;
using System.Collections.Generic;

[Serializable]
public class Stage
{
    public List<Wave> waves;
}

[Serializable]
public class Wave
{
    public List<SpawnData> spawns;
}
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map/Monster Spawn Preset", fileName = "MonsterSpawnPreset")]
public class MonsterSpawnPreset : ScriptableObject
{
    [Serializable]
    public class SpawnEntry
    {
        public GameObject prefab;
        [Min(1)] public int count = 1;
    }

    public List<SpawnEntry> entries = new List<SpawnEntry>();

    public int GetTotalCount()
    {
        int total = 0;
        foreach (var e in entries)
        {
            if (e == null) continue;
            total += Mathf.Max(0, e.count);
        }
        return total;
    }
}

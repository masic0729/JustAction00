using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnEntry
{
    public GameObject enemyPrefab;
    [Min(1)] public int spawnCount = 1;
}

[CreateAssetMenu(menuName = "Map/Monster Spawn Preset", fileName = "MonsterSpawnPreset")]
public class MonsterSpawnPreset : ScriptableObject
{
    //이 엔트리의 데이터를 참조하여 몬스터 개수 및 NPC 유무를 확인하여 각 타일에 소환할 것
    public List<SpawnEntry> entries = new List<SpawnEntry>();

    public int GetTotalCount()
    {
        int total = 0;
        foreach (var e in entries)
        {
            if (e == null) continue;
            total += Mathf.Max(0, e.spawnCount);
        }
        return total;
    }
}

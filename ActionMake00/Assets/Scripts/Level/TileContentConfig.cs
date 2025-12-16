using System;
using UnityEngine;

public enum TileContentType
{
    None,
    Monster,
    NPC,
    MonsterAndNPC
}

[Serializable]
public class TileContentConfig
{
    public TileContentType contentType = TileContentType.None;

    // Monster
    public MonsterSpawnPreset monsterPreset;
    [Min(1)] public int monsterCountMultiplier = 1;

    // NPC (보통 1마리)
    public GameObject npcPrefab;
    [Min(1)] public int npcCount = 1; // 원하면 나중에 에디터에서 숨기고 1 고정하면 됨
}

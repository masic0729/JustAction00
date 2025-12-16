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

    // NPC (현재로선 종류는 하나다)
    public GameObject npcPrefab;
    [Min(0)] public int npcCount = 0;           
}

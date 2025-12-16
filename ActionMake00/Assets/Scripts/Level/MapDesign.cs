using System.Collections.Generic;
using UnityEngine;

public class MapDesign : MonoBehaviour
{
    [SerializeField, Min(2)] private int mapMakeCount = 7;

    // 가운데 타일만 편집: (mapMakeCount - 2)
    [SerializeField] private List<TileContentConfig> middleTileConfigs = new List<TileContentConfig>();

    public int GetMapMakeCount() => mapMakeCount;
    public IReadOnlyList<TileContentConfig> GetMiddleTiles() => middleTileConfigs;

    public void SyncMiddleTiles()
    {
        int middleCount = Mathf.Max(0, mapMakeCount - 2);

        while (middleTileConfigs.Count < middleCount)
            middleTileConfigs.Add(new TileContentConfig());

        while (middleTileConfigs.Count > middleCount)
            middleTileConfigs.RemoveAt(middleTileConfigs.Count - 1);
    }
}

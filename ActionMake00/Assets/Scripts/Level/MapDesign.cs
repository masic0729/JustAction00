using System.Collections.Generic;
using UnityEngine;

public class MapDesign : MonoBehaviour
{
    public static MapDesign instance;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField, Min(2)] private int mapMakeCount = 7;

    // 가운데 타일만 편집: (mapMakeCount - 2)
    [SerializeField] private List<TileContentConfig> middleTileConfigs = new List<TileContentConfig>();

    //타일 생성 횟수. 해당 값은 최소 2를 요구한다
    public int GetMapMakeCount() => mapMakeCount;

    //get TileContentData
    public IReadOnlyList<TileContentConfig> GetMiddleTiles() => middleTileConfigs;

    /// <summary>
    /// 생성하려는 맵과 편집중인 맵 데이터의 개수를 파악 후,
    /// 수치가 안맞을 시 이를 조정하는 함수
    /// </summary>
    public void SyncMiddleTiles()
    {
        int middleCount = Mathf.Max(0, mapMakeCount - 2);

        while (middleTileConfigs.Count < middleCount)
            middleTileConfigs.Add(new TileContentConfig());

        while (middleTileConfigs.Count > middleCount)
            middleTileConfigs.RemoveAt(middleTileConfigs.Count - 1);
    }
}

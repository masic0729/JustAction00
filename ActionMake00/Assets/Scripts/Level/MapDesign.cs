using System.Collections.Generic;
using UnityEngine;

public class MapDesign : MonoBehaviour
{
    public static MapDesign instance;

    private void Awake()
    {
        instance = this;
    }

    [Header("맵 생성 하려는 타일 개수")]
    [SerializeField, Min(2)] private int mapMakeCount = 7;

    [Header("맵의 컨텐츠를 설계한 중간 타일들의 정보")]
    [SerializeField] private List<TileContentConfig> middleTileConfigs = new List<TileContentConfig>();

    public int GetMapMakeCount() => mapMakeCount;

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

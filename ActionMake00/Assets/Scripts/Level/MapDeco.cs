using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDeco : MonoBehaviour
{
    [Header("수풀 프리팹 3종")]
    // 배치할 수풀 프리팹 배열 인덱스 0,1,2 순서로 희귀도 낮은것부터
    public GameObject[] bushPrefabs;

    [Header("배치 부모 오브젝트")]
    // Hierarchy의 Deco 오브젝트를 인스펙터에서 연결
    public Transform decoParent;

    [Header("맵 크기 설정")]
    // 맵의 절반 크기 기준으로 랜덤 위치 계산 (20x20 -> halfSize = 10)
    public float halfSize = 9.5f;

    [Header("수풀 생성 개수 범위")]
    // 최소 최대 생성 개수, 자연스러운 밀도 기준값
    public int minCount = 30;
    public int maxCount = 50;

    [Header("수풀 종류별 출현 가중치")]
    // 각 인덱스가 bushPrefabs 배열 인덱스와 대응
    // 예: {60, 30, 10} -> 0번 60%, 1번 30%, 2번 10%
    public int[] spawnWeights = { 60, 30, 10 };

    public void InitGrass()
    {
        // 프리팹 또는 부모 오브젝트가 없으면 실행하지 않음
        if (bushPrefabs == null || bushPrefabs.Length == 0)
        {
            Debug.LogWarning("MapDeco: bushPrefabs가 비어 있습니다.");
            return;
        }

        if (decoParent == null)
        {
            Debug.LogWarning("MapDeco: decoParent가 연결되지 않았습니다.");
            return;
        }

        // 가중치 합산 (확률 계산용 분모)
        int totalWeight = 0;
        foreach (int w in spawnWeights)
            totalWeight += w;

        // 생성 총 개수 결정
        int spawnCount = Random.Range(minCount, maxCount + 1);

        for (int i = 0; i < spawnCount; i++)
        {
            // 가중치 기반으로 수풀 종류 선택
            GameObject selectedPrefab = SelectPrefabByWeight(totalWeight);
            if (selectedPrefab == null) continue;

         
            // 수정 후: 타일 월드 위치 + 로컬 랜덤 오프셋
            Vector3 tileCenter = transform.position;   // 이 MapDeco가 붙은 타일의 월드 위치
            Vector3 spawnPos = new Vector3(
                tileCenter.x + Random.Range(-halfSize, halfSize),
                0.5f,
                tileCenter.z + Random.Range(-halfSize, halfSize)
            );

            // Y축만 랜덤 회전 (수풀은 위아래 뒤집힐 필요 없음)
            Quaternion spawnRot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            // 수풀 인스턴스 생성 후 Deco 하위에 배치
            GameObject bush = Instantiate(selectedPrefab);
            bush.transform.SetParent(decoParent);
            //bush.transform.SetParent(decoParent);
            bush.transform.position = spawnPos;
            bush.transform.rotation = spawnRot;
            //bush.transform.Rotate(0, 0, 60f);

            // 자연스러운 크기 변화: 0.8 ~ 1.4 범위에서 균일 스케일
            float scale = Random.Range(0.5f, 1.2f);

            // 10% 확률로 대형 수풀 (1.5 ~ 2.0) 생성해 포인트 강조
            if (Random.value < 0.1f)
                scale = Random.Range(0.8f, 1.5f);

            bush.transform.localScale = Vector3.one * scale;
        }

        Debug.Log($"MapDeco: 수풀 {spawnCount}개 배치 완료");
    }

    // 가중치 룰렛 방식으로 프리팹 인덱스 선택 후 반환
    // totalWeight: spawnWeights 합산값
    private GameObject SelectPrefabByWeight(int totalWeight)
    {
        int rand = Random.Range(0, totalWeight);
        int cumulative = 0;

        for (int i = 0; i < bushPrefabs.Length; i++)
        {
            // 배열 범위 초과 방지
            if (i >= spawnWeights.Length) break;

            cumulative += spawnWeights[i];

            // 랜덤값이 누적 가중치 이하에 들어오면 해당 프리팹 선택
            if (rand < cumulative)
                return bushPrefabs[i];
        }

        // 예외 상황 폴백: 0번 프리팹 반환
        return bushPrefabs[0];
    }
}
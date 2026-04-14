using System.Collections.Generic;
using UnityEngine;

// PatrolPos 오브젝트에 부착
// Pos_0 ~ Pos_N을 점유 테이블로 관리하며 중복 배정을 방지한다
public class PatrolPointManager : MonoBehaviour
{
    // 포인트별 점유 여부 테이블 Transform이 키 bool이 점유 상태
    private Dictionary<Transform, bool> occupiedTable = new Dictionary<Transform, bool>();
    [SerializeField] int currentCanPatrolPointCount = 0;

    void Awake()
    {
        // 자식 오브젝트 전체를 포인트로 등록하고 비점유 상태로 초기화
        foreach (Transform child in transform)
        {
            occupiedTable[child] = false;
        }
    }

/*    private void Update()
    {
        currentCanPatrolPointCount = 
    }*/

    // 비어있는 포인트를 랜덤 선택 후 예약하고 반환한다
    // 모든 포인트가 점유 중이면 null 반환
    public Transform ReserveRandomPoint()
    {
        List<Transform> available = new List<Transform>();

        foreach (var pair in occupiedTable)
        {
            if (!pair.Value)
                available.Add(pair.Key);
        }

        if (available.Count == 0)
        {
            Debug.LogWarning("PatrolPointManager: 사용 가능한 포인트가 없습니다.");
            return null;
        }

        Transform selected = available[Random.Range(0, available.Count)];
        occupiedTable[selected] = true;
        return selected;
    }

    // 몬스터 사망 또는 해제 시 해당 포인트를 빈 상태로 되돌린다
    public void ReleasePoint(Transform point)
    {
        if (point != null && occupiedTable.ContainsKey(point))
            occupiedTable[point] = false;
    }
}
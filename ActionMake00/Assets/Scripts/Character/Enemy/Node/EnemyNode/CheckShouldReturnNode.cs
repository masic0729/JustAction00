using UnityEngine;

// 몬스터가 복귀해야 하는지 판단하는 조건 노드
// 홈 위치 기준 이탈 거리 초과 또는 전투 중 플레이어 감지 범위 이탈 시 Success를 반환한다
public class CheckShouldReturnNode : Node
{
    // 플레이어 트랜스폼 참조
    Transform player;

    // 복귀 기준이 되는 홈 위치 패트롤 포인트 또는 스폰 위치
    Vector3 homePosition;

    // 홈 위치 기준 이 거리 이상 멀어지면 복귀 트리거
    float maxRoamDistance;

    public CheckShouldReturnNode(Transform player, Vector3 homePosition, float maxRoamDistance, Transform transform) : base(transform)
    {
        this.player = player;
        this.homePosition = homePosition;
        this.maxRoamDistance = maxRoamDistance;
    }

    public override NodeState Evaluate()
    {
        float distFromHome = Vector3.Distance(transform.position, homePosition);
        float distFromPlayer = Vector3.Distance(transform.position, player.position);

        // 추적 중 홈 위치에서 너무 멀어진 경우 복귀 트리거
        bool tooFarFromHome = distFromHome > maxRoamDistance;

        // 전투 중이었는데 플레이어가 감지 범위 밖으로 이탈한 경우 복귀 트리거
        // isPlayerFound가 true인 상태에서 감지 범위를 벗어나야 전투 종료로 간주한다
        bool lostPlayer = enemy.isPlayerFound && distFromPlayer > enemy.GetPlayerFindDistance();

        if (tooFarFromHome || lostPlayer)
        {
            enemy.isPlayerFound = false;
            return state = NodeState.Success;

        }

        return state = NodeState.Failure;
    }
}
using UnityEngine;
// 전투 중 몬스터가 홈 위치에서 허용 거리 이내인지 확인하는 조건 노드
// 이탈 시 Failure를 반환하여 전투 시퀀스를 중단하고 복귀 브랜치로 위임한다
public class CheckInRoamRangeNode : Node
{
    // 복귀 기준 홈 위치
    Vector3 homePosition;
    // 허용 최대 이탈 거리
    float maxRoamDistance;

    public CheckInRoamRangeNode(Vector3 homePosition, float maxRoamDistance, Transform transform) : base(transform)
    {
        this.homePosition = homePosition;
        this.maxRoamDistance = maxRoamDistance;
    }

    public override NodeState Evaluate()
    {
        float distFromHome = Vector3.Distance(transform.position, homePosition);

        // 홈 이탈 거리 초과 시 전투 중단 및 전투 상태 플래그 해제
        if (distFromHome > maxRoamDistance)
        {
            // isPlayerFound 해제하여 복귀 완료 후 패트롤로 자연스럽게 전환되게 한다
            enemy.isPlayerFound = false;
            return state = NodeState.Failure;
        }

        return state = NodeState.Success;
    }
}
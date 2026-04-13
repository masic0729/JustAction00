using UnityEngine;
// 몬스터가 홈 위치에서 허용 거리를 초과했는지 판단하는 조건 노드
// 초과 시 Success를 반환하여 복귀 시퀀스를 실행시킨다
public class CheckShouldReturnNode : Node
{
    // 복귀 기준 홈 위치
    Vector3 homePosition;
    // 홈 이탈 허용 최대 거리
    float maxRoamDistance;

    // player 참조 제거 lostPlayer 조건은 CheckInRoamRangeNode로 이전됨
    public CheckShouldReturnNode(Vector3 homePosition, float maxRoamDistance, Transform transform) : base(transform)
    {
        this.homePosition = homePosition;
        this.maxRoamDistance = maxRoamDistance;
    }

    public override NodeState Evaluate()
    {
        float distFromHome = Vector3.Distance(transform.position, homePosition);

        // 홈 이탈 거리 초과 시 복귀 트리거
        if (distFromHome > maxRoamDistance)
        {
            enemy.isPlayerFound = false;
            return state = NodeState.Success;
        }

        return state = NodeState.Failure;
    }
}
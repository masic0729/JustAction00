using UnityEngine;

public class EnemyReturnPositionNode : Node
{
    // BT 생성 시점에 확정된 복귀 목표 위치 패트롤 포인트 또는 스폰 위치
    Vector3 targetPosition;

    public EnemyReturnPositionNode(Vector3 targetTransform, Transform transform) : base(transform)
    {
        targetPosition = targetTransform;
    }

    public override NodeState Evaluate()
    {
        // 도착 판정과 이동 목표를 동일한 targetPosition으로 통일
        if (Vector3.Distance(targetPosition, enemy.transform.position) > 0.3f && enemy.GetIsAttack() == false)
        {
            anim.SetBool("Move", true);
            enemy.MoveTarget(targetPosition);   // GetSpawnPosition() 에서 targetPosition으로 변경
            return state = NodeState.Running;
        }
        else
        {
            // 강제 좌표 이동 제거 NavMesh가 자연스럽게 멈추게 둔다
            enemy.MoveTarget(transform.position);  // 제자리 정지
            anim.SetBool("Move", false);
            enemy.isDefault = true;
            enemy.isPlayerFound = false;
            return state = NodeState.Success;
        }
    }
}
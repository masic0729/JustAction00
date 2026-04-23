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
        // 복귀 진입 시 공격 상태를 강제 해제한다
        // 공격 도중 복귀가 결정됐을 때 isAttack이 남아있어 이동을 막는 문제를 방지한다
        if (enemy.GetIsAttack())
            enemy.SetIsAttack(false);

        if (Vector3.Distance(targetPosition, enemy.transform.position) > 0.3f)
        {
            anim.SetBool("Move", true);
            enemy.MoveTarget(targetPosition);
            return state = NodeState.Running;
        }
        else
        {
            enemy.MoveTarget(transform.position);
            anim.SetBool("Move", false);
            enemy.isDefault = false;
            enemy.isPlayerFound = false;
            return state = NodeState.Success;
        }
    }
}
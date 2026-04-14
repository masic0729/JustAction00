using UnityEngine;
public class GoToPlayerNode : Node
{
    Transform player;
    Vector3 homePosition;
    float maxRoamDistance;

    public GoToPlayerNode(Transform player, Transform transform, Vector3 homePosition, float maxRoamDistance) : base(transform)
    {
        this.player = player;
        this.homePosition = homePosition;
        this.maxRoamDistance = maxRoamDistance;
    }

    public override NodeState Evaluate()
    {
        // 홈 위치에서 너무 멀어지면 추격 중단 복귀 브랜치로 위임한다
        float distFromHome = Vector3.Distance(transform.position, homePosition);
        if (distFromHome > maxRoamDistance)
        {
            enemy.isPlayerFound = false;
            enemy.isDefault = true;
            anim.SetBool("Move", false);
            return state = NodeState.Failure;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        // 공격 범위 진입 시 공격 노드로 위임한다
        if (distance < enemy.GetAttackReadyDistance() &&
            enemy.GetIsAttack() == false &&
            enemy.GetHp() > 0)
        {
            anim.SetBool("Move", false);
            enemy.MoveTarget(transform.position);
            int rand = Random.Range(0, enemy.GetMaxAttackIndex());
            enemy.anim.SetInteger("PattenIndex", rand);
            return state = NodeState.Success;
        }

        enemy.MoveTarget(player.position);
        anim.SetBool("Move", true);
        return state = NodeState.Running;
    }
}
using UnityEngine;

public class GoToPlayerNode : Node
{
    Transform player;

    public GoToPlayerNode(Transform player, Transform transform) : base(transform)
    {
        this.player = player;

    }
    public override NodeState Evaluate()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        // isDefault == false 조건 제거 AI 오염 부분
        // 공격 준비 판단은 거리, 공격 여부, 체력만으로 충분하다
        if (distance < enemy.GetAttackReadyDistance() &&
            enemy.GetIsAttack() == false &&
            enemy.GetHp() >= 0)
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

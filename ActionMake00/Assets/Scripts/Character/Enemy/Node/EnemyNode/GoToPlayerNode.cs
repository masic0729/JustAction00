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

        if (distance < enemy.GetAttackReadyDistance() || enemy.isDefault == false &&
            enemy.GetIsAttack() == false || enemy.GetHp() <= 0)
        {
            anim.SetBool("Move", false);
            enemy.MoveTarget(this.transform.position);
            int rand = Random.Range(0, enemy.GetMaxAttackIndex());
            enemy.anim.SetInteger("PattenIndex", rand);
            return state = NodeState.Success;
        }

        enemy.MoveTarget(player.position);

        anim.SetBool("Move", true);
        return state = NodeState.Running;
    }
}

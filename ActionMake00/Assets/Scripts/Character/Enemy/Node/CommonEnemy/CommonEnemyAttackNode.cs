using UnityEngine;

public class CommonEnemyAttackNode : Node
{
    Transform player;


    public CommonEnemyAttackNode(Transform player, Transform transform) : base(transform)
    {
        this.player = player;
    }

    public override NodeState Evaluate()
    {
        if (enemy.isDefault == false)
            return state = NodeState.Success;
        if (enemy.isAttacked == false)
        {
            enemy.transform.LookAt(player);

            anim.SetTrigger("Attack");
            anim.SetBool("isAttacking", true);
            enemy.isAttacked = true;
            return state = NodeState.Running;
        }

        if (enemy.isAttacked == true && anim.GetBool("isAttacking") == true)
        {
            return state = NodeState.Running;

        }
        else
        {
            enemy.isAttacked = false;
            return state = NodeState.Success;
        }
    }
}

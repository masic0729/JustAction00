using UnityEngine;

public class StayNearPlayerNode : Node
{
    Transform player;

    public StayNearPlayerNode(Transform player, Transform transform) :base(transform)
    {
        this.player = player;
    }

    public override NodeState Evaluate()
    {
        if (enemy.isDefault == false)
            return state = NodeState.Success;

        if(enemy.isAttack == false)
        {
            anim.SetTrigger("Attack");
            anim.SetBool("isAttacking", true);
            enemy.isAttack = true;
            return state = NodeState.Running;
        }

        if (enemy.isAttack == true && anim.GetBool("isAttacking") == true)
        {
            return state = NodeState.Running;

        }
        else {
            enemy.isAttack = false;
            return state = NodeState.Success;
        }
    }

    
}

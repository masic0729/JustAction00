using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossThrowStone : EnemyAttackTree
{

    public BossThrowStone(Transform player, Transform transform) : base(player, transform)
    {
    }

    public override NodeState Evaluate()
    {
        if (enemy.isDefault == false)
            return state = NodeState.Success;

        if (enemy.isAttacked == false)
        {
            anim.SetTrigger("Cast");
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

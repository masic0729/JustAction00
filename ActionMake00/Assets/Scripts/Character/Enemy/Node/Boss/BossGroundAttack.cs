using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGroundAttack : EnemyAttackTree
{
    public BossGroundAttack(Transform player, Transform transform) : base(player, transform)
    {

    }


    public override NodeState Evaluate()
    {
        

        if (enemy.isDefault == false)
            return state = NodeState.Success;

        

        if (enemy.isAttack == false)
        {
            //텔레그래피를 생성한다
            transform.GetComponent<BossGolem>().SpawnTeleGuide();
            anim.SetTrigger("GroundAttack");
            anim.SetBool("isAttacking", true);
            enemy.isAttack = true;
            return state = NodeState.Running;
        }

        if (enemy.isAttack == true && anim.GetBool("isAttacking") == true)
        {
            return state = NodeState.Running;

        }
        else
        {
            enemy.isAttack = false;
            return state = NodeState.Success;
        }
    }
}

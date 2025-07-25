using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayNearPlayerNode : Node
{
    Transform player;
    bool isAttacked = false;
    public StayNearPlayerNode(Transform player, Transform transform) :base(transform)
    {
        this.player = player;
    }

    public override NodeState Evaluate()
    {
        
        if(isAttacked == false)
        {
            anim.SetTrigger("Attack");
            anim.SetBool("isAttacking", true);
            isAttacked = true;
            return state = NodeState.Running;
        }

        if (anim.GetBool("isAttacking") == false)
        {
            //공격이 끝나면 끝내기
            return state = NodeState.Success;
        }

        isAttacked = true;
        return state = NodeState.Running;

    }

    
}

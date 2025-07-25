using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayNearPlayerNode : Node
{

    public StayNearPlayerNode(Transform transform) :base(transform)
    {
    }

    public override NodeState Evaluate()
    {

        if (anim.GetBool("isAttacking") == true)
        {
            //공격 중이니까 러닝
            
            return state = NodeState.Running;
            
        }
        else 
        {
            
            anim.SetTrigger("Attack");
            anim.SetBool("isAttacking", true);
            //공격이 끝나면 끝내기
            return state = NodeState.Success;
        }

    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToPlayerNode : Node
{
    Transform player;
    Transform transform;

    public GoToPlayerNode(Transform player, Transform transform) : base(transform)
    {
        this.player = player;
        this.transform = transform;
    }
    public override NodeState Evaluate()
    {
        transform.LookAt(player);
        enemy.MoveForward();
        if (Vector3.Distance(transform.position, player.transform.position) < 2f || enemy.isDefault == false)
        {
            anim.SetBool("Move", false);
            //anim.SetBool("GoToPlayer", false);
            return state = NodeState.Success;
        }
        
        
        anim.SetBool("Move", true);
        return state = NodeState.Running;
    }
}

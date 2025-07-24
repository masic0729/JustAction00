using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayNearPlayerNode : Node
{
    Animator anim;

    public StayNearPlayerNode(Transform transform)
    {
        anim = transform.GetComponent<Animator>();
    }

    public override NodeState Evaluate()
    {
        Debug.Log("플레이어 근처에 대기중");
        return state = NodeState.Running;
    }
}

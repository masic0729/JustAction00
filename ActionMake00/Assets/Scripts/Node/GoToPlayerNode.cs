using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToPlayerNode : Node
{
    Transform player;
    Transform transform;
    Animator anim;

    public GoToPlayerNode(Transform player, Transform transform)
    {
        this.player = player;
        this.transform = transform;
        anim = transform.GetComponent<Animator>();
    }
    public override NodeState Evaluate()
    {
        transform.LookAt(player);
        transform.position = Vector3.Lerp(transform.position, player.position, Time.deltaTime);
        Debug.Log("플레이어 쫒아가는중");
        return state = NodeState.Running;
    }
}

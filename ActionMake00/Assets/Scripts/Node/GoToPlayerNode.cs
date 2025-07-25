using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToPlayerNode : Node
{
    Transform player;
    Transform transform;

    public GoToPlayerNode(Transform player, Transform transform)
    {
        this.player = player;
        this.transform = transform;
    }
    public override NodeState Evaluate()
    {
        transform.LookAt(player);
        Debug.Log("플레이어 쫒아가는중");
        if(Vector3.Distance(transform.position, player.transform.position) < 2f)
        {
            return state = NodeState.Success;
        }
        transform.position = Vector3.Lerp(transform.position, player.position, Time.deltaTime);

        return state = NodeState.Running;
    }
}

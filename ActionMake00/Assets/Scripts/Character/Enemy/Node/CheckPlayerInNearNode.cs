using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerInNearNode : Node
{
    int playerLayerMask = 1 << 6;
    Transform transform;

    /// <summary>
    /// enemy의 애니메이터를 노드의 transform 및 animator에 설정한다
    /// </summary>
    /// <param name="transform"></param>
    public CheckPlayerInNearNode(Transform transform)
    {
        this.transform = transform;
    }

    public override NodeState Evaluate()
    {
        Collider[] collider = Physics.OverlapSphere(transform.position, 4.0f, playerLayerMask);
        if(collider.Length <= 0)
        {
            //못찾았으니 실패
            return state = NodeState.Failure;
        }
        Debug.Log("플레이어가 근처에 있음");
        return state = NodeState.Success;
    }
}
    
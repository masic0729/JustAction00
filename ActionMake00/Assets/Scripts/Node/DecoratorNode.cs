using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoratorNode : Node
{
    protected Node child;

    public DecoratorNode(Node child)
    {
        this.child = child;
    }

    public override NodeState Evaluate()
    {
        NodeState result = child.Evaluate();
        if (result == NodeState.Success)
        {
            Debug.Log("单内饭捞磐 角菩贸府");
            return NodeState.Failure;
        }
        if (result == NodeState.Failure)
        {
            Debug.Log("单内饭捞磐 己傍 贸府");
            return NodeState.Success;
        }
        return NodeState.Running;
    }
}

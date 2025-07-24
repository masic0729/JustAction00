using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoratorNode : Node
{
    protected Node child;
    private Func<bool> condition;

    public DecoratorNode(Node child, Func<bool> condition)
    {
        this.child = child;
        this.condition = condition;
    }

    public override NodeState Evaluate()
    {
        if (!condition()) return NodeState.Failure;
        return child.Evaluate();
    }
}

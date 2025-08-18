using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SequenceNode : Node
{
    int currentChild = 0;
    public SequenceNode() : base() {
    }

    public SequenceNode(List<Node> children) : base(children) {
        

    }

    public override NodeState Evaluate()
    {
        foreach(Node node in childrenNode)
        {
            while(currentChild < childrenNode.Count)
            {
                NodeState result = childrenNode[currentChild].Evaluate();

                switch (result)
                {
                    case NodeState.Failure:
                        currentChild = 0;
                        return state = NodeState.Failure;

                    case NodeState.Success:
                        currentChild++;
                        continue;

                    case NodeState.Running:
                        return state = NodeState.Running;

                    default:
                        currentChild = 0;
                        return state = NodeState.Failure;
                }
            }
        }
        //모든 노드가 성공했으면 

        currentChild = 0;
        return state = NodeState.Success;
    }
}
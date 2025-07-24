using System.Collections.Generic;

public class SequenceNode : Node
{
    public SequenceNode() : base() { }

    public SequenceNode(List<Node> children) : base(children) { }

    public override NodeState Evaluate()
    {
        bool nodeRunning = false;
        foreach(Node node in childrenNode)
        {
            switch(node.Evaluate())
            {
                case NodeState.Failure:
                    
                    return state = NodeState.Failure;
                case NodeState.Success:
                    continue;
                case NodeState.Running:
                    nodeRunning = true;
                    continue;
                default:
                    continue;
            }
        }

        return state = nodeRunning ? NodeState.Running : NodeState.Success;
        
    }
}
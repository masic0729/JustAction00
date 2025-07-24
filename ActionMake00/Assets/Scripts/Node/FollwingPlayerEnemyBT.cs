using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollwingPlayerEnemyBT : Tree
{
    public Transform player;
    public Transform thisObject;



    protected override Node SetupBehaviorTree()
    {
        Node root = new SelecterNode(new List<Node>
        {
            new SequenceNode(new List<Node>
            {
                new DecoratorNode(new CheckPlayerInNearNode(thisObject)),
                new StayNearPlayerNode(thisObject)
            }),
            new GoToPlayerNode(player, thisObject)
        });
        return root;
    }
}

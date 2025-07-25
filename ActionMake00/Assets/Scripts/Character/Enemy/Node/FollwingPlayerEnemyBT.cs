using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class FollwingPlayerEnemyBT : Tree
{
    protected Node root;
    public Transform player;
    public Transform thisObject;

    protected override void Start()
    {
        base.Start();
        
    }

    protected override void Update()
    {
        base.Update();

    }


    protected override Node SetupBehaviorTree()
    {

        root = new SelecterNode(new List<Node>
        {
            new SequenceNode(new List<Node>
            { 
            new CheckPlayerInNearNode(thisObject),
            //여기에 공격을 하는데, 패턴1을 할 수도 있고, 2를 할 수도 있다
            new StayNearPlayerNode(player, thisObject)
            }),
            
            new GoToPlayerNode(player, thisObject)
        });
        return root;
    }

}

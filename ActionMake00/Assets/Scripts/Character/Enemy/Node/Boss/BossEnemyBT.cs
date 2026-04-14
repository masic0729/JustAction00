using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class BossEnemyBT : TreeCtrl
{
    private float playerDistance;
    protected float punchDistance;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Init()
    {
        base.Init();
    }

    public override void TakeDamage(float amount, Character attacker, int hitLevel = -1)
    {
        base.TakeDamage(amount, attacker, hitLevel);

    }

    bool CheckMelee()
    {
        playerDistance = Vector3.Distance(player.transform.position, this.gameObject.transform.position);

        if (playerDistance <= punchDistance)
            return true;
        else
            return false;
    }

    protected override Node SetupBehaviorTree()
    {
        node = new SequenceNode(new List<Node>
        {
            new CheckPlayerInNearNode(thisObject),

            new DecoratorNode(
                new SequenceNode(new List<Node>
                {
                    new GoToPlayerNode(player, thisObject, spawnPosition, 8f),

                    new SelecterNode(new List<Node>
                    {
                        new DecoratorNode(
                            new BossPunchAttack(player, thisObject), () => Random.Range(0,2) == 1? true : false
                            ),

                        new DecoratorNode(
                                new BossGroundAttack(player, thisObject), ()=> Random.Range(0, 2) == 1? true : false
                            ),

                        new BossThrowStone(player, thisObject)

                    })
                    
                } ), () => GetIsWasParried() == false),

            new DecoratorNode(new EnemyReturnPositionNode(spawnPosition, thisObject),
            () => !isDefault)
        });


        return node;
    }

}

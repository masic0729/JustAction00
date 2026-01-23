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
            //우선 플레이어가 올 때까지 대기한다
            new CheckPlayerInNearNode(thisObject),

            new DecoratorNode(
                new SequenceNode(new List<Node>
                {
                    new GoToPlayerNode(player, thisObject),

                    new SelecterNode(new List<Node>
                    {
                        //여기에 공격을 하는데, 패턴1을 할 수도 있고, 2를 할 수도 있다. 말이 Stay인거지, 현재는 공격이나 다름 없음
                        //또한 공격하면서 몬스터의 영역을 벗어나지 않는 선에서  
                        new DecoratorNode(
                            new BossPunchAttack(player, thisObject), () => Random.Range(0,2) == 1? true : false
                            ),
                        /*new DecoratorNode(
                            new BossPunchAttack(player, thisObject), () => CheckMelee()
                            ),*/
                        

                        //반반 확률로 바닥 공격 혹은 캐스팅 공격
                        new DecoratorNode(
                                new BossGroundAttack(player, thisObject), ()=> Random.Range(0, 2) == 1? true : false
                            ),

                        new BossThrowStone(player, thisObject)

                        /*new DecoratorNode(
                                new BossThrowStone(player, thisObject), ()=> Random.Range(0, 2) == 1? true : false
                            ),
                        new BossThrowStone(player, thisObject)*/

                    })
                    
                } ), () => GetIsWasParried() == false),

            //여기는 위 노드들이 영역체크를 한 후, 벗어나면 강제 복귀하는 기능을 넣으면 된다
            new DecoratorNode(new EnemyReturnPositionNode(spawnPosition, thisObject),
            () => !isDefault)
            

        });


        return node;
    }

}

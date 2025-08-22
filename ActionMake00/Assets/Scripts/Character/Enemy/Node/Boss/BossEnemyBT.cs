using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        root = new SequenceNode(new List<Node>
        {
            //우선 플레이어가 올 때까지 대기한다
            new CheckPlayerInNearNode(thisObject),


            new GoToPlayerNode(player, thisObject),
            new SelecterNode(new List<Node>
            {
                //여기에 공격을 하는데, 패턴1을 할 수도 있고, 2를 할 수도 있다. 말이 Stay인거지, 현재는 공격이나 다름 없음
                //또한 공격하면서 몬스터의 영역을 벗어나지 않는 선에서  
                new DecoratorNode(new BossPunchAttack(player, thisObject), () => CheckMelee()),
                new BossThrowStone(player, thisObject),
            }),


            //여기는 위 노드들이 영역체크를 한 후, 벗어나면 강제 복귀하는 기능을 넣으면 된다
            new DecoratorNode(new EnemyReturnPositionNode(spawnPosition, thisObject),
            () => !isDefault)
        });


        return root;
    }

}

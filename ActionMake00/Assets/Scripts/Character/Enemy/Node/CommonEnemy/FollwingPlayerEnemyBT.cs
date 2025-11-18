using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class FollwingPlayerEnemyBT : TreeCtrl
{
    

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

    /// <summary>
    /// 작업 목표
    /// 1. 몬스터는 일단 가만히 있는걸로
    /// 2. 몬스터 주위에 플레이어가 올 때까지 대기 or 배회를 한다
    /// 3. 만약 플레이어가 탐지 범위 안에 도달할 시 추적노드로 전환한다
    /// 4. 플레이어가 죽거나 본인의 영역을 벗어나면 강제로 복귀한다
    /// </summary>
    /// <returns></returns>
    protected override Node SetupBehaviorTree()
    {
        root = new SequenceNode(new List<Node>
        {
            //우선 플레이어가 올 때까지 대기한다
            new CheckPlayerInNearNode(thisObject),

            new GoToPlayerNode(player, thisObject),
            
            new SelecterNode(new List<Node>
            {
            //new GoToPlayerNode(mainCamera, thisObject),
            //여기에 공격을 하는데, 패턴1을 할 수도 있고, 2를 할 수도 있다. 말이 Stay인거지, 현재는 공격이나 다름 없음
            //또한 공격하면서 몬스터의 영역을 벗어나지 않는 선에서 
            new CommonEnemyAttackNode(player, thisObject)
            }),

            new DecoratorNode(new EnemyReturnPositionNode(spawnPosition, thisObject),
            () => !isDefault)
            //스크립팅은 추후 목표 위치로 이동하는 것으로 이름 변경 요규, 특정 오브젝트를 추격하거나, 본인 위치로 이동할 수 있기 때문
            
            
            //여기는 위 노드들이 영역체크를 한 후, 벗어나면 강제 복귀하는 기능을 넣으면 된다


        });


        return root;
    }

}

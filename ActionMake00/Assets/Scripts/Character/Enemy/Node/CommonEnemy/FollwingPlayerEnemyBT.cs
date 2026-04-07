//using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class FollwingPlayerEnemyBT : TreeCtrl
{
    
    public ItemObject[] dropItems;                                       //현재는 단일 게임오브젝트로 고정  소환하지만, 확률에 의해 다양한 아이템 및 여러 아이템 생성할 예정


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
        onDeathAction += DropItem;              //일반 몬스터만 아이템을 드랍한다
        rotateSpeed = 360f;
    }

    /// <summary>
    /// 몬스터 사망 시 아이템을 드랍한다
    /// 하지만 보스몬스터는 안넣을 지 고민중
    /// </summary>
    void DropItem(Character notUse)
    {
        if (dropItems == null || dropItems.Length == 0)
            return;

        int randResult = Random.Range(0, dropItems.Length);

        GameObject insItem = Instantiate(dropItems[randResult], transform.position, transform.rotation).gameObject;
        insItem.transform.Translate(0, 0.5f, 0);
    }

    protected override Node SetupBehaviorTree()
    {
        // 루트를 SelectorNode로 교체
        // 전투 시퀀스가 Failure면 패트롤 노드로 자동 분기된다
        node = new SelecterNode(new List<Node>
    {
        // 브랜치 1: 전투 시퀀스
        // CheckPlayerInNearNode가 Failure를 반환하면 이 시퀀스 전체가 Failure
        // → SelectorNode가 브랜치 2로 넘어간다
        new SequenceNode(new List<Node>
        {
            // 플레이어 감지 확인
            new CheckPlayerInNearNode(thisObject),
            // 플레이어 추적
            new GoToPlayerNode(player, thisObject),
            // 공격 패턴 선택
            new SelecterNode(new List<Node>
            {
                new CommonEnemyAttackNode(player, thisObject)
            }),
            // 영역 이탈 시 배정된 패트롤 포인트로 복귀
            new DecoratorNode(
                new EnemyReturnPositionNode(
                    assignedPatrolPos != null ? assignedPatrolPos.position : spawnPosition,
                    thisObject),
                () => !isDefault)
        }),

        // 브랜치 2: 패트롤 or 대기
        // 전투 시퀀스가 Failure일 때만 실행되며 항상 Running을 반환한다
        new EnemyPatrolNode(thisObject)
    });

        return node;
    }

}

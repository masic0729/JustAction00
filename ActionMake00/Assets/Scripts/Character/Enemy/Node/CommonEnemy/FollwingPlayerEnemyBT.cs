//using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class FollwingPlayerEnemyBT : TreeCtrl
{
    // 현재는 단일 게임오브젝트로 고정 소환하지만 확률에 의해 다양한 아이템 및 여러 아이템 생성할 예정
    public ItemObject[] dropItems;

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
        // 일반 몬스터만 아이템을 드랍한다
        onDeathAction += DropItem;
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
        // 홈 위치 패트롤 포인트 우선 없으면 스폰 위치
        Vector3 homePosition = assignedPatrolPos != null ? assignedPatrolPos.position : spawnPosition;

        // 홈 이탈 허용 최대 거리 이 이상 벗어나면 복귀 트리거
        float maxRoamDistance = 15f;

        node = new SelecterNode(new List<Node>
        {
            // 브랜치 1: 복귀 최우선
            // 홈 이탈 또는 전투 중 플레이어 감지 범위 이탈 시 즉시 복귀
            // 전투 브랜치보다 앞에 배치하여 전투 중에도 복귀 조건이 먼저 평가된다
            new SequenceNode(new List<Node>
            {
                // 복귀 여부 판단 홈 이탈 거리 또는 플레이어 감지 범위 기준
                new CheckShouldReturnNode(player, homePosition, maxRoamDistance, thisObject),
                // 홈 위치로 실제 이동 수행
                new EnemyReturnPositionNode(homePosition, thisObject)
            }),

            // 브랜치 2: 전투
            // 복귀 조건이 Failure일 때만 도달 플레이어 감지 시 추적 및 공격 수행
            new SequenceNode(new List<Node>
            {
                // 플레이어 감지 범위 확인 범위 밖이면 Failure로 패트롤 브랜치로 이동
                new CheckPlayerInNearNode(thisObject),
                // 플레이어 추적 공격 범위 진입까지 이동
                new GoToPlayerNode(player, thisObject),
                // 공격 패턴 선택
                new SelecterNode(new List<Node>
                {
                    new CommonEnemyAttackNode(player, thisObject)
                })
            }),

            // 브랜치 3: 패트롤 or 대기
            // 복귀 및 전투 조건 모두 Failure일 때 실행
            // 항상 Running을 반환하여 SelectorNode의 마지막 브랜치로 동작한다
            new EnemyPatrolNode(thisObject)
        });

        return node;
    }
}
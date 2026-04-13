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
        // 홈 이탈 허용 최대 거리
        float maxRoamDistance = 10f;

        node = new SelecterNode(new List<Node>
    {
        // 브랜치 1: 전투 최우선
        // 플레이어 감지 또는 피격 시 즉시 전투 진입
        // CheckInRoamRangeNode가 홈 이탈 감지 시 Failure로 복귀 브랜치에 위임한다
        new SequenceNode(new List<Node>
        {
            // 플레이어 감지 범위 확인 또는 isPlayerFound가 true이면 전투 진입
            new CheckPlayerInNearNode(thisObject),
            // 홈 이탈 거리 초과 시 전투 시퀀스 중단 복귀 브랜치로 위임
            new CheckInRoamRangeNode(homePosition, maxRoamDistance, thisObject),
            // 공격 범위 진입까지 추적
            new GoToPlayerNode(player, thisObject),
            // 공격 패턴 선택
            new SelecterNode(new List<Node>
            {
                new CommonEnemyAttackNode(player, thisObject)
            })
        }),

        // 브랜치 2: 복귀
        // 전투 시퀀스가 Failure일 때 도달
        // CheckShouldReturnNode는 tooFarFromHome만 판단한다
        // lostPlayer 조건은 CheckInRoamRangeNode로 이전했으므로 제거
        new SequenceNode(new List<Node>
        {
            new CheckShouldReturnNode(homePosition, maxRoamDistance, thisObject),
            new EnemyReturnPositionNode(homePosition, thisObject)
        }),

        // 브랜치 3: 패트롤 or 대기
        // 전투 및 복귀 조건 모두 Failure일 때 실행
        // 항상 Running을 반환한다
        new EnemyPatrolNode(thisObject)
    });

        return node;
    }
}
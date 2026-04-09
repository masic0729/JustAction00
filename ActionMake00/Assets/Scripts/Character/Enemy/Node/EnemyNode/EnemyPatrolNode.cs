using UnityEngine;

// 플레이어 미감지 상태에서 배정된 패트롤 포인트와 스폰 위치 사이를 왕복하며 대기한다
// 항상 Running을 반환하여 SelectorNode의 마지막 브랜치로 동작한다
public class EnemyPatrolNode : Node
{
    // 패트롤 내부 상태 구분
    enum PatrolState { Idle, Moving }
    PatrolState currentState = PatrolState.Idle;

    // 대기 타이머 및 목표 위치
    float idleTimer = 0f;
    float idleDuration = 0f;

    // 현재 이동 목표 벡터
    Vector3 patrolTarget;

    // 왕복 방향 플래그 true이면 패트롤 포인트로 이동 false이면 스폰 위치로 복귀
    bool goingToPatrolPoint = true;

    public EnemyPatrolNode(Transform transform) : base(transform)
    {
        // 초기 대기 시간을 랜덤 설정하여 모든 몬스터가 동시에 움직이지 않게 한다
        idleDuration = Random.Range(1f, 3f);
    }

    public override NodeState Evaluate()
    {
        Transform patrolPos = enemy.GetAssignedPatrolPos();

        switch (currentState)
        {
            case PatrolState.Idle:
                HandleIdle(patrolPos);
                break;

            case PatrolState.Moving:
                HandleMoving();
                break;
        }

        // 패트롤 노드는 항상 Running 전투 시퀀스가 성공하기 전까지 계속 유지된다
        return state = NodeState.Running;
    }

    // 제자리 대기 처리 타이머가 끝나면 다음 목표를 설정하고 이동 상태로 전환한다
    void HandleIdle(Transform patrolPos)
    {
        // 정지 상태 유지
        enemy.MoveTarget(transform.position);
        anim.SetBool("Move", false);

        idleTimer += Time.deltaTime;

        if (idleTimer < idleDuration)
            return;

        // 타이머 초과 시 방향 전환 후 이동 시작
        idleTimer = 0f;

        // 패트롤 포인트가 없으면 제자리 대기만 반복
        if (patrolPos == null)
        {
            idleDuration = Random.Range(2f, 4f);
            return;
        }

        // goingToPatrolPoint 방향에 따라 목표 위치 결정
        patrolTarget = goingToPatrolPoint ? patrolPos.position : enemy.GetSpawnPosition();
        goingToPatrolPoint = !goingToPatrolPoint;

        currentState = PatrolState.Moving;
        //enemy.isDefault = true;

    }

    // 목표 위치로 이동 처리 도착 시 Idle로 전환하고 다음 대기 시간을 설정한다
    void HandleMoving()
    {
        anim.SetBool("Move", true);
        enemy.MoveTarget(patrolTarget);

        // 도착 판정 거리 0.5f
        if (Vector3.Distance(transform.position, patrolTarget) > 0.5f)
            return;

        // 도착 후 대기 상태 전환
        anim.SetBool("Move", false);
        idleDuration = Random.Range(2f, 5f);
        idleTimer = 0f;
        currentState = PatrolState.Idle;
        //enemy.isDefault = false;
    }
}
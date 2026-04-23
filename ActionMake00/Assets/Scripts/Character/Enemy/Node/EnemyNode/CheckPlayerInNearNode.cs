using UnityEngine;

public class CheckPlayerInNearNode : Node
{
    int playerLayerMask = 1 << 6;

    enum PatrolState { Idle, Moving }
    PatrolState currentState = PatrolState.Idle;

    float idleTimer = 0f;
    float idleDuration = 0f;
    Vector3 patrolTarget;

    // 왕복 방향 플래그 true이면 패트롤 포인트로 false이면 스폰 위치로
    bool goingToPatrolPoint = true;

    public CheckPlayerInNearNode(Transform transform) : base(transform)
    {
        idleDuration = Random.Range(1f, 3f);
    }

    public override NodeState Evaluate()
    {
        // 활동 구역을 이탈해 복귀 중인 경우(isDefault == false) 플레이어 탐지를 완전히 무시한다
        // 복귀가 완료되어 isDefault가 true로 돌아올 때까지 이 블록에서 Failure를 반환한다
        // 탐지 무시 중에는 isPlayerFound도 함께 해제하여 복귀 완료 후 자연스럽게 패트롤로 전환되게 한다
        if (enemy.isDefault)
        {
            enemy.isPlayerFound = false;
            anim.SetBool("Move", false);
            return state = NodeState.Failure;
        }

        // 플레이어 탐지 범위 체크
        // 정지(Idle) 중이든 이동(Moving) 중이든 매 틱 동일하게 실행된다
        Collider[] collider = Physics.OverlapSphere(
            transform.position,
            enemy.GetPlayerFindDistance(),
            playerLayerMask);

        // 탐지 성공 또는 이미 전투 중인 경우 전투 브랜치로 진입한다
        if (collider.Length > 0 || enemy.isPlayerFound)
        {
            Debug.Log("플레이어 감지 전투 진입");
            enemy.isPlayerFound = true;
            anim.SetBool("Move", false);
            enemy.MoveTarget(transform.position);
            return state = NodeState.Success;
        }

        // 플레이어 미탐지 시 패트롤 로직을 실행한다
        HandlePatrol();
        return state = NodeState.Failure;
    }

    // 패트롤 상태 머신을 관리한다. Idle과 Moving 두 상태를 번갈아 전환한다
    void HandlePatrol()
    {
        switch (currentState)
        {
            case PatrolState.Idle:
                HandleIdle();
                break;
            case PatrolState.Moving:
                HandleMoving();
                break;
        }
    }

    // 대기 상태를 처리한다. idleDuration이 경과하면 다음 패트롤 목표를 설정하고 Moving으로 전환한다
    void HandleIdle()
    {
        enemy.MoveTarget(transform.position);
        anim.SetBool("Move", false);

        idleTimer += Time.deltaTime;
        if (idleTimer < idleDuration)
            return;

        idleTimer = 0f;

        Transform patrolPos = enemy.GetAssignedPatrolPos();
        if (patrolPos == null)
        {
            idleDuration = Random.Range(2f, 4f);
            return;
        }

        // 방향에 따라 목표 결정. 패트롤 포인트와 스폰 위치를 번갈아 이동한다
        patrolTarget = goingToPatrolPoint ? patrolPos.position : enemy.GetSpawnPosition();
        goingToPatrolPoint = !goingToPatrolPoint;
        currentState = PatrolState.Moving;
    }

    // 이동 상태를 처리한다. 목표 지점에 도착하면 Idle로 전환한다
    void HandleMoving()
    {
        anim.SetBool("Move", true);
        enemy.MoveTarget(patrolTarget);

        if (Vector3.Distance(transform.position, patrolTarget) > 0.5f)
            return;

        // 목표 지점 도착 후 대기 상태로 전환
        anim.SetBool("Move", false);
        idleDuration = Random.Range(2f, 5f);
        idleTimer = 0f;
        currentState = PatrolState.Idle;
    }
}
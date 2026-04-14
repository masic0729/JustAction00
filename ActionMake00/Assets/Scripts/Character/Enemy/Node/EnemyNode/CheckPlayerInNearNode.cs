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
        Collider[] collider = Physics.OverlapSphere(transform.position, enemy.GetPlayerFindDistance(), playerLayerMask);
        if (collider.Length > 0 || enemy.isPlayerFound == true)
        {
            Debug.Log("플레이어 감지 전투 진입");
            enemy.isPlayerFound = true;
            anim.SetBool("Move", false);
            enemy.MoveTarget(transform.position);
            return state = NodeState.Success;
        }

        HandlePatrol();
        return state = NodeState.Failure;
    }

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

        // 방향에 따라 목표 결정 패트롤 포인트와 스폰 위치를 번갈아 이동한다
        patrolTarget = goingToPatrolPoint ? patrolPos.position : enemy.GetSpawnPosition();
        goingToPatrolPoint = !goingToPatrolPoint;
        currentState = PatrolState.Moving;
    }

    void HandleMoving()
    {
        anim.SetBool("Move", true);
        enemy.MoveTarget(patrolTarget);

        if (Vector3.Distance(transform.position, patrolTarget) > 0.5f)
            return;

        // 도착 후 대기 상태로 전환
        anim.SetBool("Move", false);
        idleDuration = Random.Range(2f, 5f);
        idleTimer = 0f;
        currentState = PatrolState.Idle;
    }
}
using UnityEngine;
// 플레이어 미감지 상태에서 배정된 패트롤 포인트로 이동 후 제자리 대기한다
// 항상 Running을 반환하여 SelectorNode의 마지막 브랜치로 동작한다
public class EnemyPatrolNode : Node
{
    // 패트롤 내부 상태 구분
    enum PatrolState { Idle, Moving }
    PatrolState currentState = PatrolState.Idle;

    // 대기 타이머 및 대기 시간
    float idleTimer = 0f;
    float idleDuration = 0f;

    // 현재 이동 목표 벡터
    Vector3 patrolTarget;

    // 목표 포인트 도착 여부 한 번 도착하면 다시 이동하지 않는다
    bool arrived = false;

    public EnemyPatrolNode(Transform transform) : base(transform)
    {
        // 초기 대기 시간을 랜덤 설정하여 모든 몬스터가 동시에 움직이지 않게 한다
        idleDuration = Random.Range(1f, 3f);
    }

    public override NodeState Evaluate()
    {
        if (enemy.isPlayerFound == true)
            return state = NodeState.Failure;

        switch (currentState)
        {
            case PatrolState.Idle:
                HandleIdle();
                break;
            case PatrolState.Moving:
                HandleMoving();
                break;
        }

        // 패트롤 노드는 항상 Running 전투 시퀀스가 성공하기 전까지 계속 유지된다
        return state = NodeState.Running;
    }

    // 대기 처리 도착 전이면 타이머 후 이동 시작 도착 후면 제자리 대기만 유지한다
    void HandleIdle()
    {
        enemy.MoveTarget(transform.position);
        anim.SetBool("Move", false);

        // 이미 목표 포인트에 도착한 상태면 제자리 대기만 유지한다
        if (arrived)
            return;

        idleTimer += Time.deltaTime;
        if (idleTimer < idleDuration)
            return;

        idleTimer = 0f;

        Transform patrolPos = enemy.GetAssignedPatrolPos();

        // 패트롤 포인트가 없으면 제자리 대기만 반복한다
        if (patrolPos == null)
        {
            idleDuration = Random.Range(2f, 4f);
            return;
        }

        patrolTarget = patrolPos.position;
        currentState = PatrolState.Moving;
    }

    // 목표 포인트로 이동 도착 시 arrived를 true로 세팅하고 제자리 대기로 전환한다
    void HandleMoving()
    {
        anim.SetBool("Move", true);
        enemy.MoveTarget(patrolTarget);

        // 도착 판정 거리 0.5f
        if (Vector3.Distance(transform.position, patrolTarget) > 0.5f)
            return;

        // 도착 후 이동 완료 플래그 세팅 이후 제자리 대기만 유지한다
        anim.SetBool("Move", false);
        arrived = true;
        currentState = PatrolState.Idle;
    }
}
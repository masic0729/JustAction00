using UnityEngine;
using UnityEngine.AI;



public class Enemy : Character
{
    protected Node rootNode;                                    //몬스터 AI를 구동하기 위해 필요한 노드. 비헤이비어 트리이다
    protected Node node;                                        //몬스터 각각의 노드 단위
    public Transform thisObject;                                //표현 그대로 자신의 게임오브젝트를 담는다
    public Transform player;                                    //목표 타겟을 정하는 용도
    [SerializeField] protected AudioSource attackAudio;         //보스 각 패턴 사운드를 위한 오디오

    NavMeshAgent nav;

    
    protected Vector3 spawnPosition;                            //최초 생성 시 본인의 위치를 저장하는 용도

    protected Transform assignedPatrolPos;                   // 배정된 패트롤 포인트. 복귀 및 활동 범위 기준으로 사용한다
    private PatrolPointManager patrolManager;                  // 사망 시 포인트 해제를 위해 참조 보관


    public float activityAllowValue = 20f;                      //몬스터 활동 범위로 기본값은 5로 정의한다
    [SerializeField]
    protected float playerFindDistance = 10f;                   //플레이어가 본인 영역에 왔는 지 확인하는 범위
    [SerializeField]
    protected float attackReadyDistance = 1f;                   //플레이어를 추격 후 다음 행동을 하기 위한 요구 거리

    protected int playerLayerMask = 1 << 6;                     //플레이어를 적으로 삼을 때 판단하는 레이어 값

    private int enemyIndex = -1;                                //몬스터 스폰 시 사용했던 인덱스 값. 추후 삭제될 수 있음
    private const int maxAttackIndex = 2;                       //패턴형 공격에 사용할 가짓 수이며, 일반몬스터는 사용하지 않는다
    [SerializeField] int expAddValue;                           //플레이어에게 제공할 경험치 양

    public bool isPlayerFound = false;
    public bool isDefault = true;                               //몬스터가 본인 구역을 벗어날 때, 비헤이비어 트리 상에서 자동으로 복귀하기 위한 용도. 활성화 시 복귀한다
    public bool isAttack = false;                               //몬스터의 공격중인 지 확인하는 용도
    protected bool isCanTurn = false;                           //공격 시 타겟 대상으로 회전상태
    bool isWasParried = false;                                  //패링 공격에 당했는지 확인하는 변수. 일반몬스터는 사용하지 않으며, 보스 몬스터의 근접 공격에만 유효하다
    protected bool isAction = false;                                 //몬스터가 전투 중인지 확인하는 변수. 전투 해제 시 복귀한다

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        CheckCharacterActivityZone();
        RotateByAttack();
    }

    protected override void Init()
    {
        base.Init();
        transform.tag = "Enemy";
        player = GameObject.Find("Player").transform;
        spawnPosition = this.transform.position;
        thisObject = this.gameObject.transform;
        nav = GetComponent<NavMeshAgent>();
        pEffectDic["pDeath"] = pEffect[1];

        //onDeathAction += ExpUpForPlayer;
        onDeathAction += NodeDisable;
        onDeathAction += EnemyDeath;
    }

    /// <summary>
    /// 유저의 반격에 의한 경직 및 전투 시스템 초기화
    /// 초기화의 경우 무기 콜라이더 정리 및 상태 변수를 최신화하여
    /// 비에이비어 트리를 버그 없이 구동할 수 있게 한다
    /// </summary>
    public void GetParringAction()
    {
        isWasParried = true;
        //EnemyWeaponColDisable();
    }

    public void EnemyWeaponColDisable()
    {
        Debug.Log("무기 초기화 시작");
        for(int i = 0; i < weapon.Length; i++)
        {
            weapon[i].ResetColiderDisable();
        }
    }

    // CheckCharacterActivityZone 수정
    // 배정된 패트롤 포인트가 있으면 해당 위치 기준으로 활동 범위를 체크한다
    void CheckCharacterActivityZone()
    {
        Vector3 basePosition = assignedPatrolPos != null ? assignedPatrolPos.position : spawnPosition;
        float distanceFromBase = Vector3.Distance(basePosition, transform.position);
        if (activityAllowValue < distanceFromBase)
        {
            isDefault = false;
        }
    }

    // NodeDisable 수정: 사망 시 포인트 반납
    void NodeDisable(Character attacker)
    {
        rootNode = null;

        // 배정된 패트롤 포인트를 점유 해제하여 다른 몬스터가 사용할 수 있게 한다
        if (patrolManager != null && assignedPatrolPos != null)
            patrolManager.ReleasePoint(assignedPatrolPos);
    }

    /// <summary>

    /// </summary>
    void RotateByAttack()
    {
        if (isCanTurn == false || player == null || this.hp <= 0)
            return;

        // Update에서 회전
        Vector3 dir = (player.position - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(dir);

        // 보간으로 부드럽게 회전
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRot,
            rotateSpeed * Time.deltaTime
        );
    }
    void ExpUpForPlayer(Character attacker)
    {

        if (attacker.gameObject.transform.tag == "Player")
        {
            Player player = attacker.gameObject.GetComponent<Player>();
            player.ExpUp(expAddValue);
        }
    }

    /// <summary>
    /// 몬스터 사망 시 애니메이션 버그를 수정하기 위해 예외처리한 것
    /// </summary>
    void EnemyDeath(Character attacker)
    {
        anim.SetBool("isAlive", false);
    }

    public void MoveForward()
    {
        
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    

    /// <summary>
    /// 목표 위치로 이동한다. 플레이어든 복귀 위치든 특정 위치든
    /// </summary>
    /// <param name="target"></param>
    public void MoveTarget(Vector3 target)
    {
        if (this.hp <= 0)
            return;

        if (target == null)
        {
            nav.isStopped = true;                 // 이동 중지
            nav.ResetPath();                      // 안전하게 경로 초기화
            nav.SetDestination(transform.position);
            nav.velocity = Vector3.zero;
            Debug.Log("버그 또는 플레이어 사망 추정");
            return;                               
        }

        nav.isStopped = false;
        nav.SetDestination(target);
    }


    // 게터 세터 목록에 추가
    public void SetAssignedPatrolPos(Transform point, PatrolPointManager manager)
    {
        assignedPatrolPos = point;
        patrolManager = manager;   // 사망 시 해제용으로 매니저 보관
    }

    public Transform GetAssignedPatrolPos() => assignedPatrolPos;

    public void SetEnemyIndex(int value) => enemyIndex = value;

    public int GetEnemyIndex() => enemyIndex;

    public float GetPlayerFindDistance() => playerFindDistance;

    public float GetAttackReadyDistance() => attackReadyDistance;
    public void SetIsCanTurn(bool state) => isCanTurn = state;
    public bool GetIsCanTurn() => isCanTurn;

    public int GetMaxAttackIndex() => maxAttackIndex;

    public Vector3 GetSpawnPosition() => spawnPosition;

    public bool GetIsAttack() => isAttack;

    public void SetIsAttack(bool state) => isAttack = state;

    public bool GetIsWasParried() => isWasParried;

    public void SetIsWasParried(bool state) => isWasParried = state;


}

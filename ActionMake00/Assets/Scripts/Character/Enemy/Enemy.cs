using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;



public class Enemy : Character
{
    public ItemObject DropItems;                                       //현재는 단일 게임오브젝트로 고정  소환하지만, 확률에 의해 다양한 아이템 및 여러 아이템 생성할 예정

    protected Node root;
    public Transform thisObject;
    public Transform player;

    NavMeshAgent nav;

    protected int playerLayerMask = 1 << 6;
    
    protected Vector3 spawnPosition;

    public float activityAllowValue = 20f;                        //몬스터 활동 범위로 기본값은 5로 정의한다
    [SerializeField]
    protected float playerFindDistance = 10f;                     //플레이어가 본인 영역에 왔는 지 확인하는 범위
    [SerializeField]
    protected float attackReadyDistance = 1f;                       //플레이어를 추격 후 다음 행동을 하기 위한 요구 거리

    private int enemyIndex = -1;
    private const int maxAttackIndex = 2;
    //private int attackIndex = -1;
    public bool isPlayerFound = false;
    public bool isDefault = true;
    public bool isAttack = false;                                    //몬스터의 공격중인 지 확인하는 용도
    protected bool isCanTurn = false;
    bool isWasParried = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        CheckCharacterActivityZone();
        RotateByAttack();
    }

    protected override void Init()
    {
        base.Init();
        rotateSpeed = 7.5f;
        transform.tag = "Enemy";
        player = GameObject.Find("Player").transform;
        spawnPosition = this.transform.position;
        thisObject = this.gameObject.transform;
        nav = GetComponent<NavMeshAgent>();
        pEffectDic["pDeath"] = pEffect[1];

        deathAction += DropItem;
        deathAction += ExpUpForPlayer;
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

    void CheckCharacterActivityZone()
    {
        float distanceSpawnPosition = Vector3.Distance(spawnPosition, transform.position);
        if (activityAllowValue < distanceSpawnPosition)
        {
            isDefault = false;
        }
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
            player.ExpUp(currentExp);
        }
    }


    public void MoveForward()
    {
        
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 몬스터 사망 시 아이템을 드랍한다
    /// 하지만 보스몬스터는 안넣을 지 고민중
    /// </summary>
    void DropItem(Character notUse)
    {
        if (DropItems == null)
            return;
        Instantiate(DropItems, transform.position, transform.rotation);
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

    protected virtual void OnTriggerEnter(Collider other)
    {
        
    }

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

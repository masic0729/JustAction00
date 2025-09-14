using UnityEngine;
using UnityEngine.AI;



public class Enemy : Character
{
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
    public bool isAttacked = false;                                    //몬스터의 공격중인 지 확인하는 용도
    protected bool isCanTurn = false;


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
    /// todo : 연속 공격 시 회전을 자연스럽게 할 것
    /// 공격 이후 대기 또는 이동? 계열로 잠깐 틀고 공격하기에 애니메이션이 다소 부자연스러움
    /// </summary>
    void RotateByAttack()
    {
        if (isCanTurn == false || player == null)
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

    public void MoveForward()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }


    /// <summary>
    /// 목표 위치로 이동한다. 플레이어든 복귀 위치든 특정 위치든
    /// </summary>
    /// <param name="target"></param>
    public void MoveTarget(Transform target)
    {
        if (target == null)
        {
            nav.isStopped = true;                 // 이동 중지
            nav.ResetPath();                      // 안전하게 경로 초기화
            nav.SetDestination(transform.position);
            nav.velocity = Vector3.zero;
            return;                               
        }

        nav.isStopped = false;
        nav.SetDestination(target.position);
    }

    /*public override void Dead(float animationTime)
    {
        base.Dead(animationTime);
        
    }*/

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "PlayerAttack")
        {
            //기본적으로 피해를 받는다
            //TakeDamage(other.GetComponent<PlayerSword>().GetDamage());
        }
    }

    public void SetEnemyIndex(int value) => enemyIndex = value;

    public int GetEnemyIndex() => enemyIndex;

    public float GetPlayerFindDistance() => playerFindDistance;

    public float GetAttackReadyDistance() => attackReadyDistance;
    public void SetIsCanTurn(bool state) => isCanTurn = state;
    public bool GetIsCanTurn() => isCanTurn;

    public int GetMaxAttackIndex() => maxAttackIndex;
}

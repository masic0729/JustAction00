using UnityEngine;
public class Enemy : Character
{
    public Transform player;

    protected int playerLayerMask = 1 << 6;
    
    protected Vector3 spawnPosition;

    public float activityAllowValue = 5f;                        //몬스터 활동 범위로 기본값은 5로 정의한다
    //private int aiStateIndex = 0; // 0: 추적, 1: 대기
    private int enemyIndex = -1;
    public bool isPlayerFound = false;
    public bool isDefault = true;

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
    }

    protected override void Init()
    {
        base.Init();
        player = GameObject.Find("Player").transform;
        spawnPosition = this.transform.position;
    }

    void CheckCharacterActivityZone()
    {
        float distanceSpawnPosition = Vector3.Distance(spawnPosition, transform.position);
        if (activityAllowValue < distanceSpawnPosition)
        {
            isDefault = false;
        }
    }

    public void MoveForward()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    public override void Dead(float animationTime)
    {
        base.Dead(animationTime);
        ParticleManager.instance.PlayParticle(pEffectDic["pEnemyDeath0"], this.transform);
        SpawnManager.instance.DestroyCommonEnemy(this.gameObject.GetComponent<Enemy>());
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "PlayerAttack")
        {
            //기본적으로 피해를 받는다
            TakeDamage(other.GetComponent<Sword>().GetDamage());
        }
    }

    public void SetEnemyIndex(int value) => enemyIndex = value;

    public int GetEnemyIndex() => enemyIndex;
}

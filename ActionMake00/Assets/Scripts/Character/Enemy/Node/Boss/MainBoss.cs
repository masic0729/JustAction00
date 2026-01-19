using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBoss : BossEnemyBT
{
    GameObject pattenEffect;

    [SerializeField] GameObject GroundAttackGuide;                          //보통 해당 데이터를 확장하여 다양한 텔레그래피의 기능을 똑같이 수행할 수 있다. 현재는 하나만 할 것이므로 여기까지
    GameObject currentTeleObject;                                           //현재 시전 중인 텔레그래피 오브젝트

    Transform spawnProjectileTransform;                                     //발사체의 생성 위치


    protected override void Start()
    {
        base.Start();
        Init();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Init()
    {
        base.Init();
        for(int i = 0; i < weapon.Length; i++)
        {
            weaponDic[weapon[i].name] = weapon[i];
        }
        /*weaponDic["PunchWeapon"] = FindTransformAtChild("PunchWeapon").GetComponent<Weapon>();
        weaponDic["HookWeapon"] = FindTransformAtChild("HookWeapon").GetComponent<Weapon>();*/

        onDeathAction += BossDeath;
        onDeathAction += ShowGameOverPanel;

        playerFindDistance = 10f;
        activityAllowValue = 20f;
        attackReadyDistance = 3f;
        punchDistance = 2.0f;


    }

    public override void Dead(float animationTime)
    {
        base.Dead(animationTime);
        if(pattenEffect != null)
        {
            //보스가 사망할 때 바로 삭제해버릴 것
            pattenEffect.GetComponent<ParticlePoolReleaser>().PoolReleaser();
        }
    }


    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    public void CastWarning(int pattenIndex)
    {
        spawnProjectileTransform = FindTransformAtChild("CastShooter").GetComponent<Transform>();
        if(pattenIndex != 0)
        {
            pattenEffect = PoolManager.instance.Spawn("pCastWarning", spawnProjectileTransform.position, spawnProjectileTransform.rotation);
            pattenEffect.transform.parent = spawnProjectileTransform;
        }
        
    }

    public void SpawnStone(int pattenIndex)
    {
        GameObject instance;
        //instance = Instantiate(skillProjectiles[0], spawnProjectileTransform.position, spawnProjectileTransform.rotation);
        instance = PoolManager.instance.Spawn(skillProjectiles[0].name, spawnProjectileTransform.position, spawnProjectileTransform.rotation, this);

        //이펙트가 있으면 상대적으로 강한 패턴이므로 더 많은 발사체를 소환한다 
        if(pattenEffect != null)
        {
            Destroy(pattenEffect);
            pattenEffect = null;

            float rotateValue = 25f;
            instance = PoolManager.instance.Spawn(skillProjectiles[0].name, spawnProjectileTransform.position, spawnProjectileTransform.rotation, this);
            instance.transform.Rotate(0, rotateValue, 0);
            instance = PoolManager.instance.Spawn(skillProjectiles[0].name, spawnProjectileTransform.position, spawnProjectileTransform.rotation, this);
            instance.transform.Rotate(0, -rotateValue, 0);
        }

        spawnProjectileTransform = null;
    }



    /// <summary>
    /// 패턴에 맞는 텔레그래피를 생성한다
    /// </summary>
    public void SpawnTeleGuide()
    {
        currentTeleObject = Instantiate(GroundAttackGuide,transform.position, transform.rotation);
        currentTeleObject.GetComponent<GroundAttack>().SetOwner(this);
        currentTeleObject.transform.parent = this.transform;
    }

    public void ClearParent()
    {
        currentTeleObject.transform.parent = null;
    }

    /// <summary>
    /// 해당 텔레그래피 오브젝트 내 공격하는 기능을 사용한다
    /// </summary>
    public void AttackOfTele()
    {
        currentTeleObject.GetComponent<GroundAttack>().AttackTelePatten();
        currentTeleObject = null;
    }

    public void TurnOff()
    {
        isCanTurn = false;
    }


    /// <summary>
    /// 보스 사망 시 플레이어의 키 인풋및 관련 데이터들을 통제한다
    /// </summary>
    public void BossDeath(Character attacker)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        //게임 내 플레이어가 여러 명으로 개발 할 수 있으므로, 이를 고려하여 처리한다
        foreach(GameObject player in players)
        {
            player.GetComponent<Player>().GameEnd();
        }


    }

    /// <summary>
    /// 보스 사망 시 승리 화면이 노출되는 방식이다
    /// 해당 함수는 사망 애니메이션 시작 후 일정 시간이 지난 후에 실행된다.
    /// </summary>
    public void ShowGameOverPanel(Character attacker)
    {
        const float showTimer = 4f;
        GameManager.instance.Invoke("GameClear", showTimer);
    }
}

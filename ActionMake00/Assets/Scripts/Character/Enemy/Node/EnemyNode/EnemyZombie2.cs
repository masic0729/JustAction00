using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyZombie2 : FollwingPlayerEnemyBT
{
    [SerializeField]Transform spawnProjectileTransform;                                     //발사체의 생성 위치

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
        pEffectDic["CommonEnemyAttack"] = pEffect[0];
        playerFindDistance = 5f;
        activityAllowValue = 10f;
        attackReadyDistance = 5f;
    }



    /// <summary>
    /// 일반 몬스터의 일반 공격
    /// </summary>
    void Attack01()
    {
        PoolManager.instance.Spawn("EnemyProjectile", spawnProjectileTransform.position, spawnProjectileTransform.rotation, this);
    
    }

    public override void TakeDamage(float amount, Character attacker, int hitLevel = -1)
    {
        base.TakeDamage(amount, attacker, hitLevel);
        
    }


    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyZombie1 : FollwingPlayerEnemyBT
{

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
        attackReadyDistance = 1.2f;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStaffExplosion : PlayerSkillInfo
{
    AttackColManager attackManager;
    protected override void Start()
    {
        base.Start();
        Init();
    }
    

    protected override void Init()
    {
        base.Init();
        hitLevel = 0;
        attackManager = new AttackColManager();
        CheckEnemyHitBySphere();
    }

    void CheckEnemyHitBySphere()
    {
        Collider[] enemy = attackManager.CheckPlayerAttackAround(transform, 2.5f, enemyLayer);
        if(enemy != null)
        {
            for(int i = 0; i < enemy.Length; i++)
            {
                enemy[i].GetComponent<Character>().TakeDamage(damageMultify, owner);

            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGroundAttackProjectile : BossProjectile
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
        hitLevel = 1;
        attackManager = new AttackColManager();
        CheckEnemyHitBySphere();
    }

    void CheckEnemyHitBySphere()
    {
        Collider[] player = attackManager.CheckPlayerAttackAround(transform, 2.0f, playerLayer);
        if (player != null)
        {
            for (int i = 0; i < player.Length; i++)
            {
                player[i].GetComponent<Character>().TakeDamage(damage, owner, hitLevel);

            }
        }
    }
}

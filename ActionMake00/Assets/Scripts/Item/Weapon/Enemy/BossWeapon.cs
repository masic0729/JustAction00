using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWeapon : EnemyWeapon
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Init()
    {
        base.Init();
        hitLevel = 0;

        damageMultify = 3f;
    }

}

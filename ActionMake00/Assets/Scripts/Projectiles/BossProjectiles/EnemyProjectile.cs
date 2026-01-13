using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : Projectile
{
    protected override void Init()
    {
        base.Init();
        target = "Player";
        hitLevel = -1;

    }

    protected override void Update()
    {
        base.Update();

    }

}

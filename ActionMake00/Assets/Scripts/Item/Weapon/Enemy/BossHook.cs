using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHook : EnemyWeapon
{
    protected override void Start()
    {
        base.Start();
        Init();
    }
    protected override void Init()
    {
        base.Init();
        hitLevel = 1;
    }


}

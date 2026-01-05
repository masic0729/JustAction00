using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : Weapon
{
    protected override void Start()
    {
        base.Start();
    }
    protected override void Init()
    {
        base.Init();
        target = "Player";
        damageMultify = 1;
        tagName = "EnemyAttack";
    }


}

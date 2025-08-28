using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : Weapon
{
    protected override void Init()
    {
        base.Init();
        target = "Player";
        damage = 5;
    }

    
}

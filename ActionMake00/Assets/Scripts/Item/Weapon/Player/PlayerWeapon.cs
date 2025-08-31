using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : Weapon
{
    protected override void Start()
    {
        base.Start();
        //Init();
    }

    protected override void Init()
    {
        base.Init();
        tagName = "PlayerAttack";
        target = "Enemy";
        damage = 10;
        hitLevel = 0;
    }
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}

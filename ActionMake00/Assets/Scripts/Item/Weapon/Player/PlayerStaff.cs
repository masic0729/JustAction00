using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStaff : PlayerWeapon
{
    protected override void Start()
    {
        base.Start();
        Init();
    }

    protected override void Init()
    {
        base.Init();
        hitLevel = 0;
        weaponType = WeaponType.Staff;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}

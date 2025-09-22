using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStaff : PlayerWeapon
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Init()
    {
        base.Init();
        weaponType = WeaponType.Staff;
    }
}

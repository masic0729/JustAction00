using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Sword,
    Staff,
}

public class PlayerWeapon : Weapon
{
    public WeaponType weaponType;
    protected override void Start()
    {
        base.Start();
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

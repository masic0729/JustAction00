using UnityEngine;

public class PlayerSword : PlayerWeapon
{

    protected override void Start()
    {
        base.Start();
        Init();
    }


    protected override void Init()
    {
        base.Init();
        weaponType = WeaponType.Sword;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}
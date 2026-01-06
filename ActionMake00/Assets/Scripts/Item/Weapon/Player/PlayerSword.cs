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
        weaponTrail = GetComponentsInChildren<TrailRenderer>();

        InitWeaponTrail();
    }

    /// <summary>
    /// 게임 시작 시 트레일 모드를 비활성화한다
    /// </summary>
    void InitWeaponTrail()
    {
        for(int i = 0; i < weaponTrail.Length; i++)
        {
            weaponTrail[i].emitting = false;
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}
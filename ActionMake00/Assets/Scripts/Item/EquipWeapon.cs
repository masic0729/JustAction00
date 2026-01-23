using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipWeapon : EquipRoot
{
    public PlayerWeapon WeaponEquipment;


    public override void UseItem(Character character, SlotBase slot)
    {
        base.UseItem(character, slot);
        player.WeaponInit(WeaponEquipment);
    }

    public override string SetItemComment()
    {
        return "평범한 장비이다. 예외 발생!!";
    }
}

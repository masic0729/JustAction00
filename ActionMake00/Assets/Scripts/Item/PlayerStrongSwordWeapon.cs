using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class PlayerStrongSwordWeapon : EquipWeapon
{
    public override string SetItemComment()
    {
        return $"날카로운 한손검이다. 공격력 <color=#ff5555>{equipmentStat.Damage}</color> 상승한다";
    }
}

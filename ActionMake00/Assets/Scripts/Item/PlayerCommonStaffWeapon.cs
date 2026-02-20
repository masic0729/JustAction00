using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCommonStaffWeapon : EquipWeapon
{
    public override string SetItemComment()
    {
        return $"낡아 빠진 지팡이이다. 공격력 <color=#ff5555>{statData.Damage}</color> 상승한다";
    }


}

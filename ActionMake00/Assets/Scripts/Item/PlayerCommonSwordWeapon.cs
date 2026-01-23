using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCommonSwordWeapon : EquipWeapon
{
    public override string SetItemComment()
    {
        return $"평범한 한손검이다. 공격력 <color=#ff5555>{statData.Damage}</color> 상승한다";
    }
}

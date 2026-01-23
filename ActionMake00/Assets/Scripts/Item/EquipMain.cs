using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipMain : EquipDefs
{
    //기본설명을 적기 위한 부분
    [SerializeField] string baseComment;
    public override string SetItemComment()
    {
        return $"{baseComment} 체력 <color=#ff5555>{statData.MaxHp}</color>, 방어력 <color=#0000ff>{statData.Defense}</color> 상승";
    }
}

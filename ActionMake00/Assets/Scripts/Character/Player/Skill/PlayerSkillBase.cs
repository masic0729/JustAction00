using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSkillBase
{
    public Sprite icon;                         //스킬의 아이콘
    public string skillName;                    //스킬명

    public string description;                  //스킬 설명
    public int cooldown;                        //스킬 쿨
    public string triggerName;                  //애니메이션 및 함수 트리거명
}

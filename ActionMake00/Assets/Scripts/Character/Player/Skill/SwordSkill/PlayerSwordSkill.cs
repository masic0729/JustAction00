using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSwordSkill : PlayerSkillProcessor
{
    

    //protected Weapon weapon;

    private void Start()
    {
        //Init();
    }

    public override void InitSkill()
    {
        base.InitSkill();
        Debug.Log("무기 초기화 됨");

    }

}

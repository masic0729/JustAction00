using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillInfo : Skill
{
    protected override void Start()
    {
        base.Start();
    }

    

    protected override void Init()
    {
        base.Init();
        target = "Enemy";
    }


}

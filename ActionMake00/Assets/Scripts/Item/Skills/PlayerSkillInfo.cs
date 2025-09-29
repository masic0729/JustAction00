using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillInfo : Skill
{
    //콜라이더 기반이 아닌 특정 도형 기반 레이케이스 탐지할 때 사용한다
    protected int enemyLayer = 1 << 7;
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

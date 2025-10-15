using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordSlash : PlayerSkillInfo
{
    
    protected override void Start()
    {
        base.Start();
        Init();
    }
    

    protected override void Init()
    {
        base.Init();
        hitLevel = 1;
    }

    

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}

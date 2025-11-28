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
        BuffBase addDamageBuff = new BuffTransDamage(10f, "DamageUpEffect", null, BuffType.Buff, 6f);
        ownerBuffs.Add(addDamageBuff);

        BuffBase stunDeBuff = new BuffGetStun(2f, "StunEffect", null, BuffType.Buff);
        ownerBuffs.Add(stunDeBuff);
    }

    

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}

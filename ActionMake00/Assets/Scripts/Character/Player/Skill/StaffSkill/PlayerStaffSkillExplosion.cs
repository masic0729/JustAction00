using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStaffSkillExplosion : PlayerWeaponSkill
{
    protected override void Start()
    {
        base.Start();
        
    }

    public override void SkillUse()
    {
        base.SkillUse();
        GameObject instance = PoolManager.instance.Spawn(skillPrefab.name, player.transform.position, player);
        instance.transform.Rotate(0, player.transform.rotation.y, 0);
    }
}

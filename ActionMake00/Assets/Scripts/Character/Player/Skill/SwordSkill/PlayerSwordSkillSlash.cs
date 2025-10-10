using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordSkillSlash : PlayerWeaponSkill
{
    protected override void Start()
    {
        base.Start();
    }

    public override void SkillUse()
    { 
        base.SkillUse();
        PoolManager.instance.Spawn(skillPrefab.name, player.weaponDic["PlayerSword"].transform.position , player.weaponDic["PlayerSword"].transform.rotation, player);
    }

    
}

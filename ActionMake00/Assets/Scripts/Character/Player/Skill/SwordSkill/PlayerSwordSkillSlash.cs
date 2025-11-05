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
        GameObject ins = PoolManager.instance.Spawn(skillPrefab.name, player.weaponDic["Sword"].transform.position , player.weaponDic["PlayerSword"].transform.rotation, player);
        ins.transform.Rotate(0, 180, 0);
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordSkillParring : PlayerWeaponSkill
{
    protected override void Start()
    {
        base.Start();
    }

    public override void SkillUse()
    {
        base.SkillUse();
        GameObject instance = PoolManager.instance.Spawn(skillPrefab.name, player.transform.position, player.transform.rotation, player);
        instance.transform.Rotate(0, player.transform.rotation.y, 0);
        instance.transform.Translate(0, 0.5f, 0);

    }
}

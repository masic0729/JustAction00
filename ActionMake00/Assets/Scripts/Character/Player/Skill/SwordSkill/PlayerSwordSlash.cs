using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerSwordSlash : PlayerBaseSkill
{
    protected override void Start()
    {
        base.Start();
    }

    public override void SkillUse()
    {
        Instantiate(skillPrefab, player.weaponDic["PlayerWeapon"].transform.position, player.weaponDic["PlayerWeapon"].transform.rotation);
    }
}

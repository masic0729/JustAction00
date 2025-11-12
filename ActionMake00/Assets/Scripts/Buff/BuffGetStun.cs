using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffGetStun : BuffBase
{
    public override GameObject ObjectSetup(Character target, float duration, string spawnParticleName, string spawnParentName)
    {
        return base.ObjectSetup(target, duration, spawnParticleName, spawnParentName);
    }

    public override void ObjectSetup(Character target, float duration)
    {
        base.ObjectSetup(target, duration);
    }
    protected override void ApplyBuff()
    {
        character.anim.SetTrigger("GetStatus");
        character.anim.SetBool("isStating", true);
        character.anim.SetInteger("StatusIndex", 0);
    }

    protected override void UpdateBuff()
    {
        
    }

    protected override void ExitBuff()
    {
        character.anim.SetBool("isStating", false);

    }


}

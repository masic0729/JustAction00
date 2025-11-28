using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffGetStun : BuffBase
{
    /// <summary>
    /// 기절의 경우 상태이상 지속시간 자체가 버프 시간이므로 파라미터는 똑같다
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="spawnParticleName"></param>
    /// <param name="spawnParentName"></param>
    /// <param name="buffType"></param>
    public BuffGetStun(float duration, string spawnParticleName, string spawnParentName, BuffType buffType)
        : base(duration, spawnParticleName, spawnParentName, buffType)
    {
        iconPath = "Icons/Buffs/StunIcon";
    }

    public override GameObject ObjectSetup(Character target, Character buffCaster)
    {
        return base.ObjectSetup(target, buffCaster);
    }



    protected override void ApplyBuff()
    {
        buffCharacter.anim.SetTrigger("GetStatus");
        buffCharacter.anim.SetBool("isStating", true);
        buffCharacter.anim.SetInteger("StatusIndex", 0);
    }

    protected override void UpdateBuff()
    {
        
    }

    protected override void ExitBuff()
    {
        buffCharacter.anim.SetBool("isStating", false);

    }


}

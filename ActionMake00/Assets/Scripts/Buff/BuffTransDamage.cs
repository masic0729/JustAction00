using System;
using UnityEngine;

public class BuffTransDamage : BuffBase
{
    //데미지 변화값. 생성자를 통해 할당된다
    float damageAmount;

    public BuffTransDamage(float duration, string spawnParticleName, string spawnParentName, BuffType buffType, float damageValue)
        : base(duration, spawnParticleName, spawnParentName, buffType)
    {
        damageAmount = damageValue;
    }

    public override GameObject ObjectSetup(Character target, Character buffCaster)
    {
        return base.ObjectSetup(target, buffCaster);

    }

    
    protected override void ApplyBuff()
    {
        if (buffCharacter != null)
        {
            buffCharacter.statDatas[(int)AddStatName.Buff].Damage += damageAmount;
            
            Debug.Log("캐릭터 공벞 시작. 공격력 추가 계수는" + damageAmount + ", 현재 공격력 계수 : " + buffCharacter.GetResultDamage());
        }
        else
        {
            Debug.Log("버프 줄려 했는데 대상이 없음");
        }
    }

    protected override void UpdateBuff()
    {
        // 지속 중 특별히 할 일 없으니 비워둠
    }

    protected override void ExitBuff()
    {
        if (buffCharacter != null)
        {
            buffCharacter.statDatas[(int)AddStatName.Buff].Damage -= damageAmount;

            Debug.Log("캐릭터 공벞 종료. 공격력 삭제 계수는" + damageAmount + ", 현재 공격력 계수 : " + buffCharacter.GetResultDamage());

        }
    }
}

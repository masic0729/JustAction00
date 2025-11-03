using System;
using UnityEngine;

public class BuffTransDamage : BuffBase
{
    [SerializeField] float damageAmount;        // 에디터 셋업 혹은 런타임 주입

    public override void ObjectSetup(Character target, float dmgAmount, float duration)
    {
        base.ObjectSetup(target, dmgAmount, duration);
        damageAmount = dmgAmount;
    }

    protected override void ApplyBuff()
    {
        if (character != null)
        {
            character.statDatas[(int)AddStatName.Buff].Damage += damageAmount;
            
            Debug.Log("캐릭터 공벞 시작. 공격력 추가 계수는" + damageAmount + ", 현재 공격력 계수 : " + character.GetResultDamage());
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
        if (character != null)
        {
            character.statDatas[(int)AddStatName.Buff].Damage -= damageAmount;

            Debug.Log("캐릭터 공벞 종료. 공격력 삭제 계수는" + damageAmount + ", 현재 공격력 계수 : " + character.GetResultDamage());

        }
    }
}

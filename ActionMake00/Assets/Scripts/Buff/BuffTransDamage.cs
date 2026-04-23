using System;
using UnityEngine;

public class BuffTransDamage : BuffBase
{
    // 데미지 변화값. 생성자를 통해 할당된다
    float damageAmount;

    public BuffTransDamage(float duration, string spawnParticleName, string spawnParentName, BuffType buffType, float damageValue)
        : base(duration, spawnParticleName, spawnParentName, buffType)
    {
        damageAmount = damageValue;
        iconPath = "Icons/Buffs/DamageUpIcon";
    }

    public override GameObject ObjectSetup(Character target, Character buffCaster)
    {
        return base.ObjectSetup(target, buffCaster);
    }

    // 버프 적용 시 ModifierContainer의 Buff 소스 공격력 보정치를 증가시킨다
    // 기존: buffCharacter.statDatas[(int)AddStatName.Buff].Damage += damageAmount
    // 변경: ModifierContainer를 통해 Buff 소스의 보정치를 읽어 수정 후 재등록한다
    protected override void ApplyBuff()
    {
        if (buffCharacter == null)
        {
            Debug.Log("버프 줄려 했는데 대상이 없음");
            return;
        }

        // 현재 Buff 소스의 보정치를 가져와 Damage 수치를 더한 뒤 다시 등록한다
        // StatModifierData는 struct이므로 반드시 로컬에 받아서 수정 후 재설정해야 한다
        StatModifierData current = buffCharacter.GetModifierContainer().GetModifier(StatModifierSource.Buff);
        current.Damage += damageAmount;
        buffCharacter.GetModifierContainer().SetModifier(StatModifierSource.Buff, current);

        Debug.Log("캐릭터 공벞 시작. 공격력 추가 계수는" + damageAmount + ", 현재 공격력 계수 : " + buffCharacter.GetResultDamage());
    }

    // 지속 중 특별히 할 일 없으니 비워둠
    protected override void UpdateBuff()
    {

    }

    // 버프 종료 시 Buff 소스 공격력 보정치를 원래대로 되돌린다
    // 기존: buffCharacter.statDatas[(int)AddStatName.Buff].Damage -= damageAmount
    protected override void ExitBuff()
    {
        if (buffCharacter == null)
            return;

        // 현재 Buff 소스의 보정치를 가져와 Damage 수치를 빼고 다시 등록한다
        StatModifierData current = buffCharacter.GetModifierContainer().GetModifier(StatModifierSource.Buff);
        current.Damage -= damageAmount;
        buffCharacter.GetModifierContainer().SetModifier(StatModifierSource.Buff, current);

        Debug.Log("캐릭터 공벞 종료. 공격력 삭제 계수는" + damageAmount + ", 현재 공격력 계수 : " + buffCharacter.GetResultDamage());
    }
}

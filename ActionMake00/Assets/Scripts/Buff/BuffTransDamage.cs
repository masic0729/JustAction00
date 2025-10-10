using System;
using UnityEngine;

public class BuffTransDamage : BuffBase
{
    [SerializeField] int amount;        // 에디터 셋업 혹은 런타임 주입

    public void Setup(Character target, int dmgAmount, float duration)
    {
        character = target;
        amount = dmgAmount;
        Init(duration, ApplyBuff, UpdateBuff, ExitBuff);
    }

    protected override void ApplyBuff()
    {
        if (character == null) character = GetComponent<Character>();
        if (character != null)
        {
            character.AddStat.damage += amount;
            Debug.Log("캐릭터 버프시작 테스트");
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
            character.AddStat.damage -= amount;

            Debug.Log("캐릭터 버프종료 테스트");

        }
    }
}

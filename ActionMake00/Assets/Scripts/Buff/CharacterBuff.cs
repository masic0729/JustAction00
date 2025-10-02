using System.Collections.Generic;
using UnityEngine;

public enum BuffType
{
    Buff,
    Debuff
}

public class CharacterBuff : MonoBehaviour
{
    List<BuffBase> buffs = new List<BuffBase>();
    List<BuffBase> debuffs = new List<BuffBase>();                      //추후 확장성으로 추가 사용할 수 있으나, 스케쥴 상 될 지는 의문

    private void Start()
    {
        BuffTransDamage testBuff = GetComponent<BuffTransDamage>();
        testBuff.Setup(this.gameObject.transform.parent.GetComponent<Character>(), 10, 10f);
        AddBuff(testBuff, BuffType.Buff);
    }

    void Update()
    {
        for(int i = 0; i < buffs.Count; i++)
        {
            if(buffs[i].UpdateTime())
            {
                buffs.RemoveAt(i);
            }
            
        }
        for (int i = 0; i < debuffs.Count; i++)
        {
            if (debuffs[i].UpdateTime())
            {
                debuffs.RemoveAt(i);
            }
        }
    }

    public void AddBuff(BuffBase buff, BuffType buffType)
    {
        buff.onApply();

        if (buffType == BuffType.Buff)
        {
            buffs.Add(buff);
        }

        if(buffType == BuffType.Debuff)
        {
            debuffs.Add(buff);
        }
    }
    
    /// <summary>
    /// 기본적인 강제 버프 해제는 모든 버프를 제거하는(999개로 이론상 모든 버프 해제)
    /// 구조이나, 특정 행동이나 패턴에 의해 해제될 수 있다.
    /// 이에 따라 상황에 맞는 버프 해제를 사용할 수 있도록 구현함
    /// </summary>
    /// <param name="buffType"></param>
    /// <param name="removeCount"></param>
    public void RemoveBuff(BuffType buffType, int removeCount = 999)
    {
        int canRemoveCount;                 //본래 제거하려는 양이 현재 소지중인 버프 개수보다 적으면 이를 보정하기 위한 변수

        if (buffType == BuffType.Buff)
        {
            canRemoveCount = buffs.Count < removeCount ? buffs.Count : removeCount;

            for(int i = 0; i < canRemoveCount; i++)
            {
                buffs[i].Deactivate();
            }
            buffs.RemoveRange(0, canRemoveCount);
        }

        if (buffType == BuffType.Debuff)
        {
            canRemoveCount = debuffs.Count < removeCount ? debuffs.Count : removeCount;

            for (int i = 0; i < canRemoveCount; i++)
            {
                debuffs[i].Deactivate();
            }
            debuffs.RemoveRange(0, canRemoveCount);
        }

    }
}
using System.Collections.Generic;
using UnityEngine;



public class CharacterBuff : MonoBehaviour
{
    List<BuffBase> buffs = new List<BuffBase>();                 //캐릭터가 받는 이로운 버프
    List<BuffBase> debuffs = new List<BuffBase>();              //캐릭터가 받는 해로운 디버프

    [SerializeField] 

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

    /// <summary>
    /// 캐릭터에게 버프를 부여하는 곳
    /// 기본적으로 버프, 디버프로 분리가 되며
    /// 각 버프 스크롤 뷰에 해당 버프의 비주얼이 할당된다.
    /// 
    /// ---구현 목표---
    /// 하지만 중첩형 버프의 경우 같은 타입의 버프를 찾아낸 후
    /// 해당 버프의 시간만 갱신한다.
    /// 
    /// 중첩형 중에 스택형 버프는 기획안에 존재하지는 않다.
    /// 
    /// </summary>
    /// <param name="buff"></param>
    public bool AddBuff(BuffBase buff)
    {
        BuffBase updateBuffTarget = null;

        if (buff.buffType == BuffType.Buff)
        {
            updateBuffTarget = buffs.Find(data => data.GetType() == buff.GetType());
            if (updateBuffTarget != null)
            {
                updateBuffTarget.BuffUpdate();
                return false;
            }
            else
            {
                //디버프 창 내 중복된 타입의 버프가 없으면 추가하기
                buffs.Add(buff);
                buff.onApply?.Invoke();
                return true;

            }
        }

        if (buff.buffType == BuffType.Debuff)
        {
            updateBuffTarget = debuffs.Find(data => data.GetType() == buff.GetType());
            if(updateBuffTarget != null)
            {
                updateBuffTarget.BuffUpdate();
                return false;
            }
            else
            {
                //디버프 창 내 중복된 타입의 버프가 없으면 추가하기
                debuffs.Add(buff);
                buff.onApply?.Invoke();
                return true;
            }
        }

        Debug.Log("AddBuff예외 발생. 확인 요망");
        return false;
    }

    bool SameBuffType()
    {
        return false;
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
        int canRemoveCount;                                                                 //본래 제거하려는 양이 현재 소지중인 버프 개수보다 적으면 이를 보정하기 위한 변수

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
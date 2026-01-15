using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterBuff : MonoBehaviour
{
    //캐릭터의 능력치가 반영되는 버프들
    List<BuffBase> buffs = new List<BuffBase>();                
    List<BuffBase> debuffs = new List<BuffBase>();

    //화면에 보여주기 위한 버프들
    //버프 생성 및 삭제할 때 활용된다
    /*public List<Slider> buffSliders = new List<Slider>();       
    public List<Slider> deBuffSliders = new List<Slider>();*/

    //버프 슬라이더가존재해야하는 부모UI데이터
    [SerializeField] Slider buffSlider;
    [SerializeField] RectTransform buffParent;            
    [SerializeField] RectTransform deBuffParent;            


    void Update()
    {
        for(int i = 0; i < buffs.Count; i++)
        {
            if(buffs[i].UpdateTime())
            {
                //.RemoveAt(i);
            }
        }

        for (int i = 0; i < debuffs.Count; i++)
        {                                                                                                                          
            if (debuffs[i].UpdateTime())
            {
                //debuffs.RemoveAt(i);
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
    /// 중첩형 중에 스택형 버프는 기획안에 존재하지는 않으나
    /// 물론 확장 자체는 가능하다
    /// 
    /// </summary>
    /// <param name="buff">버프 리스트에 넣으려는 버프</param>
    public bool AddBuff(BuffBase buff)
    {
        BuffBase updateBuffTarget = null;
        RectTransform insBuff = null;

        List<BuffBase> targetBuffs = null;
        RectTransform targetBuffParent = null;

        if (buff.buffType == BuffType.Buff)
        {
            targetBuffs = buffs;
            targetBuffParent = buffParent;
        }
        else
        {
            targetBuffs = debuffs;
            targetBuffParent = deBuffParent;
        }

        updateBuffTarget = targetBuffs.Find(data => data.GetType() == buff.GetType());

        if (updateBuffTarget != null)
        {
            updateBuffTarget.BuffUpdate();
            return false;
        }
        else
        {
            insBuff = Instantiate(buffSlider).GetComponent<RectTransform>();
            insBuff.SetParent(targetBuffParent, false);
            
            insBuff.GetComponent<BuffStater>().InitBuffData(buff.GetIconPath(), buff.GetBuffTime());

            //버프 창 내 중복된 타입의 버프가 없으면 추가하기
            targetBuffs.Add(buff);
            buff.onApply?.Invoke();
            buff.characterBuff = this;
            buff.buffSlider = insBuff.GetComponent<Slider>();

            return true;
        }
    }

    

    public void RemoveBuffSlider(ref Slider buffSlider)
    {
        Destroy(buffSlider.gameObject);
        //Debug.Log("버프 삭제됨");
    }

    /// <summary>
    /// 기본적인 강제 버프 해제는 모든 버프를 제거하는(999개로 이론상 모든 버프 해제)
    /// 구조이나, 특정 행동이나 패턴에 의해 해제될 수 있다.
    /// 이에 따라 상황에 맞는 버프 해제를 사용할 수 있도록 구현함
    /// </summary>
    /// <param name="buffType"></param>
    /// <param name="removeCount"></param>
    public void CustomRemoveBuff(BuffType buffType, int removeCount = 999)
    {
        int canRemoveCount;                                                                 //본래 제거하려는 양이 현재 소지중인 버프 개수보다 적으면 이를 보정하기 위한 변수

        List<BuffBase> instanceBuffs = null;

        if (buffType == BuffType.Buff)
        {
            instanceBuffs = buffs;
            
        }
        else
        {
            instanceBuffs = debuffs;
        }

        canRemoveCount = instanceBuffs.Count < removeCount ? instanceBuffs.Count : removeCount;

        for (int i = 0; i < canRemoveCount; i++)
        {
            instanceBuffs[i].Deactivate();
        }
        instanceBuffs.RemoveRange(0, canRemoveCount);
    }

    /// <summary>
    /// 버프의 지속시간 만료 시 실행되는 함수
    /// 실행 시 지속시간 만료된 버프는 리스트에서 사라진다
    /// </summary>
    /// <param name="buff"></param>
    public void RemoveBuffByTimeOver(BuffBase buff)
    {
        List<BuffBase> instanceBuffs = null;
        if(buff.buffType == BuffType.Buff)
        {
            instanceBuffs = buffs;

        }
        else
        {
            instanceBuffs = debuffs;
        }

        instanceBuffs.Remove(buff);
    }
}
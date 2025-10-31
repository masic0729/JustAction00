using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
public class EqitTest : ItemObject
{
    float transMaxHpValue = 5f;

    /// <summary>
    /// 상호작용을 통해 해당 아이템에 설정된 기능들을 실행한다
    /// </summary>
    /// <param name="character">아이템이 실행될 때 적용되는 캐릭터 대상</param>
    /// <param name="slot">상호작용할 때 해당 아이템의 슬롯 정보</param>
    public override void UseItem(Character character, ItemSlot slot)
    {
        if (ItemUseCheck(character) == false || slot == null)
        {
            //원래 이곳에 사용되지 않았다는 메세지 및 사운드 구현해야함
            return;
        }
        //장비 스텟 적용

        //장비창 적용
        slot.GetInventory().equipManager.equipSlotDic[item.data.equipmentType.ToString()]
            .SwapItem(slot);


    }

    public override void UpdateInventory(ItemSlot slot)
    {
        slot.currentCount--;
        if (slot.currentCount == 0)
        {
            slot.ClearSlot();
        }
    }

    /// <summary>
    /// 이곳엔 전투 중인지 확인해야함
    /// 전투 중이라면 아이템 상호작용 불가
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    public override bool ItemUseCheck(Character character)
    {
        Debug.Log("장비 테스트");

        


        return true;
    }

}

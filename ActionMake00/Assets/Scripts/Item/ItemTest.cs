using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
public class ItemTest : ItemObject
{
    float transHpValue = 5f;

    protected override void Start()
    {
        //base.Start();
    }

    public override void UseItem(Character character, ItemSlot slot)
    {
        if (ItemUseCheck(character) == false)
        {
            //원래 이곳에 사용되지 않았다는 메세지 및 사운드 구현해야함
            return;
        }
        
    }

    public override void UpdateInventory(ItemSlot slot)
    {
        slot.currentCount--;
        if (slot.currentCount == 0)
        {
            slot.ClearSlot();
        }
    }

    public override bool ItemUseCheck(Character character)
    {
        Debug.Log("아이템 테스트");

        if (character.GetHp() >= character.GetMaxHp())
        {
            return false;
        }


        return true;
    }

}

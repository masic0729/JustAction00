using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTest : ItemObject
{
    float transHpValue = 5f;

    protected override void Start()
    {
        base.Start();
    }

    public override void UseItem(Character character)
    {
        if (ItemUseCheck(character) == false)
        {
            //원래 이곳에 사용되지 않았다는 메세지 및 사운드 구현해야함
            return;
        }
        Debug.Log("회복 전 : " + character.GetHp());
        //포션 역할 실행 및 인벤토리 최신화
        character.HpTransfer(transHpValue);
        Debug.Log("회복됨 : " + character.GetHp());
        //UpdateInventory()
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

        if (character.GetHp() >= character.GetMaxHp())
        {

            Debug.Log("사용 불가. 현재 최대 체력");
            return false;
        }


        return true;
    }

}

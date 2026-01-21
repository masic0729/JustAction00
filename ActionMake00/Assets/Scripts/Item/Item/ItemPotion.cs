using UnityEngine;

public class ItemPotion : ItemObject
{
    float transHpValue = 5f;

    public override void UseItem(Character character, SlotBase slot)
    {
        Debug.Log(item.comment);

        if (ItemUseCheck(character) == false)
        {
            //원래 이곳에 사용되지 않았다는 메세지 및 사운드 구현해야함
            return;
        }
        Debug.Log("회복 전 : " + character.GetHp());
        //포션 역할 실행 및 인벤토리 최신화
        character.HpTransfer(transHpValue);
        base.UseItem(character, slot);
        Debug.Log("회복됨 : " + character.GetHp());

        character.onTransStatData?.Invoke();

    }

    public override void UpdateInventory(SlotBase slot)
    {
        slot.currentCount--;
        slot.UpdateSlot();
        if(slot.currentCount == 0)
        {
            slot.ClearSlot();
        }
    }

    public override bool ItemUseCheck(Character character)
    {
        
        if (character.GetHp() >= character.GetResultMaxHp())
        {

            Debug.Log("사용 불가. 현재 최대 체력");
            return false;
        }
            

        return true;
    }

    public override string SetItemComment()
    {
        return $"테스트 포션 : <color=#ff5555>{transHpValue}</color>" + "이지롱";
    }
}

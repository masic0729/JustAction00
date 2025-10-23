using UnityEngine;

public class ItemPotion : ItemObject
{
    float transHpValue = 5f;

    protected override void Start()
    {
        base.Start();
    }

    public override void UseItem(Character character, ItemSlot slot)
    {
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
        
    }

    public override void UpdateInventory(ItemSlot slot)
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
        
        if (character.GetHp() >= character.GetMaxHp())
        {

            Debug.Log("사용 불가. 현재 최대 체력");
            return false;
        }
            

        return true;
    }

    
}

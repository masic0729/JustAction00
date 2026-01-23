using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipArmor : EquipDefs
{
    //[SerializeField] int playerArmorIndex;                  //방어구 기본 값은0(아무것도 없는 방어구 상태)으로 분류하며, 최대값은 2이다.
    public override void UseItem(Character character, SlotBase slot)
    {
        base.UseItem(character, slot);
        character.GetComponent<PlayerArmorCustom>().SetPlayerArmorVisual(item.data.equipmentType, playerArmorIndex);
    }

    public override void UpdateInventory(SlotBase slot)
    {
        base.UpdateInventory(slot);
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPotion : ItemObject
{
    float transHpValue = 5f;

    protected override void Start()
    {
        base.Start();
    }

    public override void UseItem(Character character)
    {
        if (ItemUseCheck(character))
        {
            //원래 이곳에 사용되지 않았다는 메세지 및 사운드 구현해야함
            return;
        }

        base.UseItem(character);

        //포션 역할 실행 및 인벤토리 최신화
        character.HpTransfer(transHpValue);
    }

    public override void UpdateInventory(Character character)
    {
        base.UpdateInventory(character);
    }

    public override bool ItemUseCheck(Character character)
    {
        if (character.GetHp() >= character.GetMaxHp())
            return false;

        return true;
    }

    
}

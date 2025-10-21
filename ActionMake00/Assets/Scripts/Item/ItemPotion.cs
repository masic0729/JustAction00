using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPotion : ItemObject
{
    float TranshpValue = 5f;
    protected override void Start()
    {
        base.Start();
    }

    public bool CheckUseItem(Character character)
    {
        if (character.GetHp() == character.GetMaxHp())
            return false;

        return true;

    }

    public override void UseItem(Character character)
    {
        base.UseItem(character);
        
    }

    public override void UpdateInventory(Character character)
    {
        base.UpdateInventory(character);
    }
}

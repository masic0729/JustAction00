using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemObject : MonoBehaviour, ItemInteration, ItemUseChecker
{
    Character test;
    [SerializeField]public ItemBase item;

    // Start is called before the first frame update
    virtual protected void Start()
    {
        /*item.OnCheckUse += CheckUseItem;*/
        item.OnItemUse += UseItem;
        //item.OnItemUpdate += UpdateInventory;
    }


    public virtual void UseItem(Character character, ItemSlot slot)
    {
        UpdateInventory(slot);
    }



    abstract public void UpdateInventory(ItemSlot slot);


    public abstract bool ItemUseCheck(Character character);

}

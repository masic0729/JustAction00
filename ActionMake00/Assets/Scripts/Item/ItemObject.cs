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


    virtual public void UseItem(Character character)
    {
        UpdateInventory(character);

    }

    virtual public void UpdateInventory(Character character)
    {

    }

    public abstract bool ItemUseCheck(Character character);

}

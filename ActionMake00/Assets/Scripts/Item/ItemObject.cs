using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour, ItemInteration
{
    Character test;
    [SerializeField]protected ItemBase item;

    // Start is called before the first frame update
    virtual protected void Start()
    {
        /*item.OnCheckUse += CheckUseItem;*/
        item.OnItemUse += UseItem;
        item.OnItemUpdate += UpdateInventory;
    }


    virtual public void UseItem(Character character)
    {

    }

    virtual public void UpdateInventory(Character character)
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public GameObject slot;
    Transform inventoryTransform;
    const int slotCount = 40;
    List<ItemSlot> lSlot;
    private void Awake()
    {
        Init();
    }


    void Init()
    {
        inventoryTransform = this.gameObject.transform.GetComponentInChildren<GridLayoutGroup>().transform;
        lSlot = new List<ItemSlot>();
        for (int i = 0; i < slotCount; i++)
        {
            ItemSlot instance = Instantiate(slot).GetComponent<ItemSlot>();
            lSlot.Add(instance);
            instance.transform.SetParent(inventoryTransform, false);
        }
        this.gameObject.SetActive(false);
    }

    public void AddItemInList(Item item)
    {
        bool isConsumableItemSum = false;
        ItemSlot voidSlot = null;
        for (int i = 0; i < lSlot.Count; i++)
        {
            if (item.data.itemType == ItemType.Equitment && lSlot[i].CanAddItem() == true)
            {
                lSlot[i].AddItem(item);
                break;
            }
            

            if (item.data.itemType == ItemType.Consumable && lSlot[i].CanSumItem(item))
            {
                Debug.Log("°¡´É");
                isConsumableItemSum = true;
                lSlot[i].SumItem(item);
                break;
            }
            else if (voidSlot == null && lSlot[i].CanAddItem() == true)
            {
                voidSlot = lSlot[i];
            }

        }
        if (isConsumableItemSum == false && voidSlot != null)
        {
            voidSlot.AddItem(item);
        }


    }
}
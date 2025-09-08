using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
            instance.gameObject.name += i;
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
                Debug.Log("가능");
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

    /// <summary>
    /// 인벤토리를 정렬한다
    /// 정렬 기준은 아이템 타입에 따라 다르며, 이후 아이템의 묶음 수량이 많은 순으로 정렬된다
    /// </summary>
    public void SortInventoryTest()
    {
        //Inventory inven = this.gameObject.transform.parent.GetComponent<Inventory>();
        Inventory inven = GameObject.Find("Inventory").GetComponent<Inventory>();

        if (inven == null)
        {
            Debug.Log("inventory is not found");
            return;
        }

        List<ItemSlot> list = inven.lSlot.ToList();

        list.Sort((a, b) => a.type.CompareTo(b.type));
        for(int i = 0; i < list.Count; i++)
        {
            Debug.Log(list[i].currentCount + " " + i.ToString());
        }

        /*foreach(ItemSlot item in inven.lSlot)
        {
            item.ClearSlot();
            //item.UpdateSlot();
            item.UpdateSlot(list[]);
        }*/

        /*for (int i = 0; i < list.Count; i++)
        {
            inven.lSlot[i].ClearSlot();
            inven.lSlot[i].SortSlot(list[i]);
        }*/



        /*list.Sort((a, b) => a.type.CompareTo(b.type));

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].currentCount == 0)
                continue;

            Debug.Log(list[i].currentItem.data.itemName + " 인덱스는 " + i.ToString());
        }

        for (int i = 0; i < inven.lSlot.Count; i++)
        {
            inven.lSlot[i].ClearSlot();
        }

        for (int i = 0; i < list.Count; i++)
        {
            inven.lSlot[i] = list[i];
            inven.lSlot[i].UpdateSlot();
        }*/
    }

    void OtherCodeBox()
    {
        // 1) 아이템만 모으기
        var items = new List<Item>();
        foreach (var slot in lSlot)
            if (slot != null && slot.currentItem != null && slot.currentItem.data != null)
                items.Add(new Item { data = slot.currentItem.data, addCount = slot.currentItem.addCount });

        // 2) 정렬 (예: 타입 → 이름)
        items.Sort((a, b) =>
        {
            int t = a.data.itemType.CompareTo(b.data.itemType);
            if (t != 0) return t;
            return string.Compare(a.data.itemName, b.data.itemName, System.StringComparison.Ordinal);
        });

        // 3) 슬롯 비우고
        foreach (var slot in lSlot) slot.ClearSlot();

        // 4) 앞에서부터 다시 채우기
        for (int i = 0; i < items.Count && i < lSlot.Count; i++)
            lSlot[i].AddItem(items[i]);
    }
}
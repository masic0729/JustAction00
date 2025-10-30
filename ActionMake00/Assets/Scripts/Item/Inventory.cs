using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public GameObject slot;
    [SerializeField] Character inventoryOwner;                   //인벤토리 소유자. 현재는 플레이어 밖에 없음
    Transform inventoryTransform;
    const int slotCount = 40;
    public List<ItemSlot> lSlot;
    ItemSlot dragSlot = null;

    private void Awake()
    {
        Init();
    }


    /// <summary>
    /// 초기에 인벤토리의 슬롯은 40칸으로 정해져 있으며,
    /// 미리 생성 후 인벤토리 슬롯을 상자에 할당
    /// </summary>
    void Init()
    {
        inventoryTransform = this.gameObject.transform.GetComponentInChildren<GridLayoutGroup>().transform;
        lSlot = new List<ItemSlot>();
        for (int i = 0; i < slotCount; i++)
        {
            ItemSlot instance = Instantiate(slot).GetComponent<ItemSlot>();
            instance.gameObject.name += i;
            instance.SetTarget(inventoryOwner);
            instance.SetInventory(this);
            instance.SetSlotIndex(i);
            instance.slotType = SlotType.InventorySlot;

            lSlot.Add(instance);
            instance.transform.SetParent(inventoryTransform, false);
        }
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// 인벤토리 내 빈 슬롯을 기준으로 새 아이템 데이터를 넣는다
    /// 획득을 할 때 추가되는 값을 그대로 정의를 한다
    /// </summary>
    /// <param name="itemObject">인벤토리에 삽입할 아이템 정보</param>
    public void AddItemInList(ItemObject itemObject)
    {
        bool isConsumableItemSum = false;
        ItemSlot voidSlot = null;
        for (int i = 0; i < lSlot.Count; i++)
        {
            if (voidSlot == null && lSlot[i].CanAddItem() == true)
            {
                voidSlot = lSlot[i];
            }

            if (itemObject.item.data.itemType == ItemType.Equitment && lSlot[i].CanAddItem() == true)
            {
                //lSlot[i].AddItem(itemObject.item);
                lSlot[i].AddItem(itemObject);
                return;
            }
            

            if (itemObject.item.data.itemType == ItemType.Consumable && lSlot[i].CanSumItem(itemObject.item))
            {
                isConsumableItemSum = true;
                //lSlot[i].SumItem(itemObject.item);
                lSlot[i].SumItem(itemObject);
                return;
            }

        }
        if (isConsumableItemSum == false && voidSlot != null)
        {
            //voidSlot.AddItem(itemObject.item);
            voidSlot.AddItem(itemObject);
        }

    }

    /// <summary>
    /// 인벤토리 내 모든 아이템을 정렬한다.
    /// 장비, 소비아이템, 기타아이템(이러한 종류는 없어질 수 있음)으로 구분되며,
    /// 장비 소비 아이템 순으로 정렬된다.
    /// 소비 아이템의 경우 묶음 개념이기 때문에 묶음 값이 큰 순으로 정렬된다.
    /// </summary>
    /*public void SortInventoryTest()
    {
        // 1) 현재 슬롯에서 (아이템 데이터, 개수) 스냅샷 수집
        List<(ItemData data, int count)> snaps = new List<(ItemData data, int count)>();
        for (int i = 0; i < lSlot.Count; i++)
        {
            ItemSlot s = lSlot[i];
            bool hasItem = (s != null &&
                            s.currentItem != null &&
                            s.currentItem.data != null &&
                            s.currentCount > 0);

            if (hasItem)
            {
                ItemData data = s.currentItem.data;
                int count = s.currentCount;
                snaps.Add((data, count));
            }
        }

        
        snaps.Sort(delegate ((ItemData data, int count) x, (ItemData data, int count) y)
        {
            // 타입을 기준으로 정렬할 것
            int typeCompare = x.data.itemType.CompareTo(y.data.itemType);
            if (typeCompare != 0)
                return typeCompare;

            //소비 아이템은 묶음 값이 큰 순서대로 정렬한다
            if (x.data.itemType == ItemType.Consumable)
            {
                int stackCompare = y.count.CompareTo(x.count); // 방향은 내림차순
                if (stackCompare != 0)
                    return stackCompare;
            }

            //이름 오름차순 정렬
            return string.Compare(x.data.itemName, y.data.itemName, System.StringComparison.Ordinal);
        });

        // 3) 기존 슬롯 모두 비우기
        for (int i = 0; i < lSlot.Count; i++)
        {
            lSlot[i].ClearSlot();
        }

        // 4) 정렬된 순서대로 다시 채우기
        //    (AddItem은 count를 '대입'하도록 동작해야 함)
        int index = 0;
        while (index < snaps.Count && index < lSlot.Count)
        {
            (ItemData data, int count) snap = snaps[index];
            //ItemBase item = new ItemBase { data = snap.data, addCount = snap.count };
            ItemBase item = new ItemBase { data = snap.data, addCount = snap.count };
            lSlot[index].AddItem(item);
            item.slotData = lSlot[index];
            index++;
        }

        // (선택) 필요하면 여기서 디버그 로그로 결과 확인
        // for (int i = 0; i < snaps.Count; i++)
        // {
        //     Debug.Log($"[{i}] {snaps[i].data.itemType} {snaps[i].data.itemName} x{snaps[i].count}");
        // }
    }*/

    public void SortInventoryTest()
    {
        var snaps = new List<(ItemBase item, int count)>();
        for (int i = 0; i < lSlot.Count; i++)
        {
            var s = lSlot[i];
            bool hasItem = (s != null &&
                            s.currentItem != null &&
                            s.currentItem.data != null &&
                            s.currentCount > 0);
            if (hasItem)
            {
                snaps.Add((s.currentItem, s.currentCount));
            }
        }

        snaps.Sort((x, y) =>
        {
            int typeCompare = x.item.data.itemType.CompareTo(y.item.data.itemType);
            if (typeCompare != 0) return typeCompare;

            if (x.item.data.itemType == ItemType.Consumable)
            {
                int stackCompare = y.count.CompareTo(x.count);
                if (stackCompare != 0) return stackCompare;
            }

            return string.Compare(x.item.data.itemName, y.item.data.itemName, System.StringComparison.Ordinal);
        });

        for (int i = 0; i < lSlot.Count; i++)
            lSlot[i].ClearSlot();

        int idx = 0;
        while (idx < snaps.Count && idx < lSlot.Count)
        {
            var snap = snaps[idx];

            // 아이템의 현재 수량을 스냅샷 값으로 맞춤
            snap.item.addCount = snap.count;

            lSlot[idx].AddItem(snap.item);

            snap.item.slotData = lSlot[idx];

            idx++;
        }
    }

    public void SetDragSlot(ItemSlot slot) => dragSlot = slot;

    public ItemSlot GetDragSlot() => dragSlot;

    public void ResetDragSlot() => dragSlot = null;


    void OtherCodeBox()
    {
        // 1) 아이템만 모으기
        var items = new List<ItemBase>();
        foreach (var slot in lSlot)
            if (slot != null && slot.currentItem != null && slot.currentItem.data != null)
                items.Add(new ItemBase { data = slot.currentItem.data, addCount = slot.currentItem.addCount });

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


        /*
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

            list.Sort((a, b) => a.attackType.CompareTo(b.attackType));
            for(int i = 0; i < list.Count; i++)
            {
                Debug.Log(list[i].currentCount + " " + list[i].maxCount + " " + i.ToString());
            }

            for (int i = 0; i < list.Count; i++)
            {
                inven.lSlot[i].ClearSlot();
                inven.lSlot[i].SortSlot(list[i]);
            }
        }*/
    }
}
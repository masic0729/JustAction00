using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SlotType
{
    InventorySlot,
    Equipment

}
public abstract class SlotBase : MonoBehaviour
{
    public ItemType type = ItemType.nullItem;                                               //아이템 정렬을 위한 데이터 타입
    public Character target;

    public TextMeshProUGUI countText;

    [SerializeField] protected Inventory inventory;      
    //슬롯의 인벤토리 주체. 아이템 간 이동 시 활용함

    public SlotType slotType;                                                               //장비 형태
    public Sprite baseSlotImage;                    //비어있을 때 쓰는 이미지
    public Image slotIcon;
    public ItemBase currentItem = null;

    /*public string slotItemName;
    public string slotItemComment;*/

    public Action<Character, SlotBase> OnSlotItemUse;   //아이템을 사용할 때 발생하는 상호작용
    
    //아이템 사용 후 처리에 대한 부분. 예시로 슬롯 데이터 삭제,
    //카운트 및 차감 등등 기본적인 상호작용 이후의 처리를 뜻한다
    public Action<SlotBase> OnSlotItemUpdate;

    public int currentCount = 0, maxCount = 0;
    public int slotIndex = -1;                      //슬롯의 인덱스 정보

    public abstract bool AddItem(ItemObject itemObject);

    public abstract bool AddItem(ItemBase item);

    public abstract void SwapItem(ItemSlot slot);

    public abstract void SwapItem(SlotBase slot);

    public abstract void SumItem(ItemObject itemObject);

    public abstract void SetItemDirect(ItemData data, int count, string comment);

    public abstract void UpdateSlot();

    public abstract void SortSlot(ItemBase itemData);

    public abstract void UpdateUI();

    //public abstract void TestInteraction();

    public abstract void ClearSlot();

    public abstract bool CanAddItem();

    public abstract bool CanSumItem(ItemBase item);

    public Inventory GetInventory() => inventory;

    public void SetTarget(Character character)
    {
        target = character;
    }

    public string GetItemName(ItemBase slotItem)
    {
        return slotItem.data.itemName;
    }

    public string GetItemComment(ItemBase slotItem)
    {
        return slotItem.comment;
    }
}

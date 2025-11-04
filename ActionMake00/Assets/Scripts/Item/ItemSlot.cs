using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 슬롯 타입에 따라 장비칸인 지,
/// 인벤토리 칸인 지 구분할 수 있다.
/// 이에 따라서 캐릭터 정보 및 인벤토리를 일괄적으로 처리할 수 있을 것으로 판단
/// </summary>
public class ItemSlot : SlotBase, IPointerClickHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    private void Start()
    {
        slotType = SlotType.InventorySlot;
    }

    public override bool AddItem(ItemObject itemObject)
    {
        if (itemObject == null)
        {
            Debug.Log("아이템을 저장하지 못했습니다.");
            return false;
        }

        currentItem = itemObject.item;
        icon.sprite = itemObject.item.data.icon;
        icon.enabled = true;
        icon.color = new Vector4(1, 1, 1, 1);
        currentItem.slotData = this;
        currentCount = itemObject.item.addCount;
        maxCount = itemObject.item.data.maxCount;
        type = itemObject.item.data.itemType;

        // UseItem 시그니처가 Action<Character, SlotBase> 라면 그대로 대입
        OnItemUse = itemObject.UseItem;

        // 만약 UseItem 시그니처가 Action<Character, ItemSlot> 라면, 아래 어댑터 사용
        // OnItemUse = (ch, sb) =>
        // {
        //     ItemSlot islot = sb as ItemSlot;
        //     if (islot != null) itemObject.UseItem(ch, islot);
        //     else Debug.LogWarning("UseItem 슬롯 타입 불일치");
        // };

        UpdateUI();
        Debug.Log("성공");
        return true;
    }

    public override bool AddItem(ItemBase item)
    {
        if (item == null)
        {
            Debug.Log("아이템을 저장하지 못했습니다.");
            return false;
        }

        currentItem = item;
        icon.sprite = item.data.icon;
        icon.enabled = true;
        icon.color = new Vector4(1, 1, 1, 1);
        currentItem.slotData = this;
        currentCount = item.addCount;
        maxCount = item.data.maxCount;
        type = item.data.itemType;

        // null 덮어쓰기 방지
        if (item.OnItemUse != null) OnItemUse = item.OnItemUse;
        if (item.OnItemUpdate != null) OnItemUpdate = item.OnItemUpdate;

        UpdateUI();
        Debug.Log("성공");
        return true;
    }

    public override void SwapItem(ItemSlot slot)
    {
        if (slot == null || slot == this) return;

        // 현재 슬롯 데이터 보관 (명시형 + SlotBase 시그니처)
        ItemBase t_item = this.currentItem;
        int t_count = this.currentCount;
        int t_max = this.maxCount;
        ItemType t_type = this.type;
        Action<Character, SlotBase> t_use = this.OnItemUse;
        Action<SlotBase> t_update = this.OnItemUpdate;

        // this <- slot
        this.currentItem = slot.currentItem;
        this.currentCount = slot.currentCount;
        this.maxCount = slot.maxCount;
        this.type = slot.type;
        this.OnItemUse = slot.OnItemUse;
        this.OnItemUpdate = slot.OnItemUpdate;
        if (this.currentItem != null) this.currentItem.slotData = this;

        // slot <- temp
        slot.currentItem = t_item;
        slot.currentCount = t_count;
        slot.maxCount = t_max;
        slot.type = t_type;
        slot.OnItemUse = t_use;
        slot.OnItemUpdate = t_update;
        if (slot.currentItem != null) slot.currentItem.slotData = slot;

        // 각자 UI 갱신 — 자기 슬롯의 currentItem 기준
        //if (this.currentItem != null)
        if (this.type != ItemType.nullItem)
        {
            this.icon.sprite = this.currentItem.data.icon;
            this.icon.enabled = true;
            this.icon.color = Color.white;
            this.countText.text = (this.currentCount > 1) ? this.currentCount.ToString() : "";
        }
        else
        {
            this.ClearSlot();
        }

        //if (slot.currentItem != null)
        if (slot.type != ItemType.nullItem)
        {
            slot.icon.sprite = slot.currentItem.data.icon;
            slot.icon.enabled = true;
            slot.icon.color = Color.white;
            slot.countText.text = (slot.currentCount > 1) ? slot.currentCount.ToString() : "";
        }
        else
        {
            slot.ClearSlot();
        }
    }

    public override void SwapItem(SlotBase slot)
    {
        if (slot == null || slot == this) return;

        // 현재 슬롯 데이터 보관 (명시형 + SlotBase 시그니처)
        ItemBase t_item = this.currentItem;
        int t_count = this.currentCount;
        int t_max = this.maxCount;
        ItemType t_type = this.type;
        Action<Character, SlotBase> t_use = this.OnItemUse;
        Action<SlotBase> t_update = this.OnItemUpdate;

        // this <- slot
        this.currentItem = slot.currentItem;
        this.currentCount = slot.currentCount;
        this.maxCount = slot.maxCount;
        this.type = slot.type;
        this.OnItemUse = slot.OnItemUse;
        this.OnItemUpdate = slot.OnItemUpdate;
        if (this.currentItem != null) this.currentItem.slotData = this;

        // slot <- temp
        slot.currentItem = t_item;
        slot.currentCount = t_count;
        slot.maxCount = t_max;
        slot.type = t_type;
        slot.OnItemUse = t_use;
        slot.OnItemUpdate = t_update;
        if (slot.currentItem != null) slot.currentItem.slotData = slot;

        // 각자 UI 갱신 — 자기 슬롯의 currentItem 기준
        //if (this.currentItem != null)
        if (this.type != ItemType.nullItem)
        {
            this.icon.sprite = this.currentItem.data.icon;
            this.icon.enabled = true;
            this.icon.color = Color.white;
            this.countText.text = (this.currentCount > 1) ? this.currentCount.ToString() : "";
        }
        else
        {
            this.ClearSlot();
        }

        //if (slot.currentItem != null)
        if (slot.type != ItemType.nullItem)
        {
            slot.icon.sprite = slot.currentItem.data.icon;
            slot.icon.enabled = true;
            slot.icon.color = Color.white;
            slot.countText.text = (slot.currentCount > 1) ? slot.currentCount.ToString() : "";
        }
        else
        {
            slot.ClearSlot();
        }
    }

    public override void SumItem(ItemObject itemObject)
    {
        if (itemObject == null) return;

        int sum = currentCount + itemObject.item.addCount;
        currentCount = (sum > maxCount) ? maxCount : sum;

        UpdateUI();
        Debug.Log("합체 성공");
    }

    /// <summary>
    /// 정렬/재배치 때 쓰는 "직접 세팅" API (스냅샷을 그대로 주입)
    /// </summary>
    public override void SetItemDirect(ItemData data, int count)
    {
        if (data == null || count <= 0)
        {
            ClearSlot();
            return;
        }

        currentItem = new ItemBase { data = data, addCount = count }; // 독립 인스턴스
        currentItem.slotData = this;
        maxCount = data.maxCount;
        currentCount = count;
        icon.sprite = data.icon;
        icon.enabled = true;
        icon.color = Color.white;

        UpdateUI();
    }

    /// <summary>
    /// 슬롯 내 데이터가 존재하나, 해당 데이터의 수치값이 변경될 때 실행한다
    /// </summary>
    public override void UpdateSlot()
    {
        if (currentItem == null) // ← 반전 버그 수정
        {
            ClearSlot();
            return;
        }

        icon.sprite = currentItem.data.icon;
        icon.enabled = true;
        icon.color = new Vector4(1, 1, 1, 1);
        maxCount = currentItem.data.maxCount;
        type = currentItem.data.itemType;
        UpdateUI();
    }

    public override void SortSlot(ItemBase itemData)
    {
        if (itemData == null)
        {
            ClearSlot();
            return;
        }

        // 1) 아이템/수치 세팅
        currentItem = (itemData.slotData != null) ? itemData.slotData.currentItem : itemData;
        if (currentItem != null) currentItem.slotData = this;
        currentCount = itemData.currentCount;

        // 2) 메타/타입
        if (currentItem != null)
        {
            maxCount = currentItem.data.maxCount;
            type = currentItem.data.itemType;

            // 3) UI 갱신
            icon.sprite = currentItem.data.icon;
            icon.enabled = true;
            icon.color = Color.white;
            countText.text = (currentCount > 1) ? currentCount.ToString() : "";
        }
        else
        {
            ClearSlot();
        }
    }

    protected override void UpdateUI()
    {
        if (currentItem == null) { countText.text = ""; return; }
        countText.text = (currentCount > 1) ? currentCount.ToString() : "";
    }

    public override void TestInteraction()
    {
        if (currentItem == null) return;
        OnItemUse?.Invoke(target, this);
    }

    public override void ClearSlot()
    {
        currentItem = null;
        icon.sprite = baseSlotImage;
        countText.text = "";
        currentCount = 0;
        maxCount = 0;
        type = ItemType.nullItem;
        OnItemUse = null;
        OnItemUpdate = null;
    }

    public override bool CanAddItem()
    {
        return (type == ItemType.nullItem);
    }

    public override bool CanSumItem(ItemBase item)
    {
        if (currentItem.data == null || item == null) return false;

        if (currentItem.data.itemName == item.data.itemName &&
            currentCount + item.addCount <= maxCount)
        {
            return true;
        }
        return false;
    }

    public void SetInventory(Inventory inven) => inventory = inven;

    public void SetSlotIndex(int index) => slotIndex = index;
    public int GetSlotIndex() => slotIndex;

    public void OnPointerClick(PointerEventData eventData)
    {
        // 우클릭인 경우에만 실행
        if (eventData.button != PointerEventData.InputButton.Right) return;

        Debug.Log("OnClick");
        if (currentItem == null) return; // ← 반전 버그 수정

        OnItemUse?.Invoke(target, this);
        OnItemUpdate?.Invoke(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        // 마우스 좌클릭 드롭 + 드래그 중이어야만
        if (eventData.button != PointerEventData.InputButton.Left || inventory.GetDragSlot() == null) return;

        Debug.Log("OnDrop");

        ItemSlot dragSlot = inventory.GetDragSlot();
        dragSlot.SwapItem(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag");
        inventory.ResetDragSlot();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        Debug.Log("OnDrag");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 좌클릭 + 아이템이 있어야 드래그 시작
        if (eventData.button != PointerEventData.InputButton.Left || this.currentItem == null) return;

        Debug.Log("OnBeginDrag");
        inventory.SetDragSlot(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("OnEnter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("OnExit");
    }
}

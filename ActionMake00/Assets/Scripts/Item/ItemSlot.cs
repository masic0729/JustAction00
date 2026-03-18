using System;
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
        slotIcon = GetComponent<Image>();
    }

    public override bool AddItem(ItemObject itemObject)
    {
        if (itemObject == null)
        {
            Debug.Log("아이템을 저장하지 못했습니다.");
            return false;
        }

        //ItemData data = ib.data;
        //왜 이렇게 됐냐.
        //1. 스크립터블 오브젝트는 필요가 없어졌다.
        //2. csv기반으로 데이터를 불러오기 때문에, 이를 해당아이템 데이터를 불러온 후 인벤토리에 할당한다
        itemObject.SetItemData(itemObject.itemId);
        ItemData data = SetItemData(itemObject.itemId);
        ItemBase ib = itemObject.item;      // ItemBase

        // 1) 슬롯 기본 세팅
        currentItem = ib;
        currentItem.slotData = this;
        currentItem.comment = itemObject.GetItemComment();

        slotIcon.sprite = data.icon;
        slotIcon.enabled = true;
        slotIcon.color = Color.white;

        type = data.itemType;
        maxCount = data.maxCount;

        // 2) 수량 결정: currentCount > 0 우선, 없으면 addCount, 장비면 1
        int count = currentItem.currentCount;
        if (count <= 0)
        {
            if (type == ItemType.Equitment)
                count = 1;
            else
                count = ib.addCount; // 소비/기타는 획득 수량
        }
        currentCount = Mathf.Clamp(count, 0, maxCount);

        // 3) 델리게이트 바인딩: 아이템 쪽이 우선, 없으면 ItemObject의 핸들러 사용
        if (ib.OnItemUse != null)
            OnSlotItemUse = ib.OnItemUse;

        /*else if (itemObject.UseItem != null) */
        OnSlotItemUse = itemObject.UseItem;

        if (ib.OnItemUpdate != null)
            OnSlotItemUpdate = ib.OnItemUpdate;
        // (itemObject에 업데이트 콜백이 따로 있다면 여기서 보강)

        UpdateUI();
        return true;
    }


    /// <summary>
    /// 정렬할 때 해당 함수 사용중임
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public override bool AddItem(ItemBase item)
    {

        if (item == null || item.data == null)
        {
            Debug.Log("아이템을 저장하지 못했습니다.");
            return false;
        }

        currentItem = new ItemBase  // 슬롯 전용 복제(레퍼런스 공유 방지)
        {

            data = item.data,
            addCount = item.addCount,
            currentCount = (item.currentCount > 0)
                            ? item.currentCount
                            : (item.data.itemType == ItemType.Equitment ? 1 : item.addCount),
            OnItemUse = item.OnItemUse,
            OnItemUpdate = item.OnItemUpdate,

            comment = item.comment,
        };

        OnSlotItemUse = item.OnItemUse;
        OnSlotItemUpdate = item.OnItemUpdate;
        currentItem.slotData = this;

        type = currentItem.data.itemType;
        maxCount = currentItem.data.maxCount;
        currentCount = Mathf.Clamp(currentItem.currentCount, 0, maxCount);

        slotIcon.sprite = currentItem.data.icon;
        slotIcon.enabled = true;
        slotIcon.color = Color.white;
        UpdateUI();
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
        Action<Character, SlotBase> t_use = this.OnSlotItemUse;
        Action<SlotBase> t_update = this.OnSlotItemUpdate;

        // this <- slot
        this.currentItem = slot.currentItem;
        this.currentCount = slot.currentCount;
        this.maxCount = slot.maxCount;
        this.type = slot.type;
        this.OnSlotItemUse = slot.OnSlotItemUse;
        this.OnSlotItemUpdate = slot.OnSlotItemUpdate;
        //this.slotItemName = slot.slotItemName;
        if (this.currentItem != null) this.currentItem.slotData = this;

        // slot <- temp
        slot.currentItem = t_item;
        slot.currentCount = t_count;
        slot.maxCount = t_max;
        slot.type = t_type;
        slot.OnSlotItemUse = t_use;
        slot.OnSlotItemUpdate = t_update;
        /*slot.slotItemName = t_item.data.itemName;
        slot.slotItemComment = t_item.comment;*/
        if (slot.currentItem != null) slot.currentItem.slotData = slot;

        // 각자 UI 갱신 — 자기 슬롯의 currentItem 기준
        //if (this.currentItem != null)
        if (this.type != ItemType.nullItem)
        {
            this.slotIcon.sprite = this.currentItem.data.icon;
            this.slotIcon.enabled = true;
            this.slotIcon.color = Color.white;
            this.countText.text = (this.currentCount > 1) ? this.currentCount.ToString() : "";
        }
        else
        {
            this.ClearSlot();
        }

        //if (slot.currentItem != null)
        if (slot.type != ItemType.nullItem)
        {
            slot.slotIcon.sprite = slot.currentItem.data.icon;
            slot.slotIcon.enabled = true;
            slot.slotIcon.color = Color.white;
            slot.countText.text = (slot.currentCount > 1) ? slot.currentCount.ToString() : "";
        }
        else
        {
            slot.ClearSlot();
        }

        //스왑한 슬롯이 장비 슬롯이면, 스왑 이후 장비 사용 및 업데이트
        if (slot.slotType == SlotType.Equipment)
        {
            slot.OnSlotItemUse?.Invoke(target, slot);
            slot.OnSlotItemUpdate?.Invoke(slot);
        }
    }

    public override void SwapItem(SlotBase slot)
    {
        if (slot == null || slot == this)
            return;

        // 현재 슬롯 데이터 보관 (명시형 + SlotBase 시그니처)
        ItemBase t_item = this.currentItem;
        int t_count = this.currentCount;
        int t_max = this.maxCount;
        ItemType t_type = this.type;
        Action<Character, SlotBase> t_use = this.OnSlotItemUse;
        Action<SlotBase> t_update = this.OnSlotItemUpdate;

        // this <- slot
        this.currentItem = slot.currentItem;
        this.currentCount = slot.currentCount;
        this.maxCount = slot.maxCount;
        this.type = slot.type;
        this.OnSlotItemUse = slot.OnSlotItemUse;
        this.OnSlotItemUpdate = slot.OnSlotItemUpdate;
        //this.slotItemComment = slot.slotItemComment;

        if (this.currentItem != null) this.currentItem.slotData = this;

        // slot <- temp
        slot.currentItem = t_item;
        slot.currentCount = t_count;
        slot.maxCount = t_max;
        slot.type = t_type;
        slot.OnSlotItemUse = t_use;
        slot.OnSlotItemUpdate = t_update;
        //slot.slotItemComment = t_item.comment;

        if (slot.currentItem != null)
            slot.currentItem.slotData = slot;

        // 각자 UI 갱신 — 자기 슬롯의 currentItem 기준
        //if (this.currentItem != null)
        if (this.type != ItemType.nullItem)
        {
            this.slotIcon.sprite = this.currentItem.data.icon;
            this.slotIcon.enabled = true;
            this.slotIcon.color = Color.white;
            this.countText.text = (this.currentCount > 1) ? this.currentCount.ToString() : "";
        }
        else
        {
            this.ClearSlot();
        }

        //if (slot.currentItem != null)
        if (slot.type != ItemType.nullItem)
        {
            slot.slotIcon.sprite = slot.currentItem.data.icon;
            slot.slotIcon.enabled = true;
            slot.slotIcon.color = Color.white;
            slot.countText.text = (slot.currentCount > 1) ? slot.currentCount.ToString() : "";
        }
        else
        {
            slot.ClearSlot();
        }

        //스왑한 슬롯이 장비 슬롯이면, 스왑 이후 장비 사용 및 업데이트
        if(slot.slotType == SlotType.Equipment)
        {
            slot.OnSlotItemUse?.Invoke(target, slot);
            slot.OnSlotItemUpdate?.Invoke(slot);
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
    public override void SetItemDirect(ItemData data, int count, string comment)
    {
        if (data == null || count <= 0)
        {
            ClearSlot();
            return;
        }

        currentItem = new ItemBase { data = data, addCount = count, comment = comment };
        currentItem.slotData = this;

        type = data.itemType;          // ← 추가
        maxCount = data.maxCount;
        currentCount = count;
        //slotItemName = data.itemName;
        //slotItemComment = data.

        slotIcon.sprite = data.icon;
        slotIcon.enabled = true;
        slotIcon.color = Color.white;

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

        slotIcon.sprite = currentItem.data.icon;
        slotIcon.enabled = true;
        slotIcon.color = new Vector4(1, 1, 1, 1);
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
            slotIcon.sprite = currentItem.data.icon;
            slotIcon.enabled = true;
            slotIcon.color = Color.white;
            countText.text = (currentCount > 1) ? currentCount.ToString() : "";
        }
        else
        {
            ClearSlot();
        }
    }

    public override void UpdateUI()
    {
        if (currentItem == null) 
        {
            countText.text = "";
            return;
        }
        countText.text = (currentCount > 1) ? currentCount.ToString() : "";
    }

    /*public override void TestInteraction()
    {
        if (currentItem == null) return;
        OnItemUse?.Invoke(target, this);
    }*/

    public override void ClearSlot()
    {
        currentItem = null;
        slotIcon.sprite = baseSlotImage;
        countText.text = "";
        currentCount = 0;
        maxCount = 0;
        type = ItemType.nullItem;
        OnSlotItemUse = null;
        OnSlotItemUpdate = null;
    }

    public override bool CanAddItem()
    {
        return (type == ItemType.nullItem);
    }

    public override bool CanSumItem(ItemBase item)
    {
        // 유효성 검사
        if (item == null)
            return false;

        if (currentItem == null)
            return false;                         // 이 슬롯이 비어있으면 합칠 수 없음

        if (type != ItemType.Consumable)
            return false;                       // 소비 아이템만 합치기

        // 같은 아이템인지: 데이터 레퍼런스 기준 (이름 문자열보다 안전)
        if (currentItem.data != item.data)
            return false;

        // 슬롯의 '현재 수량'을 써야 함 (기존 bug: currentItem.currentCount 사용)
        if (currentCount <= 0)
            return false;                          // 방어적 체크

        if (currentCount >= maxCount)
            return false;                         // 이미 꽉 찬 슬롯

        // 완전히 들어갈 수 있을 때만 합치기 (넘치면 빈 슬롯으로 가도록)
        return (currentCount + item.addCount) <= maxCount;
    }

    public void SetInventory(Inventory inven) => inventory = inven;

    public void SetSlotIndex(int index) => slotIndex = index;
    public int GetSlotIndex() => slotIndex;

    public void OnPointerClick(PointerEventData eventData)
    {
        // 우클릭인 경우에만 실행
        /*if (eventData.button != PointerEventData.InputButton.Right)
            return;*/

        Debug.Log("OnClick");
        if (currentItem == null)
            return; // 반전 버그 수정

        
        OnSlotItemUse?.Invoke(target, this);
        OnSlotItemUpdate?.Invoke(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        // 마우스 좌클릭 드롭 + 드래그 중이어야만
        if (eventData.button != PointerEventData.InputButton.Left || inventory.GetDragSlot() == null) return;

        Debug.Log("OnDrop");

        SlotBase dragSlot = inventory.GetDragSlot();
        dragSlot.SwapItem(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag");

        RectTransform invenTransform = inventory.GetComponent<RectTransform>();
        


        //드래그 데이터 삭제
        inventory.ResetDragSlot();
        inventory.DragImage.sprite = null;
        inventory.DragImage.gameObject.SetActive(false);

        //원래 이 부분은 인벤토리 외부로 이동 시 파괴되는 시스템인데, 논리 오류로 추정
        if (RectTransformUtility.RectangleContainsScreenPoint(invenTransform, eventData.position) == false)
        {
            //this.ClearSlot();
            //Debug.Log("나 실행됨");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 좌클릭 + 아이템이 있어야 드래그 시작
        if (eventData.button != PointerEventData.InputButton.Left || this.currentCount == 0) return;

        inventory.SetDragSlot(this);
        inventory.DragImage.sprite = inventory.GetDragSlot().slotIcon.sprite;
        inventory.DragImage.gameObject.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (type == ItemType.nullItem) return;
        Debug.Log("OnEnter");

        inventory.TooltipView.gameObject.SetActive(true);

        // 1) 툴팁이 속한 캔버스 찾기
        Canvas canvas = inventory.TooltipView.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.transform as RectTransform;

        // 2) Canvas RenderMode에 맞는 카메라 선택 (Overlay면 null)
        Camera uiCam = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera;

        // 3) 마우스 스크린 좌표 -> 캔버스 로컬 좌표
        Vector2 mouseLocal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            uiCam,
            out mouseLocal
        );

        // 4) 툴팁 크기(캔버스 로컬 기준) 가져오기
        RectTransform tooltipRT = inventory.TooltipView;
        // 레이아웃/스케일 반영된 크기를 쓰고 싶으면 lossyScale까지 고려
        Vector2 tooltipSize = new Vector2(
            tooltipRT.rect.width * tooltipRT.lossyScale.x,
            tooltipRT.rect.height * tooltipRT.lossyScale.y
        );

        // 5) 최종 위치 계산 후 anchoredPosition으로 배치
        tooltipRT.anchoredPosition = CalToolTipPosition(mouseLocal, tooltipSize, canvasRect, 55f);

        inventory.testItemName.text = this.currentItem.data.itemName;
        inventory.testItemComment.text = GetItemComment(this.currentItem);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventory.TooltipView.gameObject.SetActive(false);

    }
}

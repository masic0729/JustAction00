using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum EquipmentType
{
    Weapon = 0,
    Head = 1,
    Top = 2,
    Bottom = 3,
    None = -1
}

/// <summary>
/// 장비에 의한 조정되는 스탯
/// </summary>
public struct EquipmentStat
{
    public float addHp;                        //체력 조정값
    public float addDamage;                    //데미지 조정값
    public float addDefense;                   //방어력 조정값
    public float addMoveSpeed;                 //이동속도 조정값
    //float defenseBreaking;                   //현재는 없지만, 방어력 무시 비율값
}
/// <summary>
/// 장비 슬롯:
/// - 인벤토리에서 우클릭/드래그-드롭 시, 해당 장비 타입 슬롯과 "스왑"된다.
/// - 장비 슬롯에서 우클릭 시, 인벤토리 비어있는 칸으로 "반납"을 시도한다(성공 시 장비 슬롯은 Clear).
/// - 장비는 비스택(항상 1개)으로 취급하고, ItemUse 액션을 호출하지 않는다.
/// </summary>
public class EquipmentSlot : SlotBase, IPointerClickHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [Header("장비 슬롯 메타")]
    public EquipmentType equipmentType;     // 이 슬롯이 담당하는 장비 타입(Weapon/Head/Top/Bottom...)
    public AddStatData equipmentStat;       // 이 슬롯에 장착된 아이템이 제공하는 스탯(합산은 EquipmentManager가 처리)

    private void Awake()
    {
        slotIcon = GetComponent<Image>();
    }

    /* ---------- 공통 유틸 ---------- */

    /// <summary>
    /// 장비 아이템인지(및 슬롯 타입과 호환되는지) 판단.
    /// 실제 프로젝트의 데이터 구조(예: EquipmentItemData.equipType)에 맞게 확장해서 쓰면 됨.
    /// 기본값은 '장비 타입만 일치하면 OK'로 처리.
    /// </summary>
    private bool IsCompatibleEquipment(ItemBase item)
    {
        if (item == null || item.data == null) return false;
        if (item.data.itemType != ItemType.Equitment) return false;
        if (item.data.equipmentType != this.equipmentType) return false;

        // ※ 필요 시 여기서 item.data를 캐스팅하여 실제 equipType을 비교하세요.
        // ex) var equipData = item.data as EquipmentItemData;
        //     return equipData != null && equipData.equipType == equipmentType;
        return true; // 최소 보장: 장비 아이템이면 허용 (타입 라우팅은 외부에서 보장된다고 가정)
    }

    /// <summary>
    /// 장비는 비스택이므로 항상 1개로 강제한다.
    /// 슬롯 전용 런타임 인스턴스로 복제하여 참조 공유 문제를 방지한다.
    /// </summary>
    private ItemBase MakeEquippedCopy(ItemBase src)
    {
        return new ItemBase
        {
            data = src.data,
            addCount = 1,
            currentCount = 1,
            OnItemUse = src.OnItemUse,     // 장비창에선 쓰지 않지만 보존
            OnItemUpdate = src.OnItemUpdate,
            comment = src.comment
        };
    }

    private void RefreshIcon()
    {
        if (currentItem != null && currentItem.data != null)
        {
            slotIcon.sprite = currentItem.data.icon;
            slotIcon.enabled = true;
            slotIcon.color = Color.white;
        }
        else
        {
            slotIcon.sprite = baseSlotImage;
            slotIcon.enabled = true;
            slotIcon.color = Color.white;
        }
        UpdateUI();
    }

    private void Recalc()
    {
        // 슬롯마다 들고 있는 equipmentStat은 필요 시 외부에서 조회하여 합산.
        // 여기서는 합산 트리거만 건다.
        GetInventory()?.equipManager?.UpdateCharacterStatResult();
    }


    /// <summary>
    /// 해당 함수는 정렬할 때 사용함
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public override bool AddItem(ItemBase item)
    {

        // 외부에서 강제로 "장착"하고자 할 때 사용할 수 있는 경로 (보통은 SwapItem을 통해 들어옴)
        if (!IsCompatibleEquipment(item))
        {
            Debug.Log("장비 슬롯과 호환되지 않는 아이템입니다.");
            return false;
        }

        currentItem.data = SetItemData(item.data.id);
        currentItem.slotData = this;
        currentItem.comment = item.comment;
        Debug.Log("장비?? : " + item.comment);

        type = ItemType.Equitment;
        maxCount = currentItem.data.maxCount;
        currentCount = 1;

        // (선택) 장비 스탯 로드가 필요하면 여기서 equipmentStat을 세팅
        equipmentStat = new AddStatData(); // 실제 구조가 있으면 그에 맞게 채우세요.

        RefreshIcon();
        Recalc();
        return true;
    }

    public override bool AddItem(ItemObject itemObject)
    {
        if (itemObject == null || itemObject.item == null || itemObject.item.data == null)
        {
            Debug.Log("아이템을 저장하지 못했습니다.");
            return false;
        }

        var ib = itemObject.item;      // ItemBase
        //var data = ib.data;
        var data = SetItemData(itemObject.itemId);
        //currentItem.data = SetItemData(item.data.id);

        // 1) 슬롯 기본 세팅
        currentItem = ib;
        currentItem.slotData = this;
        

        slotIcon.sprite = data.icon;
        slotIcon.enabled = true;
        slotIcon.color = Color.white;
        //currentItem.data.itemName = itemObject.item.data.itemName;
        currentItem.comment = itemObject.GetItemComment();

        type = data.itemType;
        maxCount = data.maxCount;

        // 2) 수량 결정: currentCount > 0 우선, 없으면 addCount, 장비면 1
        int count = ib.currentCount;
        if (count <= 0)
        {
            if (type == ItemType.Equitment)
                count = 1;
            else
                count = ib.addCount; // 소비/기타는 획득 수량
        }
        currentCount = Mathf.Clamp(count, 0, maxCount);

        // 3) 델리게이트 바인딩: 아이템 쪽이 우선, 없으면 ItemObject의 핸들러 사용
        if (ib.OnItemUse != null) OnSlotItemUse = ib.OnItemUse;
        /*else if (itemObject.UseItem != null) */
        OnSlotItemUse = itemObject.UseItem;

        if (ib.OnItemUpdate != null) OnSlotItemUpdate = ib.OnItemUpdate;
        // (itemObject에 업데이트 콜백이 따로 있다면 여기서 보강)

        UpdateUI();
        //Debug.Log("성공");
        return true;
    }

    public override void SwapItem(ItemSlot slot)
    {
        // 인벤토리 슬롯과 "교환" 로직
        if (slot == null || slot == this) return;

        // 1) 인벤토리에서 온 아이템이 장비로 들어올 수 있는지 체크
        var incoming = slot.currentItem;
        if (incoming == null || !IsCompatibleEquipment(incoming))
        {
            Debug.Log("해당 장비 슬롯에 장착할 수 없는 아이템입니다.");
            return;
        }

        // 2) 현재 장비(있다면)를 인벤토리로 내보내고, 인벤토리 아이템을 장비로 들여온다 (진짜 스왑)
        ItemBase prevEquip = currentItem;      // 장비칸 기존 아이템
        int prevCount = currentCount;
        int prevMax = maxCount;
        ItemType prevType = type;
        var prevUse = OnSlotItemUse;
        var prevUpdate = OnSlotItemUpdate;

        // this <- slot (장비는 복제 + 1개)
        currentItem = MakeEquippedCopy(incoming);
        currentItem.slotData = this;

        type = ItemType.Equitment;
        maxCount = currentItem.data.maxCount;
        currentCount = 1;
        OnSlotItemUse = slot.OnSlotItemUse;
        OnSlotItemUpdate = slot.OnSlotItemUpdate;

        // (선택) 장비 스탯 로드
        equipmentStat = new AddStatData(); // 실제 구조가 있으면 채우세요.

        if(prevType == ItemType.nullItem)
        {
            slot.ClearSlot();
        }
        else
        {
            // slot <- prevEquip (인벤토리에는 원본/복제 무엇이 들어가도 되지만,
            // 인벤토리 쪽 AddItem이 복제 로직을 갖고 있으므로 prevEquip 그대로 꽂아도 안전)
            slot.currentItem = prevEquip;
            slot.currentCount = prevCount;
            slot.maxCount = prevMax;
            slot.type = (prevEquip != null) ? prevEquip.data.itemType : ItemType.nullItem;
            slot.OnSlotItemUse = prevUse;
            slot.OnSlotItemUpdate = prevUpdate;
            slot.currentItem.slotData = slot;
        }
            
        /*if (slot.currentItem != null) slot.currentItem.slotData = slot;*/

        Recalc();

        // 3) 아이콘 반영
        RefreshIcon();
        if (slot.type != ItemType.nullItem)
        {
            slot.slotIcon.sprite = slot.currentItem.data.icon;
            slot.slotIcon.enabled = true;
            slot.slotIcon.color = Color.white;
            slot.UpdateUI();
        }
        else
        {
        }

        // 4) 합산 재계산
        
    }

    public override void SwapItem(SlotBase slot)
    {
        // 인벤토리 외의 슬롯 타입과 교환은 현재 사용하지 않음
        // 필요 시 ItemSlot 경로만 허용
        var invSlot = slot as ItemSlot;
        if (invSlot != null) { SwapItem(invSlot); }
    }

    public override void SumItem(ItemObject itemObject) { /* 장비는 스택 불가 */ }

    /// <summary>
    /// 장비의 개수는 고정1 이므로 파라미터에 이러한 구조로 되어있음
    /// </summary>
    /// <param name="data"></param>
    /// <param name="_"></param>
    /// <param name="comment"></param>
    public override void SetItemDirect(ItemData data, int _, string comment)
    {
        if (data == null)
        {
            ClearSlot();
            return;
        }

        // 강제 장착(자동 장착 등) 경로
        var dummy = new ItemBase { data = data, addCount = 1, currentCount = 1, comment = comment };
        AddItem(dummy);
    }

    public override void UpdateSlot()
    {
        // 장비창은 스택 텍스트가 없으므로 아이콘만 갱신
        RefreshIcon();
    }

    public override void SortSlot(ItemBase itemData) { /* 장비창은 정렬 안함 */ }

    public override void UpdateUI()
    {
        // 장비는 스택 표기를 사용하지 않음
        if (countText != null) countText.text = "";
    }

    /// <summary>
    /// 장비 해제(우클릭): 인벤토리 비어있는 칸으로 반납 시도 → 성공 시 장비칸 Clear
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;
        if (currentItem == null) return;

        // 무기 등 해제 금지 슬롯이면 여기서 차단
        if (equipmentType == EquipmentType.Weapon) { Debug.Log("무기는 해제되지 않습니다."); return; }

        var inven = GetInventory();
        if (inven == null) return;

        // 1) 인벤토리의 빈 칸 찾기(보다 안전한 판정: currentItem == null)
        ItemSlot empty = null;
        foreach (var s in inven.lSlot)
        {
            if (s != null && s.type == ItemType.nullItem) { empty = s; break; }
        }
        //
        if (empty == null)
        {
            Debug.Log("인벤토리에 빈 슬롯이 없습니다.");
            return;
        }
        
        if(currentItem.OnItemUse != null)
        {
            Debug.Log("current가 비어있음");
        }
        if(OnSlotItemUse != null)
        {
            Debug.Log("슬롯 내 기능이 비어있음");
        }
        // 2) 인벤토리 빈칸에 장비 아이템 추가(인벤토리 AddItem이 복제/수량 보정 처리)
        empty.SwapItem(this);



        // 3) 장비칸 비우기 + 재계산
        ClearSlot();
        Recalc();

        target.GetComponent<PlayerArmorCustom>().SetPlayerArmorVisual(equipmentType, 0);
    }

    public override void ClearSlot()
    {
        currentItem = null;
        currentCount = 0;
        maxCount = 0;
        type = ItemType.nullItem;

        OnSlotItemUse = null;
        OnSlotItemUpdate = null;

        equipmentStat = new AddStatData(); // 장비 스탯 초기화

        RefreshIcon(); // 아이콘/텍스트 리셋
        // 재계산은 호출부에서 한 번 더 묶어서 호출하는 편이 안전하지만, 여기서는 생략 가능
    }

    public override bool CanAddItem()
    {
        // 장비칸이 비어있을 때만 장착 허용(전투 중 차단 등의 조건은 여기서 추가)
        return currentItem == null;
    }

    public override bool CanSumItem(ItemBase item) { return false; } // 장비는 합치기 불가

    /* ---------- 드래그/드롭 훅(인벤토리에서 끌어다 놓을 때) ---------- */

    public void OnBeginDrag(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left || this.currentCount == 0) return;

        inventory.SetDragSlot(this);
        inventory.DragImage.sprite = inventory.GetDragSlot().slotIcon.sprite;
        inventory.DragImage.gameObject.SetActive(true);
    }
    public void OnDrag(PointerEventData eventData) { }
    public void OnEndDrag(PointerEventData eventData) {
        inventory.ResetDragSlot();
        inventory.DragImage.sprite = null;
        inventory.DragImage.gameObject.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        // 인벤토리에서 드래그해온 슬롯이 있으면 스왑
        if (eventData.button != PointerEventData.InputButton.Left) return;
        var inven = GetInventory();
        if (inven == null) return;

        var dragSlot = inven.GetDragSlot();
        if (dragSlot == null) return;

        SwapItem(dragSlot);
    }

    public void OnPointerEnter(PointerEventData eventData) 
    {
        Debug.Log("OnEnter");
        if (type == ItemType.nullItem) return;

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
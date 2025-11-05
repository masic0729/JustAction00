using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

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
            OnItemUpdate = src.OnItemUpdate
        };
    }

    private void RefreshIcon()
    {
        if (currentItem != null && currentItem.data != null)
        {
            icon.sprite = currentItem.data.icon;
            icon.enabled = true;
            icon.color = Color.white;
        }
        else
        {
            icon.sprite = baseSlotImage;
            icon.enabled = true;
            icon.color = Color.white;
        }
        UpdateUI();
    }

    private void Recalc()
    {
        // 슬롯마다 들고 있는 equipmentStat은 필요 시 외부에서 조회하여 합산.
        // 여기서는 합산 트리거만 건다.
        GetInventory()?.equipManager?.UpdateCharacterStatResult();
    }


    public override bool AddItem(ItemBase item)
    {
        // 외부에서 강제로 "장착"하고자 할 때 사용할 수 있는 경로 (보통은 SwapItem을 통해 들어옴)
        if (!IsCompatibleEquipment(item))
        {
            Debug.Log("장비 슬롯과 호환되지 않는 아이템입니다.");
            return false;
        }

        currentItem = MakeEquippedCopy(item);
        currentItem.slotData = this;

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
        // 장비창은 ItemObject 경로를 쓰지 않아도 되지만, 안전하게 동일 처리
        if (itemObject == null || itemObject.item == null) return false;
        return AddItem(itemObject.item);
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
        var prevUse = OnItemUse;
        var prevUpdate = OnItemUpdate;

        // this <- slot (장비는 복제 + 1개)
        currentItem = MakeEquippedCopy(incoming);
        currentItem.slotData = this;

        type = ItemType.Equitment;
        maxCount = currentItem.data.maxCount;
        currentCount = 1;
        OnItemUse = slot.OnItemUse;
        OnItemUpdate = slot.OnItemUpdate;

        // (선택) 장비 스탯 로드
        equipmentStat = new AddStatData(); // 실제 구조가 있으면 채우세요.

        // slot <- prevEquip (인벤토리에는 원본/복제 무엇이 들어가도 되지만,
        // 인벤토리 쪽 AddItem이 복제 로직을 갖고 있으므로 prevEquip 그대로 꽂아도 안전)
        slot.currentItem = prevEquip;
        slot.currentCount = prevCount;
        slot.maxCount = prevMax;
        slot.type = (prevEquip != null) ? prevEquip.data.itemType : ItemType.nullItem;
        slot.OnItemUse = prevUse;
        slot.OnItemUpdate = prevUpdate;
        if (slot.currentItem != null) slot.currentItem.slotData = slot;

        // 3) 아이콘 반영
        RefreshIcon();
        if (slot.type != ItemType.nullItem)
        {
            slot.icon.sprite = slot.currentItem.data.icon;
            slot.icon.enabled = true;
            slot.icon.color = Color.white;
            slot.UpdateUI();
        }
        else
        {
            slot.ClearSlot();
        }

        // 4) 합산 재계산
        Recalc();
    }

    public override void SwapItem(SlotBase slot)
    {
        // 인벤토리 외의 슬롯 타입과 교환은 현재 사용하지 않음
        // 필요 시 ItemSlot 경로만 허용
        var invSlot = slot as ItemSlot;
        if (invSlot != null) { SwapItem(invSlot); }
    }

    public override void SumItem(ItemObject itemObject) { /* 장비는 스택 불가 */ }

    public override void SetItemDirect(ItemData data, int _)
    {
        if (data == null)
        {
            ClearSlot();
            return;
        }

        // 강제 장착(자동 장착 등) 경로
        var dummy = new ItemBase { data = data, addCount = 1, currentCount = 1 };
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
        // if (equipmentType == EquipmentType.Weapon) { Debug.Log("무기는 해제되지 않습니다."); return; }

        var inven = GetInventory();
        if (inven == null) return;

        // 1) 인벤토리의 빈 칸 찾기(보다 안전한 판정: currentItem == null)
        ItemSlot empty = null;
        foreach (var s in inven.lSlot)
        {
            if (s != null && s.currentItem == null) { empty = s; break; }
        }

        if (empty == null)
        {
            Debug.Log("인벤토리에 빈 슬롯이 없습니다.");
            return;
        }
        
        if(currentItem.OnItemUse != null)
        {
            Debug.Log("current가 비어있음");
        }
        if(OnItemUse != null)
        {
            Debug.Log("슬롯 내 기능이 비어있음");
        }
        // 2) 인벤토리 빈칸에 장비 아이템 추가(인벤토리 AddItem이 복제/수량 보정 처리)
        empty.SwapItem(this);



        // 3) 장비칸 비우기 + 재계산
        ClearSlot();
        Recalc();
    }

    public override void ClearSlot()
    {
        currentItem = null;
        currentCount = 0;
        maxCount = 0;
        type = ItemType.nullItem;

        OnItemUse = null;
        OnItemUpdate = null;

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

    public void OnBeginDrag(PointerEventData eventData) { }
    public void OnDrag(PointerEventData eventData) { }
    public void OnEndDrag(PointerEventData eventData) { }

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

    public void OnPointerEnter(PointerEventData eventData) { }
    public void OnPointerExit(PointerEventData eventData) { }
}
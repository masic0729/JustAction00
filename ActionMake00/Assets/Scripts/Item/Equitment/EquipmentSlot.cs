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

public class EquipmentSlot : SlotBase, IPointerClickHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    public EquipmentType equipmentType;
    public AddStatData equipmentStat;

    private void Start()
    {
        //currentItem = null;
        //SetTarget(inventory.inventoryOwner);
        //SetInventory(this);
    }

    /// <summary>
    /// 장비창의 슬롯은 아이템 정보를 기반으로
    /// 데이터를 수정한다
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public override bool AddItem(ItemBase item)
    {
        if (item.data.itemType != ItemType.Equitment) return false;

        if (item.data.equipmentType != equipmentType) return false;

        if (item != null)
        {
            currentItem = item;
            icon.sprite = item.data.icon;
            icon.enabled = true;
            icon.color = new Vector4(1, 1, 1, 1);
            currentItem.slotData = this;
            currentCount = item.addCount;
            maxCount = item.data.maxCount;
            type = item.data.itemType;
            //OnItemUse = itemObject.item.OnItemUse;
            OnItemUse = item.OnItemUse;
            //OnItemUpdate = item.OnItemUpdate;
            UpdateUI();
            Debug.Log("성공");
            return true;
        }
        else
        {
            Debug.Log("아이템을 저장하지 못했습니다.");
            return false;
        }

    }

    /// <summary>
    /// 장비 슬롯의 경우 해당 함수는 사용하지 않는다.
    /// </summary>
    /// <param name="itemObject"></param>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public override bool AddItem(ItemObject itemObject)
    {
        if (itemObject != null)
        {
            currentItem = itemObject.item;
            icon.sprite = itemObject.item.data.icon;
            icon.enabled = true;
            icon.color = new Vector4(1, 1, 1, 1);
            currentItem.slotData = this;
            currentCount = itemObject.item.addCount;
            maxCount = itemObject.item.data.maxCount;
            type = itemObject.item.data.itemType;
            //OnItemUse = itemObject.item.OnItemUse;
            OnItemUse = itemObject.UseItem;
            //OnItemUpdate = item.OnItemUpdate;
            UpdateUI();
            Debug.Log("성공");
            return true;
        }
        else
        {
            Debug.Log("아이템을 저장하지 못했습니다.");
            return false;
        }
    }

    public override void SwapItem(ItemSlot slot)
    {
        if (slot == null || slot == this)
            return;

        // 현재 슬롯 데이터 보관
        ItemBase t_item = currentItem;
        int t_count = currentCount;
        int t_max = maxCount;
        ItemType t_type = type;
        var t_use = OnItemUse;
        var t_update = OnItemUpdate;

        // this <- s
        currentItem = slot.currentItem;
        currentCount = slot.currentCount;
        maxCount = slot.maxCount;
        type = slot.type;
        OnItemUse = slot.OnItemUse;
        OnItemUpdate = slot.OnItemUpdate;
        if (slot.type != ItemType.nullItem) currentItem.slotData = this;

        // s <- temp
        slot.currentItem = t_item;
        slot.currentCount = t_count;
        slot.maxCount = t_max;
        slot.type = t_type;
        slot.OnItemUse = t_use;
        slot.OnItemUpdate = t_update;
        if (slot.type != ItemType.nullItem) slot.currentItem.slotData = slot;

        // 각자 UI 갱신
        if (slot.type != ItemType.nullItem)
        {
            icon.sprite = currentItem.data.icon;
            icon.enabled = true;
            icon.color = Color.white;
            //countText.text = currentCount > 1 ? currentCount.ToString() : "";
        }
        else
        {
            ClearSlot();
        }

        if (slot.type != ItemType.nullItem)
        {
            slot.icon.sprite = slot.currentItem.data.icon;
            slot.icon.enabled = true;
            slot.icon.color = Color.white;
            //s.countText.text = s.currentCount > 1 ? s.currentCount.ToString() : "";
        }
        else
        {
            slot.ClearSlot();
        }
    }

    public override void SwapItem(SlotBase slot)
    {
        /*if (slot == null || slot == this)
            return;

        // 현재 슬롯 데이터 보관
        ItemBase t_item = currentItem;
        int t_count = currentCount;
        int t_max = maxCount;
        ItemType t_type = type;
        var t_use = OnItemUse;
        var t_update = OnItemUpdate;

        // this <- s
        currentItem = slot.currentItem;
        currentCount = slot.currentCount;
        maxCount = slot.maxCount;
        type = slot.type;
        OnItemUse = slot.OnItemUse;
        OnItemUpdate = slot.OnItemUpdate;
        if (currentItem != null) currentItem.slotData = this;

        // s <- temp
        slot.currentItem = t_item;
        slot.currentCount = t_count;
        slot.maxCount = t_max;
        slot.type = t_type;
        slot.OnItemUse = t_use;
        slot.OnItemUpdate = t_update;
        if (slot.currentItem != null) slot.currentItem.slotData = slot;

        // 각자 UI 갱신
        if (currentItem != null)
        {
            icon.sprite = currentItem.data.icon;
            icon.enabled = true;
            icon.color = Color.white;
            //countText.text = currentCount > 1 ? currentCount.ToString() : "";
        }
        else
        {
            ClearSlot();
        }

        if (slot.currentItem != null)
        {
            slot.icon.sprite = slot.currentItem.data.icon;
            slot.icon.enabled = true;
            slot.icon.color = Color.white;
            //s.countText.text = s.currentCount > 1 ? s.currentCount.ToString() : "";
        }
        else
        {
            slot.ClearSlot();
        }*/
    }

    public override void SumItem(ItemObject itemObject)
    {
        throw new System.NotImplementedException();
    }

    public override void SetItemDirect(ItemData data, int count)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 이곳엔 장비를 통한 캐릭터 스탯 조정
    /// 하지만 사용하지 않을 수 있음
    /// </summary>
    public override void UpdateSlot()
    {
        throw new System.NotImplementedException();

    }

    /// <summary>
    /// 해당 기능은 장비엔 없어야 함
    /// </summary>
    /// <param name="itemData"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public override void SortSlot(ItemBase itemData)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 아직 써야하는 이유를 찾지 못했음.
    /// 혹시 모르니 한번 더 생각할 것
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    protected override void UpdateUI()
    {
        if (currentCount == 0)
            return;

        countText.text = currentCount > 1 ? currentCount.ToString() : "";
    }

    /// <summary>
    /// 단순 테스트용
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    public override void TestInteraction()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 장비를 변경 또는 제거할 때 스탯 및 메쉬 적용을 초기화해야한다
    /// </summary>
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

    /// <summary>
    /// 현재는 당장 구현하지 않겠지만,
    /// 전투 중이나 모든 스킬이 초기화되지 않았다면
    /// 장비 간 교환이 일어나면 안된다
    /// </summary>
    /// <returns></returns>
    public override bool CanAddItem()
    {
        
        //전투 중인 조건문으로 전투 중이라면 처리 불가
        /*if ()
        { 
            return false;
        }*/
        
        
        
        

        return true;
    }

    public bool CanSwapItem()
    {
        //전투 중인 조건문으로 전투 중이라면 교환 불가
        /*if ()
        { 
            return false;
        }*/



        return true;
    }

    /// <summary>
    /// 장비는 합쳐지면 안된다.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public override bool CanSumItem(ItemBase item)
    {
        return false;
    }



    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnDrop(PointerEventData eventData)
    {
        /*
        // 마우스 좌클릭 드롭 + 드래그 중이어야만
        if (eventData.button != PointerEventData.InputButton.Left ||
            inventory.GetDragSlot() == null) return;

        Debug.Log("OnDrop");

        ItemSlot dragSlot = inventory.GetDragSlot();
        //if (dragSlot == this) return;

        // 단 한 번의 스왑으로 끝낸다
        dragSlot.SwapItem(this);*/
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 우클릭인 경우에만 실행
        if (eventData.button != PointerEventData.InputButton.Right) return;

        //무기 슬롯은 내가 만든 게임 상 우클릭 해제할 수는 없다.
        //반드시 무기 교체만 이루어 진다.
        if (equipmentType == EquipmentType.Weapon)
        {
            Debug.Log("무기는 해제되지 않는다.");
            return;
        }

        Inventory inven = transform.parent.GetComponent<EquipmentManager>().inven;

        for(int i = 0; i < inven.lSlot.Count; i++)
        {
            if (inven.lSlot[i].currentCount == 0)
            {
                inven.lSlot[i].SwapItem(this);
                //교환했다면 장비 슬롯에 있는 장비 옵션을 해당 장비 슬롯 데이터에 저장한다
                equipmentStat = new AddStatData();


                //저장 이후 장비 슬롯 매니저에 각 부위의 장비들의 스탯을 최신화해야한다
                GetInventory().equipManager.UpdateCharacterStatResult();
                //ClearSlot();
                break;
            }
        }

/*        ItemSlot slot = inven.lSlot.Find(s => s.currentItem == null);
        if(slot != null)
        {
            //교환했다면 장비 슬롯에 있는 장비 옵션을 해당 장비 슬롯 데이터에 저장한다
            equipmentStat = new AddStatData();


            //저장 이후 장비 슬롯 매니저에 각 부위의 장비들의 스탯을 최신화해야한다
            slot.GetInventory().equipManager.UpdateCharacterStatResult();
        }
        else
        {
            Debug.Log("해제 하기엔, 자리가 없음");
        }*/

        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum EquipmentType
{
    Weapon,
    Head,
    Top,
    Bottom
}

/// <summary>
/// 장비에 의한 조정되는 스탯
/// </summary>
public struct EquipmentStat
{
    float addHp;                        //체력 조정값
    float addDamage;                    //데미지 조정값
    float addDefense;                   //방어력 조정값
    //float defenseBreaking;            //현재는 없지만, 방어력 무시 비율값
}

public class EquipmentSlot : MonoBehaviour, IPointerClickHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    public EquipmentType equipmentType;
    public EquipmentStat equitmentStat;



    public void ClearSlot()
    {

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

    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }
}

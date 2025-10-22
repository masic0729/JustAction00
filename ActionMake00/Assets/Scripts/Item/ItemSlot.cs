using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{

    Character target;
    Inventory inventory;                                                                    //슬롯의 인벤토리 주체. 아이템 간 이동 시 활용함
    public Sprite baseSlotImage;                                                            //비어있을 때 쓰는 이미지
    public ItemBase currentItem;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI countText;
    public int currentCount = 0, maxCount = 1;
    public ItemType type = ItemType.nullItem;                                               //아이템 정렬을 위한 데이터 타입

    public Action<Character> OnItemUse;                              //아이템을 사용할 때 발생하는 상호작용
    public Action<ItemSlot> OnItemUpdate;                            //아이템 사용 후 처리에 대한 부분. 예시로 슬롯 데이터 삭제, 카운트 및 차감 등등 기본적인 상호작용 이후의 처리를 뜻한다

    public bool AddItem(ItemBase item)
    {

        if (item != null)
        {
            currentItem = item;
            icon.sprite = item.data.icon;
            icon.enabled = true;
            icon.color = new Vector4(1,1,1,1);
            currentItem.slotData = this;
            currentCount = item.addCount;
            maxCount = item.data.maxCount;
            type = item.data.itemType;
            OnItemUse = item.OnItemUse;
            OnItemUpdate = item.OnItemUpdate;
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

    public void SumItem(ItemBase item)
    {
        if (item == null)
            return;

        currentCount += item.addCount;
        UpdateUI();
        Debug.Log("합체 성공");
    }

    /// <summary>
    /// 정렬/재배치 때 쓰는 "직접 세팅" API (스냅샷을 그대로 주입)
    /// </summary>
    /// <param name="data"></param>
    /// <param name="count"></param>
    public void SetItemDirect(ItemData data, int count)
    {
        if (data == null || count <= 0) { ClearSlot(); return; }

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
    public void UpdateSlot()
    {
        if (currentCount == 0)
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

    public void SortSlot(ItemBase itemData)
    {
        if (itemData.currentCount == 0)
            return;

        // 1) 아이템/수치 세팅 (필요시 깊은 복사)
        // 참조를 공유해도 된다면 아래처럼:
        currentItem = itemData.slotData.currentItem;
        currentItem.slotData = this;
        currentCount = itemData.currentCount;
        
        //maxCount = (itemData.maxCount > 0) ? itemData.maxCount : itemData.currentItem.data.maxCount;

        maxCount = itemData.slotData.maxCount;
        type = itemData.slotData.currentItem.data.itemType;

        // 2) UI 갱신
        icon.sprite = currentItem.data.icon;
        icon.enabled = true;
        icon.color = Color.white;
        countText.text = currentCount > 1 ? currentCount.ToString() : "";
    }

    void UpdateUI()
    {
        if (currentCount == 0)
            return;

        countText.text = currentCount > 1 ? currentCount.ToString() : "";

    }

    public void TestInteraction()
    {
        if (currentItem == null)
        {
            return;
        }
        OnItemUse(target);
        //currentItem.OnItemUpdate(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 우클릭인 경우에만 실행
        if (eventData.button != PointerEventData.InputButton.Right) return;

        if (currentItem == null)
        {
            return;
        }
        OnItemUse(target);
        OnItemUpdate(this);
    }

    public void ClearSlot()
    {
        currentItem = null;
        icon.sprite = baseSlotImage;
        //icon.enabled = false;
        countText.text = "";
        currentCount = 0;
        maxCount = 0;
        type = ItemType.nullItem;
        OnItemUse = null;
        OnItemUpdate = null;
    }

    public bool CanAddItem()
    {
        if (currentCount == 0)
            return true;

        return false;
    }

    public bool CanSumItem(ItemBase item)
    {
        if (currentCount == 0)
            return false;

        if(currentItem.data.itemName == item.data.itemName && currentCount + item.addCount <= maxCount)
        {
            return true;
        }
            
        return false;
    }

    public void SetTarget(Character character)
    {
        target = character;
    }

    public Inventory GetInventory() => inventory;

    public void SetInventory(Inventory inven) => inventory = inven;
}
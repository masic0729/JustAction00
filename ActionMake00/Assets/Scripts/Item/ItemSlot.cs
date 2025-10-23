using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ItemSlot : MonoBehaviour, IPointerClickHandler,
    IBeginDragHandler, IDragHandler,IEndDragHandler,IDropHandler, IPointerEnterHandler, IPointerExitHandler
{

    Character target;
    Inventory inventory;                                                                    //슬롯의 인벤토리 주체. 아이템 간 이동 시 활용함
    public Sprite baseSlotImage;                                                            //비어있을 때 쓰는 이미지
    public ItemBase currentItem;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI countText;
    public int currentCount = 0, maxCount = 1;
    public ItemType type = ItemType.nullItem;                                               //아이템 정렬을 위한 데이터 타입
    int slotIndex = -1;                                                                        //슬롯의 인덱스 정보

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

    void SwapItem(ItemSlot slot)
    {
        if (slot == null)
            return;


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

    public void SetSlotIndex(int index) => slotIndex = index;
    public int GetSlotIndex() => slotIndex;

    public void OnPointerClick(PointerEventData eventData)
    {
        // 우클릭인 경우에만 실행
        if (eventData.button != PointerEventData.InputButton.Right) return;

        Debug.Log("OnClick");
        if (currentCount == 0)
            return;

        OnItemUse(target);
        OnItemUpdate(this);
    }

    /// <summary>
    /// 어쨋든 마우스에 아이템 데이터가 있다면,
    /// 각 슬롯의 데이터를 스왑한다
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData)
    {
        //마우스가 들고 있는 아이템이 비어있거나, 제대로 좌클릭을 한게 아니면 처리하지 말것
        if (eventData.button != PointerEventData.InputButton.Left &&
            inventory.GetDragSlot() != null) return;

        Debug.Log("OnDrop");
        //이게 여기에서 스왑 처리

        inventory.lSlot[inventory.GetDragSlot().slotIndex] = this;
        //this = inventory.GetDragSlot();

        //드래그 중인 슬롯 데이터를 기반으로 데이터 교환
        SetSlotData(inventory.GetDragSlot());
    }

    /// <summary>
    /// 해당 함수는 현재까지 드래그에 의한 슬롯 데이터 스왑을 처리한다
    /// 추후 같은 기능이 존재하면 로직이 변경될 수 있음
    /// </summary>
    void SetSlotData(ItemSlot dragItem)
    {
        
    }

    /// <summary>
    /// 드래그를 끝낼 때 사용하는데, 이건 보류
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag");
        inventory.ResetDragSlot();

    }

    /// <summary>
    /// 단순 좌클릭 기반으로 그래그 됨 뿐만 아니라,
    /// 아이템을 쥐고 있는 지 확인하는 조건까지 사용하여 처리할 것
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        Debug.Log("OnDrag");
    }

    /// <summary>
    /// 해당 슬롯을 그래그 시작할 때, 마우스에 해당 아이템의 아이콘 이미지를 띄워 마우스 포인터에 고정한다
    /// 이때, 비어있는 슬롯은 아무 일도 일어나지 않게 한다
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 좌클릭인 경우에만 실행
        if (eventData.button != PointerEventData.InputButton.Left && 
            this.currentItem != null) return;

        Debug.Log("OnBeginDrag");
        inventory.SetDragSlot(this);

    }

    /// <summary>
    /// 인벤토리 슬롯에 들어갈 때 아이템에 대한 정보를 띄우는 걸
    /// 보통 처리한다
    /// 하지만 지금은 사용하지 않는다.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("OnEnter");

    }

    /// <summary>
    /// 마우스가 해당 슬롯을 벗어날 때,
    /// 해당 슬롯 내 아이템의 정보 노출을 숨길 때 사용한다.
    /// 하지만 지금은 사용하지 않는다.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("OnExit");
    }
}
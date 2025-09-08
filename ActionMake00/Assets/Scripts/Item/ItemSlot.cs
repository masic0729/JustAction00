using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEditor.Progress;

public class ItemSlot : MonoBehaviour
{
    //[SerializeField]private Item currentItem;
    public Item currentItem;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI countText;
    public int currentCount = 0, maxCount = 0;
    public ItemType type = ItemType.nullItem;                                               //아이템 정렬을 위한 데이터 타입.

    public bool AddItem(Item item)
    {

        if (item != null)
        {
            currentItem = item;
            icon.sprite = item.data.icon;
            icon.enabled = true;
            icon.color = new Vector4(1,1,1,1);

            currentCount = currentItem.addCount;
            maxCount = item.data.maxCount;
            type = item.data.itemType;

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

    public void SumItem(Item item)
    {
        if (item == null)
            return;

        currentCount += item.addCount;
        UpdateUI();
        Debug.Log("합체 성공");
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

    public void SortSlot(ItemSlot itemData)
    {
        if (itemData.currentCount == 0)
            return;

        // 1) 아이템/수치 세팅 (필요시 깊은 복사)
        // 참조를 공유해도 된다면 아래처럼:
        currentItem = itemData.currentItem;
        currentCount = itemData.currentCount;
        maxCount = (itemData.maxCount > 0) ? itemData.maxCount : itemData.currentItem.data.maxCount;
        type = itemData.currentItem.data.itemType;

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


    public void ClearSlot()
    {
        currentItem = null;
        icon.sprite = null;
        icon.enabled = false;
        countText.text = "";
        currentCount = 0;
        maxCount = 0;
        type = ItemType.nullItem;
    }

    public bool CanAddItem()
    {
        if (currentCount == 0)
            return true;

        return false;
    }

    public bool CanSumItem(Item item)
    {
        if (currentCount == 0)
            return false;

        if(currentItem.data.itemName == item.data.itemName && currentCount + item.addCount <= maxCount)
        {
            return true;
        }
            
        return false;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    [SerializeField]private Item currentItem;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI countText;
    private int currentCount = 0, maxCount = 0;

    public bool AddItem(Item item)
    {

        if (item != null)
        {
            currentItem = item;
            icon.sprite = item.data.icon;
            icon.enabled = true;
            icon.color = new Vector4(1,1,1,1);
            currentCount += currentItem.addCount;
            maxCount = item.data.maxCount;
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

    }

    void UpdateUI()
    {
        if (currentCount == 0)
            return;

        //currentCount = currentItem.currentCount;
        countText.text = currentCount > 1 ? currentCount.ToString() : "";

    }

    /// <summary>
    /// 특정 조건에 의해 인벤토리의 두 데이터가 교환을 할 때 실행한다
    /// </summary>
    public void SwapSlot()
    {

    }

    public void ClearSlot()
    {
        currentItem = null;
        icon.sprite = null;
        icon.enabled = false;
        countText.text = "";
        currentCount = 0;
        maxCount = 0;

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
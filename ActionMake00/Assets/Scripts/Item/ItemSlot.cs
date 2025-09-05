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

    public bool AddItem(Item item)
    {

        if (item != null && (currentItem == null || currentItem.GetType() == item.GetType()))
        {
            currentItem = item;
            icon.sprite = item.data.icon;
            icon.enabled = true;
            icon.color = new Vector4(1,1,1,1);
            countText.text = item.count > 1 ? item.count.ToString() : "";
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
    /// 슬롯 내 데이터가 존재하나, 해당 데이터의 수치값이 변경될 때 실행한다
    /// </summary>
    public void UpdateSlot()
    {

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
        icon.enabled = false;
        countText.text = "";
    }

    public bool CanAddItem()
    {
        if(currentItem.data == null)
        {
            return true;
        }

        return false;
    }
}
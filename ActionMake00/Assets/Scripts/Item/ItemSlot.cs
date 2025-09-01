using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    private Item currentItem;
    private Image icon;
    private Text countText;

    public void SetItem(Item item)
    {
        currentItem = item;

        if (item != null && item.data != null)
        {
            icon.sprite = item.data.icon;
            icon.enabled = true;
            countText.text = item.count > 1 ? item.count.ToString() : "";
        }
        else
        {
            Clear();
        }
    }

    public void Clear()
    {
        currentItem = null;
        icon.enabled = false;
        countText.text = "";
    }
}
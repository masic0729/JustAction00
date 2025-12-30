using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoulletItem : MonoBehaviour
{
    RoulletPlaying roullet;
    RoulletUI roulletUI;
    public ItemObject[] items;

    private void Start()
    {
        roullet = GetComponent<RoulletPlaying>();
        roulletUI = GetComponent<RoulletUI>();
    }

    /// <summary>
    /// 플레이어에게 결과에 따른 보상을 준다.
    /// </summary>
    /// <returns></returns>
    public void GiveItemToPlayer(int index)
    {
        Debug.Log("룰렛에 의한 아이템 할당 시도");

        ItemObject insItem = items[index];
        insItem.SetItemData(insItem.itemId);

        roullet.GetPlayerInventory().AddItemInList(insItem);
        roulletUI.ShowResultPanel(insItem.item.data.icon);


        //insItem.item.data.icon
    }
}
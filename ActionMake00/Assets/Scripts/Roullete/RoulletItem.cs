using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoulletItem : MonoBehaviour
{
    RoulletPlaying roullet;
    public ItemObject[] items;

    private void Start()
    {
        roullet = GetComponent<RoulletPlaying>();
    }

    /// <summary>
    /// 플레이어에게 결과에 따른 보상을 준다.
    /// </summary>
    /// <returns></returns>
    public ItemObject GiveItemToPlayer(int index)
    {
        Debug.Log("아이템 할당 시도");
        roullet.GetPlayerInventory().AddItemInList(items[index]);
        return null;
    }
}

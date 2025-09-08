using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum ItemType
{
    Equitment,                      //장비
    Consumable,                     //소비
    Miscellaneous,                  //기타
    nullItem                        //정렬용 타입
}

[CreateAssetMenu(fileName = "ItemConfig", menuName ="GameData/ItemConfig")]
public class ItemData : ScriptableObject
{
    [Header("아이템 기본 정보")]
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public int maxCount;
}

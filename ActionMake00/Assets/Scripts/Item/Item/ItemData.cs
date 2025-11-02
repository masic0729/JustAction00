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

    //장비 타입에 대한 정보. 기본값은 없음
    [Header("장비 아이템 설정")]

    public EquipmentType equipmentType = EquipmentType.None;
    

    public int maxCount;
}

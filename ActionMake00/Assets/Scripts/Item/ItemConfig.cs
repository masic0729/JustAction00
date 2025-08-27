using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum ItemType
{
    Equitment,                      //장비
    Consumable,                     //소비
    Miscellaneous                   //기타
}

[CreateAssetMenu(fileName = "ItemConfig", menuName ="GameData/ItemConfig")]
public class ItemConfig : ScriptableObject
{
    [Header("아이템 기본 정보")]
    [SerializeField] string itemName;
    [SerializeField] ItemType type;
    [SerializeField] string description;
    [SerializeField] Sprite spriteIcon;
    [SerializeField] ParticleSystem pItemGet;
    [SerializeField] ParticleSystem pItemUse;
    [SerializeField] AudioSource audioItemGet; 
    [SerializeField] AudioSource audioItemUse; 
}

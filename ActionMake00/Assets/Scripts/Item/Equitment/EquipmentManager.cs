using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

enum StatInfo
{
    MaxHP = 0,
    Damage = 1,
    Defense = 2,
    MoveSpeed = 3
}

public class EquipmentManager : MonoBehaviour
{
    public EquipmentSlot[] equipSlots;
    [SerializeField] Character StatViewTarget;              //캐릭터 능력치를 노출하려는 대상
    public Text[] statTexts;
    public Inventory inven;

    //안쓸 수도 있음
    public Dictionary<string, EquipmentSlot> equipSlotDic = 
        new Dictionary<string, EquipmentSlot>();

    public StatModifierData StatResult;

    private void Awake()
    {
        Init();
    }


    private void OnEnable()
    {
        UpdateCharacterStatResult();
    }

    void Init()
    {
        //init dictionary
        for(int i = 0; i < equipSlots.Length; i++)
        {
            equipSlotDic[equipSlots[i].equipmentType.ToString()] = equipSlots[i];
        }

        UpdateCharacterStatResult();
    }



    // EquipmentManager.cs - UpdateCharacterStatResult() 수정 부분

    public void UpdateCharacterStatResult()
    {
        // 장비 슬롯 전체를 순회하며 합산값 계산 (기존 로직 동일)
        StatResult = new StatModifierData();
        for (int i = 0; i < equipSlots.Length; i++)
        {
            StatResult.MaxHp += equipSlotDic[equipSlots[i].equipmentType.ToString()].equipmentStat.MaxHp;
            StatResult.Damage += equipSlotDic[equipSlots[i].equipmentType.ToString()].equipmentStat.Damage;
            StatResult.Defense += equipSlotDic[equipSlots[i].equipmentType.ToString()].equipmentStat.Defense;
            StatResult.MoveSpeed += equipSlotDic[equipSlots[i].equipmentType.ToString()].equipmentStat.MoveSpeed;
        }

        //ModifierContainer에 Equipment 소스로 등록
        StatViewTarget.GetModifierContainer().SetModifier(StatModifierSource.Equipment, StatResult);

        CharacterStatUpdateForInfo();
    }

    void CharacterStatUpdateForInfo()
    {
        //장비창 내 텍스트 변화용
        statTexts[(int)StatInfo.MaxHP].text = StatViewTarget.GetResultMaxHp().ToString();
        statTexts[(int)StatInfo.Damage].text = StatViewTarget.GetResultDamage().ToString();
        statTexts[(int)StatInfo.Defense].text = StatViewTarget.GetResultDefense().ToString();
        statTexts[(int)StatInfo.MoveSpeed].text = StatViewTarget.GetResultMoveSpeed().ToString();

        //캐릭터의 체력바 업데이트용
        StatViewTarget.onTransStatData?.Invoke();
    }
   
}
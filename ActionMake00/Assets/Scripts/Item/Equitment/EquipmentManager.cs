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
    public TextMeshProUGUI[] statTexts;
    public Inventory inven;

    //안쓸 수도 있음
    public Dictionary<string, EquipmentSlot> equipSlotDic = 
        new Dictionary<string, EquipmentSlot>();

    public AddStatData StatResult;

    private void Awake()
    {
        Init();

    }

    void Start()
    {
        
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


    
    public void UpdateCharacterStatResult()
    {
        StatResult = new AddStatData(); //초기화

        for (int i = 0; i < equipSlots.Length; i++)
        {
            StatResult.MaxHp += equipSlotDic[equipSlots[i].equipmentType.ToString()].equipmentStat.MaxHp;
            StatResult.Damage += equipSlotDic[equipSlots[i].equipmentType.ToString()].equipmentStat.Damage;
            StatResult.Defense += equipSlotDic[equipSlots[i].equipmentType.ToString()].equipmentStat.Defense;
            StatResult.MoveSpeed += equipSlotDic[equipSlots[i].equipmentType.ToString()].equipmentStat.MoveSpeed;
        }
        StatViewTarget.statDatas[(int)AddStatName.Equit] = StatResult;
        /*StatViewTarget.SetMaxHp(StatViewTarget.GetResultMaxHp());
        StatViewTarget.SetDamage(StatViewTarget.GetResultDamage());
        StatViewTarget.SetMoveSpeed(StatViewTarget.GetResultMoveSpeed());
        StatViewTarget.SetDefense(StatViewTarget.GetResultMaxHp());*/

        CharacterStatUpdateForInfo();
    }

    void CharacterStatUpdateForInfo()
    {
        /*StatViewTarget.SetMaxHp(StatViewTarget.GetResultMaxHp());
        StatViewTarget.SetDamage(StatViewTarget.GetResultDamage());
        StatViewTarget.SetMoveSpeed(StatViewTarget.GetResultMoveSpeed());
        StatViewTarget.SetDefense(StatViewTarget.GetResultMaxHp());*/

        statTexts[(int)StatInfo.MaxHP].text = "MAXHP\n" + StatViewTarget.GetResultMaxHp().ToString();
        statTexts[(int)StatInfo.Damage].text = "DAMAGE\n" + StatViewTarget.GetResultDamage().ToString();
        statTexts[(int)StatInfo.Defense].text = "DEFENSE\n" + StatViewTarget.GetResultDefense().ToString();
        statTexts[(int)StatInfo.MoveSpeed].text = "SPEED\n" + StatViewTarget.GetResultMoveSpeed().ToString();
    }

    
}

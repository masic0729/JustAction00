using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentManager : MonoBehaviour
{
    public EquipmentSlot[] equipSlots;
    [SerializeField] Character StatViewTarget;                                           //캐릭터 능력치를 노출하려는 대상
    public TextMeshProUGUI[] statTexts;
    public Inventory inven;

    //안쓸 수도 있음
    public Dictionary<string, EquipmentSlot> equipSlotDic = 
        new Dictionary<string, EquipmentSlot>();

    public AddStatData StatResult;
    
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void OnEnable()
    {
        statTexts[0].text = "MAXHP\n" + StatViewTarget.GetResultMaxHp().ToString();
        statTexts[1].text = "DAMAGE\n" + StatViewTarget.GetResultDamage().ToString();
        statTexts[2].text = "DEFENSE\n" + StatViewTarget.GetResultDefense().ToString();
        statTexts[3].text = "SPEED\n" + StatViewTarget.GetResultMoveSpeed().ToString();
    }

    void Init()
    {
        //init dictionary
        for(int i = 0; i < equipSlots.Length; i++)
        {
            equipSlotDic[equipSlots[i].equipmentType.ToString()] = equipSlots[i];
        }

        
    }


    
    public void UpdateCharacterStatResult()
    {
        StatResult = new AddStatData(); //초기화

        for (int i = 0; i < equipSlots.Length; i++)
        {
            StatResult.maxHp += equipSlotDic[equipSlots[i].equipmentType.ToString()].equipmentStat.maxHp;
            StatResult.damage += equipSlotDic[equipSlots[i].equipmentType.ToString()].equipmentStat.damage;
            StatResult.defense += equipSlotDic[equipSlots[i].equipmentType.ToString()].equipmentStat.defense;
            StatResult.moveSpeed += equipSlotDic[equipSlots[i].equipmentType.ToString()].equipmentStat.moveSpeed;
        }
        StatViewTarget.statDatas[(int)AddStatName.Equit] = StatResult;
        CharacterStatUpdateForInfo();
    }

    void CharacterStatUpdateForInfo()
    {
        /*for (int i = 0; i < 4; i++)
        {
            statTexts[i].text = StatViewTarget.GetResultDamage().ToString();
        }*/
        statTexts[0].text = "MAXHP\n" + StatViewTarget.GetResultMaxHp().ToString();
        statTexts[1].text = "DAMAGE\n" + StatViewTarget.GetResultDamage().ToString();
        statTexts[2].text = "DEFENSE\n" + StatViewTarget.GetResultDefense().ToString();
        statTexts[3].text = "SPEED\n" + StatViewTarget.GetResultMoveSpeed().ToString();
    }

}

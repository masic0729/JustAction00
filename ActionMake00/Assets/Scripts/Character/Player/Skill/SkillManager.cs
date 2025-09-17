using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    Animator anim;
    public Dictionary<string, Action> swordSkillDic;
    public Dictionary<string, Action> staffSkillDic;

    [SerializeField] PlayerSkillData[] weaponData;
    
    Dictionary<string, float> swordSkillCooltimeDic;
    Dictionary<string, float> staffSkillCooltimeDic;

    Dictionary<string, float> swordSkillCoolListDic;
    Dictionary<string, float> staffSkillCoolListDic;

    Character character;

    string currentWeaponType;

    private void Awake()
    {
        swordSkillDic = new Dictionary<string, Action>();
        staffSkillDic = new Dictionary<string, Action>();
        
        swordSkillCoolListDic = new Dictionary<string, float>();
        staffSkillCoolListDic = new Dictionary<string, float>();
        
        swordSkillCooltimeDic = new Dictionary<string, float>();
        staffSkillCooltimeDic = new Dictionary<string, float>();
        
        

        character = GetComponent<Character>();
    }

    private void Start()
    {
        Init();
    }

    void Update()
    {
        SkillCollTimer();

    }

    void Init()
    {
        anim = GetComponent<Animator>();
        
        //초기 모든 스킬의 쿨타임 값을 0으로 초기화한다.
        for(int i = 0; i < weaponData.Length;i++)
        {
            for(int j = 0; j < weaponData[i].weaponSkillBase.Length; j++)
            {
                string skillTriggerName;
                skillTriggerName = weaponData[i].weaponSkillBase[j].triggerName;

                float coolTime = weaponData[i].weaponSkillBase[j].coolTime;

                if (weaponData[i].weaponType == "Sword")
                {
                    swordSkillCooltimeDic[skillTriggerName] = 0;
                    swordSkillCoolListDic[skillTriggerName] = coolTime;
                    //Debug.Log(swordSkillCoolListDic[skillTriggerName] + "히히 사실 이래");
                }

                if (weaponData[i].weaponType == "Staff")
                {
                    staffSkillCooltimeDic[skillTriggerName] = 0;
                    staffSkillCoolListDic[skillTriggerName] = coolTime;

                }
            }
        }



    }

    /// <summary>
    /// 플레이어의 무기에 따른 스킬구조를 저장한다.
    /// 애니메이션 트리거와 스킬 딕셔너리이름은 똑같으며,
    /// 무기에 따른 스킬 여부는 currentWeaponType에 따라 다르다.
    /// </summary>
    /// <param name="type">스킬을 저장하려는 플레이어 무기 따입</param>
    /// <param name="data">각 스킬 명령에 저장할 기능들</param>
    public void SetSkillDic(WeaponType type, Action data, int index)
    {
        /*Debug.Log(data.Count + "리스트 개수");
        for(int i = 0; i < data.Count; i++)
        {
            string skillKey = "Skill" + i.ToString();
            if (type == WeaponType.Sword)
            {
                swordSkillDic[skillKey] = data[i];
                swordSkillDic[skillKey] += character.WeaponColDisable;
            }
            else if(type == WeaponType.Staff)
            {
                staffSkillDic[skillKey] = data[i];
                staffSkillDic[skillKey] += character.WeaponColDisable;
            }
        }*/

        string skillKey = "Skill" + index.ToString();
        if(type == WeaponType.Sword)
        {
            swordSkillDic[skillKey] = data;
            swordSkillDic[skillKey] += character.WeaponColDisable;
        }
        else
        {
            staffSkillDic[skillKey] = data;
            staffSkillDic[skillKey] += character.WeaponColDisable;
        }
    }

    /// <summary>
    /// 기본적으로 현재 무기타입 내에 스킬을 사용하기 때문에
    /// 현재 무기 타입의 해당 딕셔너리인덱스 스킬 쿨타임 설정 및 
    /// 시작을 한다
    /// 
    /// 해당 코드는 매 프레임마다 스킬 쿨타임을 내릴려고 한다
    /// </summary>
    public void SkillCollTimer()
    {
        for (int i = 0; i < weaponData.Length; i++)
        {
            for (int j = 0; j < weaponData[i].weaponSkillBase.Length; j++)
            {
                string skillTriggerName;
                skillTriggerName = weaponData[i].weaponSkillBase[j].triggerName;

                if (weaponData[i].weaponType == "Sword")
                {
                    if (swordSkillCooltimeDic[skillTriggerName] > 0)
                    {
                        swordSkillCooltimeDic[skillTriggerName] -= Time.deltaTime;
                        Debug.Log("감소중");
                    }

                    if (swordSkillCooltimeDic[skillTriggerName] <= 0)
                    {
                        swordSkillCooltimeDic[skillTriggerName] = 0;
                    }
                }

                if (weaponData[i].weaponType == "Staff")
                {
                    if(staffSkillCooltimeDic[skillTriggerName] > 0)
                        staffSkillCooltimeDic[skillTriggerName] -= Time.deltaTime;

                    if (staffSkillCooltimeDic[skillTriggerName] <= 0)
                    {
                        staffSkillCooltimeDic[skillTriggerName] = 0;
                    }
                }
            }
        }

    }

    void SkillCoolTimeStart(int key)
    {
        string skillKey = "Skill" + key.ToString();
        switch (currentWeaponType)
        {
            case "Sword":
                swordSkillCooltimeDic[skillKey] = swordSkillCoolListDic[skillKey];
                break;

            case "Staff":
                staffSkillCooltimeDic[skillKey] = staffSkillCoolListDic[skillKey];

                break;
            default:
                Debug.Log("스킬 쿨 예외 발생");
                break;
        }

    }


    public bool isSkillCanUse(string skillKey)
    {
        switch (currentWeaponType)
        {
            case "Sword":
                if (swordSkillCooltimeDic[skillKey] > 0)
                    return false;
                break;

            case "Staff":
                if (staffSkillCooltimeDic[skillKey] > 0)
                    return false;
                break;
            default:
                Debug.Log("스킬 쿨 예외 발생");
                break;
        }
        return true;
    }

    /// <summary>
    /// 저장된 스킬을 기반으로 플레이어가 스킬을 사용한다
    /// </summary>
    /// <param name="key">사용되는 스킬트리거명. 애니메이터의 트리거과 동일한 값이다</param>
    public void UseSkill(int key)
    {
        string skillKey = "Skill" + key.ToString();
        switch(currentWeaponType)
        {
            case "Sword":
                if (swordSkillCooltimeDic[skillKey] <= 0)
                {
                    swordSkillDic[skillKey]();
                    SkillCoolTimeStart(key);
                }
                break;
            case "Staff":
                if (staffSkillCooltimeDic[skillKey] <= 0)
                {
                    staffSkillDic[skillKey]();
                    SkillCoolTimeStart(key);
                }
                break;
            default:
                Debug.Log("스킬 발동 중 예외 발생");
                break;
        }
    }

    public void SetCurrentWeaponType(string typeName) => currentWeaponType = typeName;

    public string GetCurrentWeaponType() => currentWeaponType;
}

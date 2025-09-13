using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    Animator anim;
    public Dictionary<string, Action> swordSkillDic;
    public Dictionary<string, Action> staffSkillDic;
    Character character;
    string currentWeaponType;

    private void Awake()
    {
        swordSkillDic = new Dictionary<string, Action>();
        staffSkillDic = new Dictionary<string, Action>();
        character = GetComponent<Character>();
    }

    private void Start()
    {
        Init();
    }

    void Init()
    {
        anim = GetComponent<Animator>();
        
    }

    public void SetSkillDic(WeaponType type, List<Action> data)
    {
        Debug.Log(data.Count + "리스트 개수");
        for(int i = 0; i < data.Count; i++)
        {
            string skillNumDic = "Skill" + i.ToString();
            if (type == WeaponType.Sword)
            {
                swordSkillDic[skillNumDic] = data[i];
                swordSkillDic[skillNumDic] += character.WeaponColDisable;
                
            }
            else if(type == WeaponType.Staff)
            {
                staffSkillDic[skillNumDic] = data[i];
                staffSkillDic[skillNumDic] += character.WeaponColDisable;
            }
        }
    }

    public void UseSkill(string skillNum)
    {
        switch(currentWeaponType)
        {
            case "Sword":
                swordSkillDic["Skill" + skillNum]();
                break;
            case "Staff":
                swordSkillDic["Skill" + skillNum]();
                break;
        }
    }

    public void SetCurrentWeaponType(string typeName) => currentWeaponType = typeName;

    public string GetCurrentWeaponType() => currentWeaponType;
}

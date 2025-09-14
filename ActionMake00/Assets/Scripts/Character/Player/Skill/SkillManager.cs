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

    /// <summary>
    /// 플레이어의 무기에 따른 스킬구조를 저장한다.
    /// 애니메이션 트리거와 스킬 딕셔너리이름은 똑같으며,
    /// 무기에 따른 스킬 여부는 currentWeaponType에 따라 다르다.
    /// </summary>
    /// <param name="type">스킬을 저장하려는 플레이어 무기 따입</param>
    /// <param name="data">각 스킬 명령에 저장할 기능들</param>
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

    /// <summary>
    /// 저장된 스킬을 기반으로 플레이어가 스킬을 사용한다
    /// </summary>
    /// <param name="skillNum"></param>
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

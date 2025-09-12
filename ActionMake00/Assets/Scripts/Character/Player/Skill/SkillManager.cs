using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    Animator anim;
    public Dictionary<string, Action> swordSkillDic;
    public Dictionary<string, Action> staffSkillDic;

    private void Awake()
    {
        swordSkillDic = new Dictionary<string, Action>();
        staffSkillDic = new Dictionary<string, Action>();
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
            string skillNameDic = "Skill" + i.ToString();
            if (type == WeaponType.Sword)
            {
                swordSkillDic[skillNameDic] = data[i];
            }
            else if(type == WeaponType.Staff)
            {
                staffSkillDic[skillNameDic] = data[i];
            }
        }


    }
}

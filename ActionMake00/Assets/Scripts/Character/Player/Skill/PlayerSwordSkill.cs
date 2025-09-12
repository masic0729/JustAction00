using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerSwordSkill : MonoBehaviour
{
    SkillManager skillManager;
    List<Action> skill;

    private void Start()
    {
        Init();
    }

    void Init()
    {
        skillManager = gameObject.GetComponent<SkillManager>();
        skill = new List<Action>();

        skill.Add(Skill0);
        skillManager.SetSkillDic(WeaponType.Sword, skill);
    }

    public void Skill0()
    {
        Debug.Log("SwordSkill0");
    }
}

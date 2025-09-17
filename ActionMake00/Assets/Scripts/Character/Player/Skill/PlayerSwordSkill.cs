using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSwordSkill : MonoBehaviour
{
    SkillManager skillManager;
    public GameObject testObject;
    Player player;
    protected Weapon weapon;

    private void Start()
    {
        Init();
    }

    void Init()
    {
        skillManager = transform.parent.GetComponent<SkillManager>();
        player = GetComponent<Player>();

        skillManager.SetSkillDic(WeaponType.Sword, new List<Action>() { Skill0 });
        
    }

    public void Skill0()
    {
        Debug.Log("SwordSkill0");
        Instantiate(testObject, player.weaponDic["PlayerWeapon"].transform.position, player.weaponDic["PlayerWeapon"].transform.rotation);   
    }
}

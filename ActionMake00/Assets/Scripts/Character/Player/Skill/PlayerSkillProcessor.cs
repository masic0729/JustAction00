using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillProcessor : MonoBehaviour
{
    protected SkillManager skillManager;

    public PlayerWeaponSkill[] skills;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void InitSkill()
    {
        skillManager = transform.parent.GetComponent<SkillManager>();

        for (int i = 0; i < skills.Length; i++)
        {
            skillManager.SetSkillDic(WeaponType.Sword, skills[i].SkillUse, i);
        }
    }
}

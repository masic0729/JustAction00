using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordParring : PlayerSkillInfo
{
    float buffTime = 10f;
    protected override void Start()
    {
        base.Start();
        Init();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Init()
    {
        base.Init();
        hitLevel = 1;

        BuffBase addDamageBuff = new BuffTransDamage();
        GameObject ins =  addDamageBuff.ObjectSetup(owner, 10, buffTime, "DamageUpEffect", null);
        ins.transform.Translate(0, 1, 0);
        buffs.Add(addDamageBuff);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (!other.TryGetComponent<Character>(out Character hitTarget))
            return;

        if(hitTarget.transform.tag == target)
        {
            hitTarget.anim.SetTrigger("GetParring");

        }

    }
}

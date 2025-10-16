using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordParring : PlayerSkillInfo
{

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
        addDamageBuff.Setup(owner, 10, 10f);
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

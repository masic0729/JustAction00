using DG.Tweening;
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
        DOTween.Complete("NORMALZOOM");
        DOTween.Restart("HARDZOOM");
    }

    protected override void Awake()
    {
        base.Awake();

        BuffBase addDamageBuff = new BuffTransDamage(10f, "DamageUpEffect", null, BuffType.Buff, 5f);
        ownerBuffs.Add(addDamageBuff);
    }

    protected override void Init()
    {
        base.Init();
        hitLevel = 1;
        isParringAttack = true;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}

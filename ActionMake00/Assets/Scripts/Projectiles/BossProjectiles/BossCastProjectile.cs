using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCastProjectile : BossProjectile
{
    protected override void Awake()
    {
        base.Awake();
        BuffBase stunDeBuff = new BuffGetStun(2f, "StunEffect", null, BuffType.Debuff);
        targetBuffs.Add(stunDeBuff);
    }
    protected override void Start()
    {
        base.Start();
        Init();
        
    }


    protected override void Init()
    {
        base.Init();
        hitLevel = -1;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}

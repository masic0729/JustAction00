using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCastProjectile : BossProjectile
{

    protected override void Start()
    {
        base.Start();
    }


    protected override void Init()
    {
        base.Init();
        hitLevel = -1;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        BuffBase addDamageBuff = new BuffGetStun();

        if (other.transform.tag == target)
        {
            float buffTime = 3f;

            Character target = other.GetComponent<Character>();
            addDamageBuff.ObjectSetup(target, buffTime, "StunEffect", null);
        }

    }
}

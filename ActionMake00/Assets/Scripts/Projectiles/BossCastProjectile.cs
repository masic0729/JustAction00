using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCastProjectile : Projectile
{

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Init()
    {
        base.Init();
        target = "Player";
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}

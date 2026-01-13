using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectile : EnemyProjectile
{
    protected int playerLayer = 1 << 6;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Init()
    {
        base.Init();
        
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}

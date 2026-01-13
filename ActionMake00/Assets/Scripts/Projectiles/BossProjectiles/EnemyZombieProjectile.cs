using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyZombieProjectile : EnemyProjectile
{
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        Init();

    }


    protected override void Init()
    {
        base.Init();
        hitLevel = 0;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        Debug.Log("아니 이게 맞아?");
    }
}

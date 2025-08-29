using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Attacker
{
    [SerializeField]protected float moveSpeed;
    [SerializeField] protected float destroyTime = 5f;

    protected virtual void Start()
    {
        Init();
    }

    protected virtual void Update()
    {
        ProjectileMove();
    }

    protected override void Init()
    {
        base.Init();
        hitLevel = 0;
        Destroy(gameObject, destroyTime);
    }

    protected void ProjectileMove()
    {
        transform.Translate(0, 0, moveSpeed * Time.deltaTime);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGolem : BossEnemyBT
{
    Weapon Stone;
    protected override void Start()
    {
        base.Start();
        Init();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Init()
    {
        base.Init();
        playerFindDistance = 10f;
        activityAllowValue = 20f;
        attackReadyDistance = 8f;
        punchDistance = 3.5f;
        weapon = FindTransformAtChild("PunchWeapon").GetComponent<Weapon>();
    }


    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    public void SpawnStone()
    {
        Transform stoneSpawn = FindTransformAtChild("StoneSpawner");

    }

    public void TurnOff()
    {
        isCanTurn = false;
    }
}

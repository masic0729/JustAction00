using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTest : FollwingPlayerEnemyBT
{
    //private int aiStateIndex = 0; // 0: 추적, 1: 대기

    protected override void Start()
    {
        base.Start();
        StartCoroutine(SwitchStateRoutine());
    }

    IEnumerator SwitchStateRoutine()
    {
        while (true)
        {
            index = 0; // 추적
            yield return new WaitForSeconds(2f);
            index = 1; // 대기
            yield return new WaitForSeconds(2f);
        }
    }
}

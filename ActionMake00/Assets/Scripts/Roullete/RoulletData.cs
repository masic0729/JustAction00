using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoulletData : MonoBehaviour
{
    //룰렛을 작동할 때 목표하려는 회전 값
    public float targetRotate = 0f;

    [Header("룰렛내 데이터(칸) 개수")]
    public int roulletBlockCount;

    //룰렛에 존재하는 점수값에 대한 정보
    public int[] roulletScoreData;

    [Header("룰렛을 회전하려는 잔여 횟수")]
    public int remainPlayCount = 3;

}

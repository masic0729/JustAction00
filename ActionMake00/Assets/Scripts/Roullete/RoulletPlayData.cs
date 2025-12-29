using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoulletPlayData : MonoBehaviour
{
    //룰렛을 작동할 때 목표하려는 회전 값
    public float targetRotate = 0f;

    [Header("룰렛내 데이터(칸) 개수")]
    public int roulletBlockCount;

    //룰렛에 존재하는 점수값에 대한 정보
    public int[] roulletScoreData;

    [Header("플레이어에게 제공하려는 아이템")]
    public ItemObject[] items;

    [Header("룰렛을 회전하려는 잔여 횟수")]
    public int remainPlayCount = 3;

    [Header("현재 룰렛을 통한 최종 점수. 3회 기준 최대 1500점이 한계이다")]
    public int currentScore = 0;

    [Header("아이템을 받기 위한 최소 요구 점수")]
    public int[] needScores;

    //아이템 제공을 위한 결과 인덱스. 구간 마다의 목표 점수를 도달할 때 마다 인덱스 값이 상승한다
    public int currentItemResultIndex = 0;

    public readonly  int maxScore = 1500;
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSkillBase
{
    public Sprite icon;                         //스킬의 아이콘
    public string skillName;                    //스킬명
    //public MonoBehaviour testScript;
    //public GameObject skillPrefab;              //스킬 프리팹
    //public ParticleSystem particleSystem;       //스킬 사용시 사용할 수 있는 파티클 시스템

    public string description;                  //스킬 설명
    public int coolTime;                        //스킬 쿨
    public string triggerName;                  //애니메이션 및 함수 트리거명
}

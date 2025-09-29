using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    int transDamage = 0;
    float transMoveSpeed = 0;
    int transDamageDefenceIgnore = 0;
    //List<>                                                    //이곳에 버프데이터들을 관리하는 리스트를 만들어서 추후에 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public int GetTransDamage()
    {
        return transDamage;
    }

    public float GetTransMoveSpeed()
    {
        return transMoveSpeed;
    }

    public int GetTransDamageDefenseIgnore()
    {
        return transDamageDefenceIgnore;
    }
}

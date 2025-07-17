using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Character : MonoBehaviour
{
    protected float hp;

    // Start is called before the first frame update
    virtual protected void Start()
    {
        
    }

    // Update is called once per frame
    virtual protected void Update()
    {
        
    }

    public float GetHp() => hp;
    public void SetHp(float value) => hp = value;
    public void TakeDamage(float amount)
    {
        if (hp - amount < 0)
            hp = 0;
        else
            hp -= amount;
    }

}

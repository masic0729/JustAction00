using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    int damage = 1;


    public void SetDamage(int value) => damage = value;
    public int GetDamage() => damage;
}

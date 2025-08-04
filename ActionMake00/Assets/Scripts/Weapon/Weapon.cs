using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IWeapon
{
    protected int damage = 1;

    public int GetDamage() => damage;

    public void SetDamage(int value) => damage = value;
}

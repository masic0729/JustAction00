using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackerType
{
    Weapon = 0,
    Projectile
}

public class Attacker : MonoBehaviour, IAttacker
{
    [SerializeField] protected AttackerType attackType;
    [SerializeField]protected Collider weaponCol;
    protected int damage = 1;
    protected string target;
    protected int hitLevel = -1;
    protected string tagName;

    protected virtual void Start()
    {
        //Init();
    }

    protected virtual void Init()
    {
        weaponCol = GetComponent<Collider>();
        if (weaponCol == null || attackType == AttackerType.Projectile)
        {
            return;
        }
        //transform.tag = "WeaponOff";
        ColliderTransEnable();
        weaponCol.enabled = false; // Ω√¿€¿∫ ≤®µŒ±‚
    }

    public void ColliderTransEnable()
    {
        /*if(gameObject.transform.tag != "WeaponOff")
        {
            transform.tag = tagName;
        }
        else
        {
            this.gameObject.transform.tag = "WeaponOff";
        }*/
        if (weaponCol == null)
        {
            Debug.Log("∞¡ æ¯¿Ω");
            return;
        }
        if (weaponCol.enabled == true)
        {
            weaponCol.enabled = false;
        }
        else if (weaponCol.enabled == false)
        {
            weaponCol.enabled = true;
        }
        Debug.Log("Collider Transed");
    }

    virtual protected void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == target && this.gameObject.tag != "WeaponOff")
        {
            other.GetComponent<Character>().TakeDamage(damage, hitLevel);
            if (attackType == AttackerType.Projectile)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public int GetDamage() => damage;
    public void SetDamage(int value) => damage = value;
}
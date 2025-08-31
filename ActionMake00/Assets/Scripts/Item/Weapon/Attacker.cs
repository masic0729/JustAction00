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
    [SerializeField] protected AttackerType type;
    protected Collider weaponcol;
    protected int damage = 1;
    protected string target;
    protected int hitLevel = -1;

    private void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        weaponcol = GetComponent<Collider>();
        if (weaponcol == null || type == AttackerType.Projectile)
        {
            return;
        }
        Debug.Log(weaponcol);

        weaponcol.enabled = false; // 시작은 꺼두기
    }

    

    virtual protected void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == target)
        {
            other.GetComponent<Character>().TakeDamage(damage, hitLevel);
            if (type == AttackerType.Projectile)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public int GetDamage() => damage;
    public void SetDamage(int value) => damage = value;
}
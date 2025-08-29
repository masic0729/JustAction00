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
    protected Collider col;
    protected int damage = 1;
    protected string target;
    protected int hitLevel = -1;

    private void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        col = GetComponent<Collider>();
        if (col == null || type == AttackerType.Projectile)
        {
            return;
        }
        col.enabled = false; // 시작은 꺼두기
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
using UnityEngine;

public class Weapon : Attacker, IAttacker
{
    

    private void Start()
    {
        Init();
    }
    protected override void Init()
    {
        base.Init();
    }

    public void ColliderTransEnable()
    {
        if (weaponcol == null) return;
        if (weaponcol.enabled == true)
        {
            weaponcol.enabled = false;
        }
        else
            weaponcol.enabled = true;
    }

    /*virtual protected void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == target)
        {
            other.GetComponent<Character>().TakeDamage(damage, hitLevel);
        }
    }

    public int GetDamage() => damage;
    public void SetDamage(int value) => damage = value;*/
}

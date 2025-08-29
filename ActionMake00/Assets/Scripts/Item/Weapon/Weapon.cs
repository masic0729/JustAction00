using UnityEngine;

public class Weapon : Attacker, IAttacker
{
    /*private Collider col;
    protected int damage = 1;
    protected string target;
    protected int hitLevel = -1;*/

    private void Start()
    {
        Init();
    }
    protected virtual void Init()
    {
        base.Init();
    }

    public void ColliderTransEnable()
    {
        if (col == null) return;
        if (col.enabled == true)
        {
            col.enabled = false;
        }
        else
            col.enabled = true;
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

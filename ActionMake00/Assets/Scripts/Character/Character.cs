using UnityEngine;

public class Character : MonoBehaviour, ICharacterDamageable
{
    public Animator anim;
    protected int hp { get; set; }
    protected int damage { get; set; }

    // Start is called before the first frame update
    virtual protected void Start()
    {
        Init();
    }

    // Update is called once per frame
    virtual protected void Update()
    {
        
    }

    virtual protected void Init()
    {
        //base Init
        hp = 10;
        damage = 1;
        anim = GetComponent<Animator>();
    }

    public float GetHp() => hp;
    public void SetHp(int value) => hp = value;

    public virtual void TakeDamage(int amount)
    {
        if (hp - amount < 0)
            hp = 0;
        else
            hp -= amount;
    }

    public virtual void Dead()
    {

    }
}

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Sword;

public class Character : MonoBehaviour, ICharacterDamageable
{
    public Animator anim;
    [SerializeField]
    public ParticleSystem[] pEffect;
    protected Dictionary<string, ParticleSystem> pEffectDic;
    CapsuleCollider col;
    protected Rigidbody rb;
    public Weapon weapon;

    protected int hp { get; set; }
    protected int commonDamage;
    protected int skillDamage;                                             //얘는 보스몬스터 한정으로 정의될 가능성이 높음


    [SerializeField] protected float moveSpeed = 5f;

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
        commonDamage = 1;
        anim = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        col.enabled = true;
        pEffectDic = new Dictionary<string, ParticleSystem>();
        for(int i = 0; i < pEffect.Length; i++)
        {
            pEffectDic[pEffect[i].gameObject.name] = pEffect[i];
        }
    }

    public float GetHp() => hp;
    public void SetHp(int value) => hp = value;

    public virtual void TakeDamage(int amount)
    {
        if (hp - amount < 0)
            hp = 0;
        else
            hp -= amount;

        if(hp > 0)
        {
            //이곳에 일반 피격 효과 처리
            anim.SetTrigger("Hit");

        }
        else if ( hp <= 0)
        {
            //Dead();                         //이 부분은 이벤트/액션 처리할 것
            anim.SetTrigger("Death");

        }
    }

    /// <summary>
    /// void
    /// </summary>
    public virtual void Dead(float animationTime) {
        rb.useGravity = false;
        col.enabled = false;
        Destroy(this.gameObject, animationTime);
    }

    public void TransHitBox(int state)
    {
        //weapon.GetComponent<BoxCollider>().enabled = true;
        weapon.ColliderEnable(state);
    }

    protected Transform FindTransformAtChild(string name)
    {
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.name == name) return t;
        }
        Debug.LogWarning("Child transform not found: " + name);
        return null;
    }

    public void SetMoveSpeed(float value) => moveSpeed = value;

    public float GetMoveSpeed() => moveSpeed;

    public int GetCommonDamage() => commonDamage;

    public void SetCommondamage(int value) => commonDamage = value;

    public int GetSkillDamage() => skillDamage;

    public void SetSkillDamage(int value) => skillDamage = value;
}

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Character : MonoBehaviour, ICharacterDamageable
{
    [HideInInspector]public Animator anim;

    //피격 및 사망 시 발생하는 액션류
    public Action hitAction;
    public Action deathAction;

    [SerializeField]
    public ParticleSystem[] pEffect;                                        //해당 데이터는 풀 매니저에 의해 없어질 가능성이 높음
    protected Dictionary<string, ParticleSystem> pEffectDic;                //해당 데이터는 풀 매니저에 의해 없어질 가능성이 높음
    Collider hitCol;
    protected Rigidbody rb;
    public Weapon[] weapon;
    protected Weapon currentWeapon;
    public Dictionary<string, Weapon> weaponDic;

    [Header("캐릭터 스킬 발사체")]
    public GameObject[] skillProjectiles;


    //캐릭터 능력치 관련 데이터
    #region
    [SerializeField] protected int hp;
    protected int damage;                                                   //기본 데미지 데이터
    protected int skillDamage;                                             //얘는 보스몬스터 한정으로 정의될 가능성이 높음
    protected float rotateSpeed;                                            //회전 속도

    [SerializeField]protected bool isSuperArmor = false;                    //피격이상 면역 유무. 활성화 시 경직이 없다.
    [SerializeField]protected bool isIgnoreDamage = false;                  //무적 유무. 활성화 시 피해를 입지 않는다.


    [SerializeField] protected float moveSpeed = 5f;
    #endregion

    // Start is called before the first frame update
    virtual protected void Start()
    {
        //Init();
    }

    // Update is called once per frame
    virtual protected void Update()
    {
        
    }

    virtual protected void Init()
    {
        //base Init
        hp = 20;
        damage = 1;
        anim = GetComponent<Animator>();
        hitCol = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        hitCol.enabled = true;
        DictionaryInit();

    }

    void DictionaryInit()
    {
        pEffectDic = new Dictionary<string, ParticleSystem>();
        weaponDic = new Dictionary<string, Weapon>();
        for (int i = 0; i < weapon.Length; i++)
        {
            weaponDic[weapon[i].name] = weapon[i];

        }
        for(int i = 0; i < pEffect.Length; i++)
        {
            pEffectDic[pEffect[i].name] = pEffect[i];
        }
    }

    public float GetHp() => hp;
    public void SetHp(int value) => hp = value;

    public virtual void TakeDamage(int amount, int hitLevel = -1)
    {
        if (isIgnoreDamage == true)
        {
            return;
        }

        if (hp - amount < 0)
            hp = 0;
        else
            hp -= amount;

        if (hp <= 0)
        {
            //Dead();                         //이 부분은 이벤트/액션 처리할 것
            anim.SetTrigger("Death");

        }

        if (isSuperArmor == false || hitLevel != -1)
        {
            anim.SetInteger("HitLevel", hitLevel);
        }
    }

        

    /// <summary>
    /// void
    /// </summary>
    public virtual void Dead(float animationTime) {
        //ParticleManager.instance.PlayParticle(pEffectDic["pDeath"], this.transform);
        PoolManager.instance.Spawn("pDeath", this.transform.position, transform.rotation);
        rb.useGravity = false;
        hitCol.enabled = false;
        Destroy(this.gameObject, animationTime);
    }

    public void TransHitBox(string name)
    {
        weaponDic[name].ColliderTransEnable();
        
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

    public int GetCommonDamage() => damage;

    public void SetCommondamage(int value) => damage = value;

    public int GetSkillDamage() => skillDamage;

    public void SetSkillDamage(int value) => skillDamage = value;

    public void SetIsSuperArmor(bool state) => isSuperArmor = state;
    public bool GetIsSuperArmor() => isSuperArmor;

    public void SetIsIgnoreDamage(bool state) => isIgnoreDamage = state;
    public bool GetIsIgnoreDamage() => isIgnoreDamage;

    public float GetRotateSpeed() => rotateSpeed;
    public void SetRotateSpeed(float value) => rotateSpeed = value;



}

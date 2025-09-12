using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static PlayerSword;

public class Character : MonoBehaviour, ICharacterDamageable
{
    [HideInInspector]public Animator anim;
    [SerializeField]
    public ParticleSystem[] pEffect;
    protected Dictionary<string, ParticleSystem> pEffectDic;
    Collider hitCol;
    protected Rigidbody rb;
    public Weapon[] weapon;
    protected Weapon currentWeapon;
    protected Dictionary<string, Weapon> weaponDic;

    [Header("캐릭터 스킬 발사체")]
    public GameObject[] skillProjectiles;
    protected float rotateSpeed;

    [SerializeField] protected int hp;
    protected int commonDamage;
    protected int skillDamage;                                             //얘는 보스몬스터 한정으로 정의될 가능성이 높음

    [SerializeField]protected bool isSuperArmor = false;
    [SerializeField]protected bool isIgnoreDamage = false;


    [SerializeField] protected float moveSpeed = 5f;

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
        commonDamage = 1;
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
            anim.SetTrigger("Hit");
        }
        
        
    }

    /// <summary>
    /// void
    /// </summary>
    public virtual void Dead(float animationTime) {
        ParticleManager.instance.PlayParticle(pEffectDic["pDeath"], this.transform);
        //SpawnManager.instance.DestroyCommonEnemy(this.gameObject.GetComponent<Enemy>());
        rb.useGravity = false;
        hitCol.enabled = false;
        Destroy(this.gameObject, animationTime);
    }

    public void TransHitBox(string name)
    {
        //weapon.GetComponent<BoxCollider>().enabled = true;
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

    public int GetCommonDamage() => commonDamage;

    public void SetCommondamage(int value) => commonDamage = value;

    public int GetSkillDamage() => skillDamage;

    public void SetSkillDamage(int value) => skillDamage = value;

    public void SetIsSuperArmor(bool state) => isSuperArmor = state;
    public bool GetIsSuperArmor() => isSuperArmor;

    public void SetIsIgnoreDamage(bool state) => isIgnoreDamage = state;
    public bool GetIsIgnoreDamage() => isIgnoreDamage;

    public float GetRotateSpeed() => rotateSpeed;
    public void SetRotateSpeed(float value) => rotateSpeed = value;



}

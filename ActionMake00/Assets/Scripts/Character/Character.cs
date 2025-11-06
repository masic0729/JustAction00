using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.Experimental.Rendering;


/// <summary>
/// 버프 및 장비에 의한 스탯 변화에 대한 능력치 조정을 해당 데이터로 관리한다
/// </summary>

[System.Serializable]
public struct AddStatData
{
    public float MaxHp;
    public float Damage;
    public float Defense;
    public float MoveSpeed;

}
public enum StatTypeToIndex
{
    MaxHP = 0,
    Damage = 1,
    Defense = 2,
    MoveSpeed = 3
}

public enum AddStatName
{
    Buff = 0,
    Equit = 1
}

public class Character : MonoBehaviour, ICharacterDamageable
{
    public AddStatData[] statDatas;


    [SerializeField] CharacterStatData characterStatData;

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
    public Dictionary<string, Weapon> weaponDic = new Dictionary<string, Weapon>();


    [Header("캐릭터 스킬 발사체")]
    public GameObject[] skillProjectiles;

    protected int exp = 1;                                                  //몬스터가 사망 시 그 대상에게 주는 경험치


    //캐릭터 능력치 관련 데이터
    #region
    protected float skillDamage;                                             //얘는 보스몬스터 한정으로 정의될 가능성이 높음. 정작 안썼음 ㅋ

    [SerializeField] protected float maxHp;                                  //최대 체력
    [SerializeField] protected float hp;                                     //체력
    [SerializeField] protected float damage;                                 //공격력
    [SerializeField] protected float moveSpeed = 5f;                         //이동속도
    [SerializeField] protected float def;                                    //방어력
    

    protected float rotateSpeed;                                             //회전 속도
                                                                             
    [SerializeField]protected bool isSuperArmor = false;                     //피격이상 면역 유무. 활성화 시 경직이 없다.
    [SerializeField]protected bool isIgnoreDamage = false;                   //무적 유무. 활성화 시 피해를 입지 않는다.

    protected bool isDead = false;
    protected bool isParring = false;

    #endregion

    // Start is called before the first frame update
    virtual protected void Start()
    {
    }

    // Update is called once per frame
    virtual protected void Update()
    {
        
    }

    virtual protected void Init()
    {
        //캐릭터 스텟 설정
        hp = characterStatData.GetHp();
        maxHp = hp;
        damage = characterStatData.GetDamage();
        moveSpeed = characterStatData.GetMoveSpeed();
        def = characterStatData.GetDef();

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
        for (int i = 0; i < weapon.Length; i++)
        {
            weaponDic[weapon[i].name] = weapon[i];

        }
        for(int i = 0; i < pEffect.Length; i++)
        {
            pEffectDic[pEffect[i].name] = pEffect[i];
        }
    }

    public float GetMaxHp() => maxHp;
    public float GetHp() => hp;
    public void SetHp(int value) => hp = value;

    public virtual void TakeDamage(float amount, Character attacker, int hitLevel = -1)
    {
        float defResult = 0f;

        for(int i = 0; i < statDatas.Length; i++)
        {
            defResult += statDatas[i].MaxHp;
        }
        amount -= (int)(defResult * 0.3f);

        if (isIgnoreDamage == true)
        {
            return;
        }

        if (hp - amount < 0)
            hp = 0;
        else
            hp -= (int)amount;

        if (hp <= 0)
        {
            //Dead();                         //이 부분은 이벤트/액션 처리할 것
            anim.SetTrigger("Death");
            isDead = true;
        }

        if (isSuperArmor == false || hitLevel != -1)
        {
            anim.SetInteger("HitLevel", hitLevel);
        }
    }

    /// <summary>
    /// 전투에 의한 변환이 아닌 아이템 및 버프에 의한 체력 변환에 사용
    /// </summary>
    /// <param name="value"></param>
    public void HpTransfer(float value)
    {
        if(hp + value >= maxHp)
        {
            hp = maxHp;
        }
        else if(hp + value <= 0)
        {
            hp = 0;
            anim.SetTrigger("Death");
            isDead = true;
        }
        else
        {
            hp += value;
        }


        //예외적으로(그럴 일은 거의 없음), 현재 체력이 최대 체력이 넘을 경우, 최대체력으로 값을 조정(내린)한다.
        if (hp > maxHp)
            hp = maxHp;
    }

    /// <summary>
    /// 패링 관련 처리할 때, 기본적으로 함께 적용이된다.
    /// 추후 스태프 무기로 수비 스킬을 구현할 때 활용할 수도 있다
    /// </summary>
    /// <param name="state"></param>
    public void SetParringAction(bool state)
    {
        isSuperArmor = state;
        isIgnoreDamage = state;
    }



    /// <summary>
    /// void
    /// </summary>
    public virtual void Dead(float animationTime) {
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

    public int GetResultMaxHp()
    {
        float result = maxHp;
        for (int i = 0; i < statDatas.Length; i++)
        {
            result += statDatas[i].MaxHp;
        }
        if (result < 1)
            return 1;

        //산정할 때 최대 체력이 현재 체력보다 작으면, 현재 최대 체력으로 조정한다.
        if (hp > maxHp)
            hp = maxHp;

        return (int)result;
    }

    /// <summary>
    /// 공격력 및 하단 함수의 이동속도를 버프를 기반으로 최종 값을 산출한다.
    /// 추후 장비 시스템이 추가되면, 장비도 공식에 추가 정리한다
    /// </summary>
    /// <returns></returns>
    public int GetResultDamage()
    {
        float result = damage;
        for (int i = 0; i < statDatas.Length; i++)
        {
            result += statDatas[i].Damage;
        }

        if (result <= 0)
        {
            return 0;
        }

        return (int)result;
    }

    /// <summary>
    /// 캐릭터의 버프 및 기본 능력치 중 이동속도를 계산한 값.
    /// 이동속도는 1이하로 내려갈 수 없다
    /// </summary>
    /// <returns></returns>
    public int GetResultMoveSpeed()
    {
        float result = moveSpeed;
        for (int i = 0; i < statDatas.Length; i++)
        {
            result += statDatas[i].MoveSpeed;
        }

        if (result < 1f)
        {
            return 1;
        }

        return (int)result;
    }

    /// <summary>
    /// 캐릭터의 현재 방어력을 계산하여 반환하는 방식.
    /// 방어력 감소 또는 증가에 의한 계산을 하며
    /// 방어력은 음수로 내려갈 수 없으며, 100이상으로 상승할 수 없다.
    /// </summary>
    /// <returns></returns>
    public int GetResultDefense()
    {
        float result = def;
        for(int i = 0; i < statDatas.Length; i++)
        {
            result += statDatas[i].Defense;
        }
        if (result < 0)
            return 0;
        if (result >= 100)
            return 100;

        return (int)result;
    }

    


    public void SetMoveSpeed(float value) => moveSpeed = value;

    public float GetMoveSpeed() => moveSpeed;

    public float GetCommonDamage() => damage;

    public void SetCommondamage(int value) => damage = value;

    public float GetSkillDamage() => skillDamage;

    public void SetSkillDamage(int value) => skillDamage = value;

    public void SetIsSuperArmor(bool state) => isSuperArmor = state;
    public bool GetIsSuperArmor() => isSuperArmor;

    public void SetIsIgnoreDamage(bool state) => isIgnoreDamage = state;
    public bool GetIsIgnoreDamage() => isIgnoreDamage;

    public float GetRotateSpeed() => rotateSpeed;
    public void SetRotateSpeed(float value) => rotateSpeed = value;

    public bool GetIsDead() => isDead;

    public bool GetIsParring() => isParring;

    public void SetIsParring(bool state) => isParring = state;
}

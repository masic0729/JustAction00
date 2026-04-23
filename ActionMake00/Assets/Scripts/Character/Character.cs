using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 버프 및 장비에 의한 스탯 변화에 대한 능력치 조정을 해당 데이터로 관리한다
/// 기존 AddStatData 구조체를 StatModifierData로 이름 통일
/// </summary>
public enum StatTypeToIndex
{
    MaxHP = 0,
    Damage = 1,
    Defense = 2,
    MoveSpeed = 3
}

public class Character : MonoBehaviour, ICharacterDamageable
{

    // 체력, 공격력 등 핵심 스탯의 원본값을 보관하고 이벤트를 발생시키는 컨테이너
    // Lyra의 ULyraHealthSet에 대응
    private CharacterAttributeSet attributeSet = new CharacterAttributeSet();

    // 버프, 장비, 레벨업에 의한 스탯 가산치를 소스별로 관리하는 컨테이너
    // 기존 statDatas[] 배열을 대체. Lyra의 LyraCombatSet 역할에 대응
    private StatModifierContainer modifierContainer = new StatModifierContainer();

    // 외부 접근자: 두 컨테이너를 외부(버프, 장비 매니저 등)에서 참조할 수 있도록 제공

    // AttributeSet 반환. CharacterBuff, EquipmentManager 등에서 이벤트 구독 시 활용
    public CharacterAttributeSet GetAttributeSet() => attributeSet;

    // ModifierContainer 반환. BuffBase 서브클래스, EquipmentManager에서 보정치 등록 시 활용
    public StatModifierContainer GetModifierContainer() => modifierContainer;

    [SerializeField] CharacterStatData characterStatData;

    [HideInInspector] public Animator anim;

    // 피격 및 사망 시 발생하는 액션류. 현재는 사망 효과 및 피격에 따른 캐릭터 상태 변화만 적용되었음
    // onTransStatData: AttributeSet의 OnHealthChanged 이벤트로 대체됨 (하위 호환을 위해 유지)
    public Action onTransStatData;
    public Action<Character> onDeathAction;

    [SerializeField]
    public ParticleSystem[] pEffect;                                                                                      //해당 데이터는 풀 매니저에 의해 없어질 가능성이 높음
    protected Dictionary<string, ParticleSystem> pEffectDic = new Dictionary<string, ParticleSystem>();  //해당 데이터는 풀 매니저에 의해 없어질 가능성이 높음
    Collider hitCol;
    protected Rigidbody rb;
    public Weapon[] weapon;
    protected Weapon currentWeapon;
    public Dictionary<string, Weapon> weaponDic = new Dictionary<string, Weapon>();

    [Header("캐릭터 스킬 발사체")]
    public GameObject[] skillProjectiles;

    protected int currentExp = 0;                                                                                         //몬스터가 사망 시 그 대상에게 주는 경험치

    // 캐릭터 능력치 관련 데이터
    #region
    protected float skillDamage;                                             //얘는 보스몬스터 한정으로 정의될 가능성이 높음. 정작 안썼음 ㅋ
    [SerializeField] protected float rotateSpeed = 7.5f;                     //회전 속도

    [SerializeField] protected bool isSuperArmor = false;                  //피격이상 면역 유무. 활성화 시 경직이 없다.
    [SerializeField] protected bool isIgnoreDamage = false;                  //무적 유무. 활성화 시 피해를 입지 않는다.

    protected bool isDead = false;
    protected bool isParring = false;
    #endregion

    virtual protected void Start()
    {

    }

    virtual protected void Update()
    {

    }

    virtual protected void Init()
    {
        // AttributeSet을 ScriptableObject 기반으로 초기화한다
        // 기존: hp = characterStatData.GetHp(); 등을 직접 대입하던 방식을 대체
        attributeSet.InitFromData(characterStatData);

        // AttributeSet 이벤트 구독
        // Lyra에서 LyraHealthComponent가 OnOutOfHealth를 구독하는 패턴에 대응
        attributeSet.OnHealthChanged += HandleHealthChanged;
        attributeSet.OnOutOfHealth += HandleOutOfHealth;

        // ModifierContainer 이벤트 구독. 보정치가 바뀌면 UI 갱신 알림 발생
        modifierContainer.OnModifierChanged += HandleModifierChanged;

        onDeathAction += CharacterDeadBase;

        anim = GetComponent<Animator>();
        hitCol = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        hitCol.enabled = true;
        DictionaryInit();
    }

    // -------------------------------------------------------
    // AttributeSet 이벤트 핸들러
    // Lyra에서 LyraHealthComponent가 OnOutOfHealth를 받아 사망처리로 연결하는 구조에 대응
    // -------------------------------------------------------

    // 체력이 변경됐을 때 호출. 기존 onTransStatData와 동일한 역할
    private void HandleHealthChanged(Character instigator, float oldValue, float newValue)
    {
        onTransStatData?.Invoke();
    }

    // 체력이 0 이하가 됐을 때 호출. 기존 TakeDamage 내부의 사망 처리를 여기로 이동
    // Lyra에서 OnOutOfHealth -> GameplayEvent.Death -> 사망 처리로 이어지는 흐름에 대응
    private void HandleOutOfHealth(Character instigator, float oldValue, float newValue)
    {
        anim.SetTrigger("Death");
        isDead = true;
        onDeathAction?.Invoke(instigator);
    }

    // 보정치가 변경됐을 때 호출. 최대 체력 변경이 있으면 AttributeSet에도 반영한다
    private void HandleModifierChanged()
    {
        // 최대 체력 보정치가 바뀌었으므로 AttributeSet의 MaxHealth를 재계산해 동기화한다
        float newMaxHealth = attributeSet.GetMaxHealth()
            - GetModifierContainer().GetModifier(StatModifierSource.Equipment).MaxHp  // 이전 장비값 제거 후
            + modifierContainer.GetTotalMaxHpModifier();                               // 새 합산값 적용

        // 간소화: 기본 maxHp + 전체 보정치 합산으로 재계산
        // 주의: characterStatData가 기준이므로 매번 기준값에서 재산출한다
        float baseMaxHp = characterStatData.GetHp();
        float totalMaxHpBonus = modifierContainer.GetTotalMaxHpModifier();
        attributeSet.ApplyMaxHealthChange(baseMaxHp + totalMaxHpBonus, null);

        onTransStatData?.Invoke();
    }

    void DictionaryInit()
    {
        for (int i = 0; i < weapon.Length; i++)
        {
            weaponDic[weapon[i].name] = weapon[i];
        }
        for (int i = 0; i < pEffect.Length; i++)
        {
            pEffectDic[pEffect[i].name] = pEffect[i];
        }
    }

    // -------------------------------------------------------
    // ICharacterDamageable 구현 : TakeDamage
    // 기존: 계산, hp 감소, 사망처리가 한 함수에 뭉쳐있었음
    // 변경: DamageExecution으로 계산 위임, AttributeSet으로 hp 변경, 이벤트로 사망처리
    // -------------------------------------------------------

    // 데미지를 받아 계산 후 AttributeSet에 체력 변화를 적용한다
    // 사망 처리는 AttributeSet의 OnOutOfHealth 이벤트가 담당한다
    public virtual void TakeDamage(float damage, Character attacker, int hitLevel = -1)
    {
        // 슈퍼아머 상태일 때는 경직 레벨을 무시한다
        int resolvedHitLevel = isSuperArmor ? -1 : hitLevel;

        // DamageExecution에 계산을 위임한다 (Lyra의 Execution 호출 패턴에 대응)
        DamageResult result = DamageExecution.Calculate(attacker, this, damage, resolvedHitLevel);

        // 무적으로 차단된 경우 이후 처리를 중단한다
        if (result.WasBlocked)
            return;

        // AttributeSet을 통해 체력을 감소시킨다 (직접 hp 필드를 건드리지 않는다)
        // 사망 여부는 AttributeSet의 OnOutOfHealth 이벤트가 자동으로 판단한다
        attributeSet.ApplyHealthChange(-result.FinalDamage, attacker);

        // 사망하지 않았다면 피격 애니메이션 처리
        if (!isDead && result.HitLevel != -1)
        {
            anim.SetInteger("HitLevel", result.HitLevel);
            anim.SetTrigger("Hit");
        }
    }

    // 캐릭터는 기본적으로 사망 시 콜라이더를 비활성화한다
    void CharacterDeadBase(Character attacker)
    {
        Collider col = GetComponent<Collider>();
        col.enabled = false;
    }

    // 전투에 의한 변환이 아닌 아이템 및 버프에 의한 체력 변환에 사용
    // Lyra의 GE_Heal_Instant -> HealExecution 흐름에 대응
    public void HpTransfer(float value)
    {
        HealResult result = DamageExecution.CalculateHeal(this, value);
        attributeSet.ApplyHealthChange(result.FinalHeal, result.Instigator);
    }

    // -------------------------------------------------------
    // 최종 스탯 계산 함수 (기본값 + 보정치 합산)
    // AttributeSet의 기본값과 ModifierContainer의 보정치를 합산해 반환한다
    // -------------------------------------------------------

    // 버프 및 장비를 포함한 최종 최대 체력을 반환한다
    public int GetResultMaxHp()
    {
        float result = attributeSet.GetMaxHealth();
        if (result < 1) return 1;
        return (int)result;
    }

    // 버프 및 장비를 포함한 최종 공격력을 반환한다
    public int GetResultDamage()
    {
        float result = attributeSet.GetDamage() + modifierContainer.GetTotalDamageModifier();
        if (result <= 0) return 0;
        return (int)result;
    }

    // 버프 및 장비를 포함한 최종 이동속도를 반환한다. 최소값은 1
    public int GetResultMoveSpeed()
    {
        float result = attributeSet.GetMoveSpeed() + modifierContainer.GetTotalMoveSpeedModifier();
        if (result < 1f) return 1;
        return (int)result;
    }

    // 버프 및 장비를 포함한 최종 방어력을 반환한다. 범위는 [0, 100]
    public int GetResultDefense()
    {
        float result = attributeSet.GetDefense() + modifierContainer.GetTotalDefenseModifier();
        if (result < 0) return 0;
        if (result >= 100) return 100;
        return (int)result;
    }

    // -------------------------------------------------------
    // 기존 접근자 유지 (하위 클래스 및 외부 참조 호환)
    // 내부는 AttributeSet을 통해 값을 읽거나 쓴다
    // -------------------------------------------------------

    // 현재 체력 반환
    public float GetHp() => attributeSet.GetHealth();

    // 최대 체력 반환 (보정치 포함 최종값은 GetResultMaxHp 사용)
    public float GetMaxHp() => attributeSet.GetMaxHealth();

    // 기본 공격력 반환 (보정치 포함 최종값은 GetResultDamage 사용)
    public float GetCommonDamage() => attributeSet.GetDamage();

    // 기본 방어력 반환
    public float GetDefense() => attributeSet.GetDefense();

    // 기본 이동속도 반환
    public float GetMoveSpeed() => attributeSet.GetMoveSpeed();

    // 사망 여부 반환
    public bool GetIsDead() => isDead;

    // 패링 여부 반환
    public bool GetIsParring() => isParring;

    // 무적 여부 반환
    public bool GetIsIgnoreDamage() => isIgnoreDamage;

    // 슈퍼아머 여부 반환
    public bool GetIsSuperArmor() => isSuperArmor;

    // 회전 속도 반환
    public float GetRotateSpeed() => rotateSpeed;

    // 획득 경험치 반환
    public int GetCurrentExp() => currentExp;

    // 스킬 데미지 반환
    public float GetSkillDamage() => skillDamage;

    // -------------------------------------------------------
    // 기존 세터 유지 (하위 클래스 호환)
    // -------------------------------------------------------

    public void SetIsIgnoreDamage(bool state) => isIgnoreDamage = state;
    public void SetIsSuperArmor(bool state) => isSuperArmor = state;
    public void SetIsParring(bool state) => isParring = state;
    public void SetRotateSpeed(float value) => rotateSpeed = value;
    public void SetSkillDamage(int value) => skillDamage = value;

    // 패링 관련 처리할 때, 기본적으로 함께 적용이 된다
    // 추후 스태프 무기로 수비 스킬을 구현할 때 활용할 수도 있다
    public void SetParringAction(bool state)
    {
        isSuperArmor = state;
    }

    // -------------------------------------------------------
    // 기타 기존 함수 유지
    // -------------------------------------------------------

    public virtual void Dead(float animationTime)
    {
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
}
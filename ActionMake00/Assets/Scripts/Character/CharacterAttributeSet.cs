// Lyra의 LyraHealthSet 구조에서 영감받아 제작한 유니티 스탯 정의 클래스
// 캐릭터의 핵심 스탯 값을 보관하고, 값이 바뀔 때 이벤트를 발생시킨다
// 스탯 값은 반드시 이 클래스의 함수를 통해서만 변경한다 (직접 필드 접근 금지)

using UnityEngine;

// 스탯 변경 시 발생하는 이벤트 델리게이트
// Lyra의 FLyraAttributeEvent에 대응
// instigator: 변경을 유발한 캐릭터 (없으면 null), oldValue: 변경 전 값, newValue: 변경 후 값
public delegate void AttributeChangedEvent(Character instigator, float oldValue, float newValue);

public class CharacterAttributeSet
{
    // 체력이 0 이하로 떨어졌는지 추적하는 플래그
    // Lyra의 bOutOfHealth에 대응. 사망 이벤트가 중복 발생하는 것을 방지한다
    private bool isOutOfHealth = false;

    // 체력 변경 전 스냅샷. PostGameplayEffectExecute에서 전후 값 비교용으로 쓰이던 패턴
    private float healthBeforeChange = 0f;

    // -------------------------------------------------------
    // 스탯 원본값 (ScriptableObject에서 초기화된 기본 수치)
    // 이 값들은 버프나 장비에 의해 직접 수정되지 않는다
    // 수정이 필요한 경우 StatModifierContainer를 통해 가산치로 관리한다
    // -------------------------------------------------------

    // 현재 체력. 0 이하로 내려가지 않으며 MaxHealth를 초과하지 않는다
    // Lyra에서 HideFromModifiers가 붙어있던 것처럼, 외부에서 직접 수정 불가
    private float health;

    // 최대 체력. 최소값은 1이다
    private float maxHealth;

    // 기본 공격력
    private float damage;

    // 기본 방어력. 0 이상 100 이하로 유지된다
    private float defense;

    // 기본 이동속도. 1 이하로 내려가지 않는다
    private float moveSpeed;

    // -------------------------------------------------------
    // 스탯 변경 이벤트 (Lyra의 OnHealthChanged, OnOutOfHealth 패턴에 대응)
    // UI, 사운드, 이펙트 등 외부 시스템이 이 이벤트를 구독해서 반응한다
    // -------------------------------------------------------

    // 체력이 변경됐을 때 발생. UI 체력바 갱신 등에 활용
    public event AttributeChangedEvent OnHealthChanged;

    // 최대 체력이 변경됐을 때 발생
    public event AttributeChangedEvent OnMaxHealthChanged;

    // 체력이 처음으로 0 이하가 됐을 때 한 번만 발생. 사망 처리 연결용
    // Lyra의 OnOutOfHealth 이벤트에 대응
    public event AttributeChangedEvent OnOutOfHealth;

    // 공격력이 변경됐을 때 발생
    public event AttributeChangedEvent OnDamageChanged;

    // -------------------------------------------------------
    // 초기화
    // -------------------------------------------------------

    // ScriptableObject 기반 기본 스탯을 받아 초기 수치를 설정한다
    // Character.Init() 에서 호출한다
    public void InitFromData(CharacterStatData data)
    {
        maxHealth   = data.GetHp();
        health      = maxHealth;
        damage      = data.GetDamage();
        defense     = data.GetDef();
        moveSpeed   = data.GetMoveSpeed();
        isOutOfHealth = false;
    }

    // -------------------------------------------------------
    // 읽기 전용 접근자 (Lyra의 ATTRIBUTE_ACCESSORS 매크로 역할)
    // -------------------------------------------------------

    // 현재 체력 반환
    public float GetHealth()    => health;

    // 최대 체력 반환
    public float GetMaxHealth() => maxHealth;

    // 기본 공격력 반환
    public float GetDamage()    => damage;

    // 기본 방어력 반환
    public float GetDefense()   => defense;

    // 기본 이동속도 반환
    public float GetMoveSpeed() => moveSpeed;

    // 체력 0 이하 상태인지 반환
    public bool GetIsOutOfHealth() => isOutOfHealth;

    // -------------------------------------------------------
    // 스탯 변경 함수 (직접 필드를 수정하는 유일한 경로)
    // Lyra에서 PostGameplayEffectExecute 내부에서 SetHealth를 통해서만 변경하던 패턴
    // -------------------------------------------------------

    // 체력 변화량을 받아 최종 체력을 계산하고 이벤트를 발생시킨다
    // delta: 양수면 회복, 음수면 데미지. 체력은 [0, MaxHealth] 범위로 클램프된다
    // instigator: 변화를 유발한 캐릭터 (없으면 null)
    public void ApplyHealthChange(float delta, Character instigator)
    {
        healthBeforeChange = health;

        // 체력 범위 클램프 (Lyra의 ClampAttribute에 대응)
        health = Mathf.Clamp(health + delta, 0f, maxHealth);

        // 실제로 체력이 바뀐 경우에만 이벤트 발생
        if (!Mathf.Approximately(health, healthBeforeChange))
        {
            OnHealthChanged?.Invoke(instigator, healthBeforeChange, health);
        }

        // 처음으로 체력이 0 이하가 됐을 때만 사망 이벤트 발생
        // Lyra의 bOutOfHealth 플래그 패턴과 동일한 방식
        if (health <= 0f && !isOutOfHealth)
        {
            isOutOfHealth = true;
            OnOutOfHealth?.Invoke(instigator, healthBeforeChange, health);
        }

        // 부활 또는 회복으로 체력이 다시 양수가 된 경우 플래그 초기화
        if (health > 0f && isOutOfHealth)
        {
            isOutOfHealth = false;
        }
    }

    // 최대 체력을 새 값으로 변경한다. 최소값은 1이며, 감소 시 현재 체력도 함께 조정된다
    public void ApplyMaxHealthChange(float newMaxHealth, Character instigator)
    {
        float oldMaxHealth = maxHealth;

        // 최대 체력은 최소 1 보장 (Lyra의 ClampAttribute에 대응)
        maxHealth = Mathf.Max(newMaxHealth, 1f);

        if (!Mathf.Approximately(maxHealth, oldMaxHealth))
        {
            OnMaxHealthChanged?.Invoke(instigator, oldMaxHealth, maxHealth);

            // 최대 체력이 줄어서 현재 체력이 초과하는 경우 현재 체력도 내린다
            if (health > maxHealth)
            {
                ApplyHealthChange(maxHealth - health, instigator);
            }
        }
    }

    // 기본 공격력을 직접 변경한다 (레벨업 등 영구 수치 변화 시 사용)
    public void SetBaseDamage(float newDamage, Character instigator)
    {
        float old = damage;
        damage = Mathf.Max(0f, newDamage);
        if (!Mathf.Approximately(damage, old))
        {
            OnDamageChanged?.Invoke(instigator, old, damage);
        }
    }

    // 기본 방어력을 직접 변경한다. 범위는 0.0f ~ 100.0f
    public void SetBaseDefense(float newDefense)
    {
        defense = Mathf.Clamp(newDefense, 0f, 100f);
    }

    // 기본 이동속도를 직접 변경한다. 최소값은 1
    public void SetBaseMoveSpeed(float newMoveSpeed)
    {
        moveSpeed = Mathf.Max(1f, newMoveSpeed);
    }
}

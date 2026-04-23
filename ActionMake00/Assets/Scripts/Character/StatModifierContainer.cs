// Lyra의 LyraCombatSet 구조에서 영감받아 제작한 스탯 보정치 관리 클래스
// 버프, 장비, 레벨업에 의한 스탯 가산치를 소스별로 딕셔너리로 관리한다
// 기존 AddStatData[] statDatas 배열의 인덱스 의존 구조를 대체한다
// 스탯 보정치의 합산 결과는 Character의 GetResult~ 함수에서 최종 수치 계산에 활용된다

using System.Collections.Generic;
using UnityEngine;

// 스탯 보정치의 출처를 구분하는 열거형
// 기존 AddStatName enum에 대응하며, 딕셔너리 키로 활용된다
public enum StatModifierSource
{
    Buff      = 0,   // 버프에 의한 가산치
    Equipment = 1,   // 장비에 의한 가산치
    LevelUp   = 2    // 레벨업에 의한 가산치
}

// 스탯 보정 수치를 담는 구조체
// 기존 AddStatData struct를 유지한다 (기존 코드와의 호환성)
[System.Serializable]
public struct StatModifierData
{
    // 최대 체력 보정치
    public float MaxHp;
    // 공격력 보정치
    public float Damage;
    // 방어력 보정치
    public float Defense;
    // 이동속도 보정치
    public float MoveSpeed;
}

// 소스별 스탯 보정치를 관리하는 컨테이너 클래스
// 기존 statDatas[] 배열의 문제점(인덱스 의존, 런타임 오류 위험)을 딕셔너리 구조로 해결한다
public class StatModifierContainer
{
    // 소스별 스탯 보정치 저장소
    // key: StatModifierSource (Buff, Equipment, LevelUp)
    // value: 해당 소스가 제공하는 스탯 보정치
    private Dictionary<StatModifierSource, StatModifierData> modifiers
        = new Dictionary<StatModifierSource, StatModifierData>();

    // 보정치가 변경될 때 외부에 알리는 이벤트
    // Character의 GetResult~ 함수 재계산 및 UI 갱신 트리거로 활용된다
    // Lyra의 OnHealthChanged 이벤트와 같은 역할로, 외부 시스템이 구독한다
    public event System.Action OnModifierChanged;

    // -------------------------------------------------------
    // 보정치 설정 및 제거
    // -------------------------------------------------------

    // 특정 소스의 스탯 보정치를 설정하거나 덮어쓴다
    // 버프 적용, 장비 착용, 레벨업 시 호출한다
    // source: 보정치 출처, data: 새로 적용할 보정 수치
    public void SetModifier(StatModifierSource source, StatModifierData data)
    {
        modifiers[source] = data;
        OnModifierChanged?.Invoke();
    }

    // 특정 소스의 스탯 보정치를 제거한다
    // 버프 만료, 장비 해제 시 호출한다
    public void RemoveModifier(StatModifierSource source)
    {
        if (modifiers.ContainsKey(source))
        {
            modifiers.Remove(source);
            OnModifierChanged?.Invoke();
        }
    }

    // 특정 소스의 보정치를 초기화(0으로)한다
    // 버프가 일시적으로 무력화될 때 등 사용
    public void ClearModifier(StatModifierSource source)
    {
        if (modifiers.ContainsKey(source))
        {
            modifiers[source] = new StatModifierData();
            OnModifierChanged?.Invoke();
        }
    }

    // -------------------------------------------------------
    // 합산 조회 함수 (Character의 GetResult~ 함수에서 호출한다)
    // -------------------------------------------------------

    // 모든 소스의 최대 체력 보정치 합산값을 반환한다
    public float GetTotalMaxHpModifier()
    {
        float total = 0f;
        foreach (var mod in modifiers.Values)
            total += mod.MaxHp;
        return total;
    }

    // 모든 소스의 공격력 보정치 합산값을 반환한다
    public float GetTotalDamageModifier()
    {
        float total = 0f;
        foreach (var mod in modifiers.Values)
            total += mod.Damage;
        return total;
    }

    // 모든 소스의 방어력 보정치 합산값을 반환한다
    public float GetTotalDefenseModifier()
    {
        float total = 0f;
        foreach (var mod in modifiers.Values)
            total += mod.Defense;
        return total;
    }

    // 모든 소스의 이동속도 보정치 합산값을 반환한다
    public float GetTotalMoveSpeedModifier()
    {
        float total = 0f;
        foreach (var mod in modifiers.Values)
            total += mod.MoveSpeed;
        return total;
    }

    // -------------------------------------------------------
    // 소스별 개별 조회
    // -------------------------------------------------------

    // 특정 소스의 보정치를 반환한다. 해당 소스가 없으면 빈 구조체를 반환한다
    public StatModifierData GetModifier(StatModifierSource source)
    {
        if (modifiers.TryGetValue(source, out StatModifierData data))
            return data;
        return new StatModifierData();
    }

    // 특정 소스의 보정치가 등록되어 있는지 확인한다
    public bool HasModifier(StatModifierSource source)
    {
        return modifiers.ContainsKey(source);
    }
}

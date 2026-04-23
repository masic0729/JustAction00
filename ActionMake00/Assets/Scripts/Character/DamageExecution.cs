// Lyra의 ULyraDamageExecution 구조에서 영감받아 제작한 데미지 계산 클래스
// 기존 Character.TakeDamage() 내부에 박혀있던 계산 로직을 완전히 분리했다
// 이 클래스는 계산 결과를 반환할 뿐, 직접 체력을 수정하지 않는다
// 체력 수정은 반환된 DamageResult를 받은 Character가 AttributeSet을 통해 수행한다

using UnityEngine;

// 데미지 계산 결과를 담는 구조체
// Lyra에서 GameplayEffectSpec이 담당하던 계산 결과 전달 역할을 대신한다
public struct DamageResult
{
    // 방어력 계산이 완료된 최종 데미지 수치
    public float FinalDamage;

    // 데미지를 유발한 캐릭터 (사망 이벤트의 attacker 파라미터로 전달된다)
    public Character Instigator;

    // 무적 또는 면역으로 데미지가 차단됐는지 여부
    // Lyra의 TAG_Gameplay_DamageImmunity 체크 결과에 대응
    public bool WasBlocked;

    // 피격 경직 레벨. -1이면 경직 없음
    public int HitLevel;
}

// 회복 계산 결과를 담는 구조체
// Lyra의 ULyraHealExecution 결과에 대응
public struct HealResult
{
    // 최종 회복량
    public float FinalHeal;

    // 회복을 시전한 캐릭터
    public Character Instigator;
}

// 데미지 및 회복 계산을 전담하는 정적 클래스
// Character나 Attacker가 직접 계산하지 않고 이 클래스에 위임한다
// Lyra에서 Execution이 AttributeSet의 값을 읽어 계산하는 구조와 동일한 흐름
public static class DamageExecution
{
    // 데미지를 계산하여 결과를 반환한다
    // Lyra의 ULyraDamageExecution.Execute()에 대응
    // 
    // attacker: 공격하는 캐릭터
    // target: 피격당하는 캐릭터
    // rawDamage: 방어력 계산 전 원본 데미지 (Attacker의 damageMultify * owner.GetResultDamage() 결과)
    // hitLevel: 피격 경직 레벨 (-1이면 경직 없음)
    public static DamageResult Calculate(
        Character attacker,
        Character target,
        float rawDamage,
        int hitLevel = -1)
    {
        DamageResult result = new DamageResult
        {
            Instigator = attacker,
            HitLevel   = hitLevel
        };

        // 무적 상태 체크. 무적이면 데미지를 0으로 처리하고 조기 반환
        // Lyra의 TAG_Gameplay_DamageImmunity 체크 로직에 대응
        if (target.GetIsIgnoreDamage())
        {
            result.WasBlocked   = true;
            result.FinalDamage  = 0f;
            return result;
        }

        // 슈퍼아머는 경직만 면역이고 데미지는 정상 적용
        // result.HitLevel = -1로 처리는 Character.TakeDamage에서 담당

        // 방어력 기반 데미지 감소 계산
        // 기존 TakeDamage 내부의 계산 공식을 그대로 이전
        // 방어력 100이면 데미지 50% 감소, 200이면 약 33% 감소
        float totalDefense      = target.GetResultDefense();
        float damageMultiplier  = 100f / (100f + totalDefense);
        float finalDamage       = rawDamage * damageMultiplier;

        result.WasBlocked  = false;
        result.FinalDamage = Mathf.Max(0f, finalDamage);

        Debug.Log($"[DamageExecution] rawDamage: {rawDamage}, defense: {totalDefense}, final: {result.FinalDamage}");

        return result;
    }

    // 회복량을 계산하여 결과를 반환한다
    // Lyra의 ULyraHealExecution에 대응
    // 현재는 단순 전달이지만, 회복 저항이나 회복량 증감 버프 확장 시 이 함수에서 처리한다
    //
    // instigator: 회복을 시전한 캐릭터 (아이템 사용 시 자기 자신)
    // rawHeal: 계산 전 원본 회복량
    public static HealResult CalculateHeal(
        Character instigator,
        float rawHeal)
    {
        HealResult result = new HealResult
        {
            Instigator = instigator,
            FinalHeal  = Mathf.Max(0f, rawHeal)
        };

        return result;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// 퍼펙트 회피 성공 시 화면 암전 및 슬로우모션 연출을 담당하는 컴포넌트
// Player.OnPerfectDodge 이벤트를 구독하여 동작한다
public class PerfectDodgeEffect : MonoBehaviour
{
    [Header("URP Post-Processing Volume 참조")]
    // 씬에 배치된 Global Volume을 인스펙터에서 연결
    [SerializeField] private Volume globalVolume;

    [Header("암전 연출 설정")]
    // Post Exposure 감소량 - 값이 클수록 화면 전체가 더 어두워짐 (권장 3.0 ~ 4.0)
    [SerializeField] private float exposureReduction = 3.0f;

    // 암전 연출 총 지속 시간 (초) - 절반은 암전 진입, 절반은 복구에 사용
    [SerializeField] private float effectDuration = 0.6f;

    [Header("슬로우모션 설정")]
    // 슬로우모션 배율 - 0.1이면 10배 느려짐
    [SerializeField] private float slowTimeScale = 0.15f;

    // URP Post-Processing ColorAdjustments Override 컴포넌트 캐싱
    private ColorAdjustments colorAdjustments;

    // Player 컴포넌트 참조 - 이벤트 구독 및 해제에 사용
    private Player player;

    // 연출이 이미 진행 중인지 여부 - 중복 실행 방지용
    private bool isEffectPlaying = false;

    private void Awake()
    {
        player = GetComponent<Player>();

        // Global Volume에서 ColorAdjustments Override 컴포넌트를 가져옴
        // 인스펙터에서 Volume Profile에 Color Adjustments가 추가되어 있어야 함
        if (globalVolume != null)
        {
            globalVolume.profile.TryGet(out colorAdjustments);
        }
        else
        {
            Debug.LogWarning("PerfectDodgeEffect : Global Volume이 연결되어 있지 않습니다.");
        }
    }

    private void OnEnable()
    {
        // Player 이벤트 구독 - 퍼펙트 회피 성공 시 연출 실행
        if (player != null)
            player.OnPerfectDodge += PlayEffect;
    }

    private void OnDisable()
    {
        // 오브젝트 비활성화 또는 파괴 시 구독 해제 - 메모리 누수 방지
        if (player != null)
            player.OnPerfectDodge -= PlayEffect;
    }

    // 퍼펙트 회피 성공 시 OnPerfectDodge 이벤트를 통해 호출되는 진입 함수
    public void PlayEffect()
    {
        // 이미 연출 중이면 중복 실행 방지
        if (isEffectPlaying)
            return;

        StartCoroutine(PerfectDodgeRoutine());
    }

    // 암전 진입 - 슬로우모션 - 복구 순서로 진행되는 연출 코루틴
    private IEnumerator PerfectDodgeRoutine()
    {
        isEffectPlaying = true;

        float halfDuration = effectDuration * 0.5f;

        // 1단계 슬로우모션 진입
        Time.timeScale = slowTimeScale;

        // Physics 연산 주기를 timeScale에 맞게 동기화 - 미동기화 시 물리가 튐
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // 2단계 화면 전체 점진적 암전 - halfDuration 동안 Post Exposure를 낮춤
        float elapsed = 0f;
        while (elapsed < halfDuration)
        {
            // timeScale이 변경된 상태에서도 연출은 실제 시간 기준으로 진행
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);

            // 화면 노출을 낮춰 공기 자체가 어두워지는 효과
            if (colorAdjustments != null)
                colorAdjustments.postExposure.value = Mathf.Lerp(0f, -exposureReduction, t);

            yield return null;
        }

        // 3단계 화면 점진적 복구 - halfDuration 동안 Post Exposure를 원래 값으로 복구
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);

            // 화면 노출을 다시 정상값으로 복구
            if (colorAdjustments != null)
                colorAdjustments.postExposure.value = Mathf.Lerp(-exposureReduction, 0f, t);

            yield return null;
        }

        // 4단계 슬로우모션 해제 및 Post Exposure 값 초기화
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        // 혹시 모를 잔류값 강제 초기화
        if (colorAdjustments != null)
            colorAdjustments.postExposure.value = 0f;

        isEffectPlaying = false;
    }
}
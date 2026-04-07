using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어의 체력, 경험치, 스킬 및 버프 등 플레이어의 전투와 관련된 데이터를 시각화하는 스크립트
/// </summary>
public class PlayerStatUI : MonoBehaviour
{
    SkillManager skillManager;
    Player player;

    // 실제 HP바 - 피격 즉시 줄어듦
    public Image HpSlider;

    // 고스트 HP바 - 딜레이 후 실제 HP바 위치까지 부드럽게 따라옴
    // 인스펙터에서 HpSlider 뒤에 배치된 Image를 연결
    public Image GhostHpSlider;

    // 고스트 HP바가 따라오기 시작하기 전 대기 시간 (초)
    [SerializeField] private float ghostDelay = 0.8f;

    // 고스트 HP바가 실제 HP바 값으로 줄어드는 속도
    [SerializeField] private float ghostLerpSpeed = 3.0f;

    public Slider ExpSlider;
    public Image[] SkillCoolTime;
    public Image[] skills;

    // 현재 실행 중인 고스트 HP바 딜레이 코루틴 - 피격 연속 시 이전 코루틴 취소 후 재시작
    private Coroutine ghostDelayCoroutine;

    // 고스트 HP바가 현재 목표값으로 이동 중인지 여부
    private bool isGhostMoving = false;

    // 고스트 HP바의 목표 fillAmount - 실제 HP바의 현재 값
    private float ghostTargetFill;

    private void Awake()
    {
        skillManager = GetComponent<SkillManager>();
    }

    void Start()
    {
        player = GetComponent<Player>();
        player.onTransStatData += PlayerUpdateHp;

        // 시작 시 고스트 HP바를 실제 HP바와 동일하게 초기화
        if (GhostHpSlider != null)
            GhostHpSlider.fillAmount = 1f;
    }

    void Update()
    {
        UpdateSkillCoolTimeValue();
        UpdateGhostHpBar();
    }

    void UpdateSkillCoolTimeValue()
    {
        float[] coolTimer = skillManager.GetSkillCoolTimerDatas();
        float[] coolTime = skillManager.GetSkillCoolTimeDatas();
        for (int i = 0; i < SkillCoolTime.Length; i++)
        {
            SkillCoolTime[i].fillAmount = coolTimer[i] / coolTime[i];
        }
    }

    /// <summary>
    /// 플레이어 체력 상태를 업데이트 한다.
    /// 실제 HP바는 즉시 반영, 고스트 HP바는 딜레이 후 따라옴
    /// </summary>
    public void PlayerUpdateHp()
    {
        // 실제 HP바 즉시 반영
        float targetFill = (float)player.GetHp() / player.GetResultMaxHp();
        HpSlider.fillAmount = targetFill;

        // 고스트 HP바 목표값 갱신
        ghostTargetFill = targetFill;

        // 이전 딜레이 코루틴이 있으면 취소 후 재시작 - 연속 피격 시 딜레이 초기화
        if (ghostDelayCoroutine != null)
            StopCoroutine(ghostDelayCoroutine);

        ghostDelayCoroutine = StartCoroutine(GhostHpDelayRoutine());
    }

    // 고스트 HP바가 이동 중일 때 매 프레임 Lerp로 부드럽게 줄어드는 처리
    void UpdateGhostHpBar()
    {
        if (!isGhostMoving || GhostHpSlider == null)
            return;

        // 실제 HP바 값을 향해 부드럽게 이동
        GhostHpSlider.fillAmount = Mathf.Lerp(
            GhostHpSlider.fillAmount,
            ghostTargetFill,
            ghostLerpSpeed * Time.deltaTime
        );

        // 목표값에 충분히 가까워지면 이동 종료 및 값 고정
        if (Mathf.Abs(GhostHpSlider.fillAmount - ghostTargetFill) < 0.001f)
        {
            GhostHpSlider.fillAmount = ghostTargetFill;
            isGhostMoving = false;
        }
    }

    // 피격 후 ghostDelay 만큼 대기한 뒤 고스트 HP바 이동을 시작하는 코루틴
    private IEnumerator GhostHpDelayRoutine()
    {
        isGhostMoving = false;
        yield return new WaitForSeconds(ghostDelay);

        // 딜레이가 끝나면 고스트 HP바 이동 시작
        isGhostMoving = true;
    }

    public void UpdateExp()
    {
        ExpSlider.value = (float)player.GetCurrentExp() / player.GetNeedExp();
    }

    /// <summary>
    /// 스킬의 아이콘을 업데이트 하기 위한 함수
    /// 스킬 활성화는 별개로 순수 장비 타입에 따른 스킬 변환이다
    /// </summary>
    public void UpdateSkillIcon()
    {
        for (int i = 0; i < skills.Length; i++)
        {
            skills[i].sprite = skillManager.GetSkillData().weaponSkillBase[i].icon;
        }
    }

    //나머지가 아마 버프시스템의 시각화인데, 이걸 최대한 빨리 할 것. 목표는 늦어도 이번 주 안으로
}
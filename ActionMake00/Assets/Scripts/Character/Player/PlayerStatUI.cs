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
    public Slider HpSlider;
    public Slider ExpSlider;
    public Slider[] SkillCoolTime; 
    public Image[] skills;

    private void Awake()
    {
        skillManager = GetComponent<SkillManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        player.hitAction += PlayerUpdateHp;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSkillCoolTimeValue();
    }
    
    void UpdateSkillCoolTimeValue()
    {
        float[] coolTimer = skillManager.GetSkillCoolTimerDatas();
        float[] coolTime = skillManager.GetSkillCoolTimeDatas();

        for (int i = 0; i < SkillCoolTime.Length; i++)
        {

            SkillCoolTime[i].value = coolTimer[i] / coolTime[i];
        }
    }

    /// <summary>
    /// 플레이어 체력 상태를 업데이트 한다.
    /// </summary>
    public void PlayerUpdateHp()
    {
        HpSlider.value = (float)player.GetHp()/player.GetResultMaxHp();
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
        for(int i = 0; i < skills.Length; i++)
        {
            skills[i].sprite = skillManager.GetSkillData().weaponSkillBase[i].icon;
        }
    }

    //나머지가 아마 버프시스템의 시각화인데, 이걸 최대한 빨리 할 것. 목표는 늦어도 이번 주 안으로
}

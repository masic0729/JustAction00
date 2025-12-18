using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public Dictionary<string, Action> weaponSkillDic;

    [SerializeField] PlayerSkillData[] weaponData;

    Dictionary<string, float> weaponSkillCooltimeDic;
    Dictionary<string, float> weaponSkillCoolListDic;

    Player player;

    string currentWeaponType;

    private void Awake()
    {
        weaponSkillDic = new Dictionary<string, Action>();

        weaponSkillCoolListDic = new Dictionary<string, float>();

        weaponSkillCooltimeDic = new Dictionary<string, float>();

        player = GetComponent<Player>();
    }

    private void Start()
    {
        Init();
    }

    void Update()
    {
        SkillCoolTimer();

    }

    void Init()
    {

        //초기 모든 스킬의 쿨타임 값을 0으로 초기화한다.
        for (int i = 0; i < weaponData.Length; i++)
        {
            for (int j = 0; j < weaponData[i].weaponSkillBase.Length; j++)
            {
                string skillTriggerName;
                skillTriggerName = weaponData[i].weaponSkillBase[j].triggerName;

                float coolTime = weaponData[i].weaponSkillBase[j].coolTime;

                //new
                weaponSkillCooltimeDic[skillTriggerName] = 0;
                weaponSkillCoolListDic[skillTriggerName] = coolTime;
            }
        }

    }

    /// <summary>
    /// 플레이어의 무기에 따른 스킬구조를 저장한다.
    /// 애니메이션 트리거와 스킬 딕셔너리이름은 똑같으며,
    /// 무기에 따른 스킬 여부는 currentWeaponType에 따라 다르다.
    /// </summary>
    /// <param name="type">스킬을 저장하려는 플레이어 무기 따입</param>
    /// <param name="data">각 스킬 명령에 저장할 기능들</param>
    public void SetSkillDic(WeaponType type, Action data, int index)
    {
        string skillKey = "Skill" + index.ToString();

        //new
        Debug.Log(skillKey + "1번째");
        Debug.Log(data + "2번째");
        weaponSkillDic[skillKey] = data;
        weaponSkillDic[skillKey] += player.WeaponColDisable;
    }

    /// <summary>
    /// 웨폰타입을 기반으로 스킬 데이터를 불러오는 것
    /// 불러올 때 PlayerSword, PlayerStaff
    /// </summary>
    /// <param name="weaponType">플레이어 무기 타입명</param>
    public void SkillDataInit(string weaponType)
    {
        transform.Find(weaponType).GetComponent<PlayerSkillProcessor>().InitSkill();
    }

    /// <summary>
    /// 모든 무기들의 스킬 쿨타임을 감소하고 있다.
    /// 
    /// 해당 코드는 매 프레임마다 스킬 쿨타임을 내릴려고 한다
    /// </summary>
    public void SkillCoolTimer()
    {

        for(int i = 0; i < GetSkillData().weaponSkillBase.Length; i++)
        {
            string skillTriggerName = weaponData[i].weaponSkillBase[i].triggerName;


            if (weaponSkillCooltimeDic[skillTriggerName] > 0)
                weaponSkillCooltimeDic[skillTriggerName] -= Time.deltaTime;

            if (weaponSkillCooltimeDic[skillTriggerName] <= 0)
                weaponSkillCooltimeDic[skillTriggerName] = 0;
        }

    }

    /// <summary>
    /// 현재 플레이어 스킬들의 잔여 쿨타임 상태 값을 반환한다
    /// </summary>
    /// <returns></returns>
    public float[] GetSkillCoolTimerDatas()
    {
        int skillCount = GetSkillData().weaponSkillBase.Length;
        float[] datas = new float[skillCount];


        for (int i = 0; i < datas.Length; i++)
        {
            string skillTriggerName = GetSkillData().weaponSkillBase[i].triggerName;
            datas[i] = weaponSkillCooltimeDic[skillTriggerName];
        }
        return datas;

    }

    /// <summary>
    /// 현재 플레이어 스킬의 기본 쿨타임 값을 반환한다
    /// </summary>
    /// <returns></returns>
    public float[] GetSkillCoolTimeDatas()
    {
        int skillCount = GetSkillData().weaponSkillBase.Length;
        float[] datas = new float[skillCount];


        for (int i = 0; i < datas.Length; i++)
        {
            string skillTriggerName = GetSkillData().weaponSkillBase[i].triggerName;
            datas[i] = weaponSkillCoolListDic[skillTriggerName];
        }
        return datas;
    }

    public void SetSkillCoolTime(int key)
    {
        string skillKey = "Skill" + key.ToString();

        weaponSkillCooltimeDic[skillKey] = weaponSkillCoolListDic[skillKey];
    }


    public bool isSkillCanUse(string skillKey)
    {

        if (weaponSkillCooltimeDic[skillKey] > 0)
            return false;

        return true;
    }


    public void WeaponSkillLoad(string weaponType)
    {
        PlayerSkillProcessor skillBase = null;
        if (weaponType == "Sword")
        {
            skillBase = transform.Find("SwordSkill").GetComponent<PlayerSwordSkill>();
        }
        if (weaponType == "Staff")
        {
            skillBase = transform.Find("StaffSkill").GetComponent<PlayerStaffSkill>();

        }
        if (skillBase != null)
        {
            skillBase.InitSkill();
        }
    }

    /// <summary>
    /// 저장된 스킬을 기반으로 플레이어가 스킬을 사용한다
    /// </summary>
    /// <param name="key">사용되는 스킬트리거명. 애니메이터의 트리거과 동일한 값이다</param>
    public void UseSkill(int key)
    {
        string skillKey = "Skill" + key.ToString();


        weaponSkillDic[skillKey]();
        //SetSkillCoolTime(key);
    }

    public void SetCurrentWeaponType(string typeName) => currentWeaponType = typeName;

    public string GetCurrentWeaponType() => currentWeaponType;

    /// <summary>
    /// 스킬 데이터를 불러온다.
    /// </summary>
    /// <param name="index">0부터 1까지 존재한다</param>
    /// <returns></returns>
    public PlayerSkillData GetSkillData()
    {
        switch(currentWeaponType)
        {
            case "Sword":
                return weaponData[0];
            case "Staff":
                return weaponData[1];
            default:
                Debug.Log("예외 발생. 확인할 것");
                return null;
        }

    }
}
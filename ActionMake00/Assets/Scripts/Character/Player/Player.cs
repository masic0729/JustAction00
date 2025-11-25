using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Experimental.Rendering;



public class Player : Character
{
    PlayerController playerCtrl;
    PlayerLevelUp playerLevelUp;
    SkillManager skillManager;
    [SerializeField] EquipmentManager equipmentManager;
    [SerializeField] int level = 1;
    [SerializeField] int[] needExp;
    [SerializeField] const int maxLevel = 3;                              //플레이어의 체대 레벨은 3레벨이다



    protected Transform weaponTransform;

    [Header("Physics check info")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;
    protected string weaponTypeString;

    
    // 초기화
    protected override void Start()
    {
        base.Start();
        Init();
    }

    protected override void Init()
    {
        base.Init();
        currentExp = 0;
        playerCtrl = GetComponent<PlayerController>();
        playerLevelUp = GetComponent<PlayerLevelUp>();

        hitAction += WeaponColDisable;

        transform.tag = "Player";
        hp = 100;
        rotateSpeed = 20f;

        /*weaponDic[WeaponType.Sword.ToString()] = null;
        weaponDic[WeaponType.Staff.ToString()] = null;*/


    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.T))
        {
            ExpUp(1);
        }
    }

    /*public void WeaponInit(string weaponName)
    {
        weaponTransform = FindTransformAtChild("PlayerWeapon");

        for (int i = 0; i < weapon.Length; i++)
        {
            PlayerWeapon playerWeapon = weapon[i].GetComponent<PlayerWeapon>();

            weaponDic[playerWeapon.weaponType.ToString()] = Instantiate(weapon[i].gameObject, weaponTransform.position, weaponTransform.rotation).GetComponent<Weapon>();
            weaponDic[playerWeapon.weaponType.ToString()].gameObject.SetActive(false);
            weaponDic[playerWeapon.weaponType.ToString()].transform.parent = weaponTransform;
            weaponDic[playerWeapon.weaponType.ToString()].SetDamage(damage);
        }
        weaponTypeString = weaponName;
        skillManager.SetCurrentWeaponType(weaponTypeString);
        skillManager.WeaponSkillLoad(weaponTypeString);

        weaponDic[weaponTypeString].gameObject.SetActive(true);

    }*/

    /// <summary>
    /// 새로 제작한 무기 초기화 함수로,
    /// 해당 기능은 무기 공격력을 적용 직후에 실행해야 한다
    /// </summary>
    /// <param name="weapon"></param>
    public void WeaponInit(PlayerWeapon weapon)
    {
        
        //현재 플레이어의 무기가 존재하면, 해당 무기 삭제
        if (weaponDic[weaponTypeString] != null && weaponTypeString != null)
        {
            Destroy(weaponDic[weaponTypeString].gameObject);
        }

        weaponTypeString = weapon.weaponType.ToString();

        weaponTransform = FindTransformAtChild("PlayerWeapon");
        weaponDic[weaponTypeString] = Instantiate(weapon.gameObject, weaponTransform.position, weaponTransform.rotation).GetComponent<PlayerWeapon>();
        weaponDic[weaponTypeString].transform.parent = weaponTransform;
        weaponDic[weaponTypeString].SetDamage(GetResultDamage());
        WeaponInitDelay();


        PlayerStatUI playerUI = GetComponent<PlayerStatUI>();
        playerUI.UpdateSkillIcon();
    }

    /// <summary>
    /// 게임을 시작할 때 실행된다.
    /// 추후 json기반으로 확장할 때 실행 구조가 변동될 예정이다.
    /// </summary>
    /// <param name="weapon"></param>
    public void WeaponAwakeInit(PlayerWeapon weapon)
    {
        weaponTypeString = weapon.weaponType.ToString();

        weaponTransform = FindTransformAtChild("PlayerWeapon");
        weaponDic[weaponTypeString] = Instantiate(weapon.gameObject, weaponTransform.position, weaponTransform.rotation).GetComponent<PlayerWeapon>();
        weaponDic[weaponTypeString].transform.parent = weaponTransform;
        weaponDic[weaponTypeString].SetDamage(GetResultDamage());
        WeaponInitDelay();

        PlayerStatUI playerUI = GetComponent<PlayerStatUI>();
        playerUI.UpdateSkillIcon();
    }

    void WeaponInitDelay()
    {
        if (skillManager == null)
            skillManager = GetComponent<SkillManager>();

        skillManager.SetCurrentWeaponType(weaponTypeString);
        skillManager.WeaponSkillLoad(weaponTypeString);
    }

    public void WeaponColDisable()
    {
        if(weaponTypeString != null && weaponDic != null)
        {
            weaponDic[weaponTypeString].ResetColiderDisable();

        }
    }

    public void TransWeapon(string weaponName)
    {
        string weaponType = weaponName;
        //같은 무기 타입이면 그냥 반환한다. 바꿀 이유가 없기 때문이다.
        if (this.weaponTypeString == weaponType)
            return;
        //기존에 쓰던 무기는 비활성화
        if (weaponDic[this.weaponTypeString] != null)
        {
            weaponDic[this.weaponTypeString].gameObject.SetActive(false);
        }

        this.weaponTypeString = weaponName;

        //무기 타입에 맞는 스킬데이터 설정
        skillManager.SetCurrentWeaponType(weaponType);
        skillManager.WeaponSkillLoad(weaponType);

        weaponDic[weaponType].gameObject.SetActive(true);
        
    }

    public override void TakeDamage(float amount, Character attacker, int hitLevel = -1)
    {
        hitAction();

        if (isIgnoreDamage == true)
        {
            Debug.Log(this.gameObject.name + " 무적 : " + transform.name);
            return;
        }

        

        base.TakeDamage(amount, attacker, hitLevel);
        if (hitLevel == -1)
            return;

        if (isSuperArmor == false || hitLevel != -1)
        {
            anim.SetTrigger("Hit");
            WeaponColDisable();
        }

        CameraController.instance.PlayCameraShake();                    //피격 시 카메라 다소 흔들림
        playerCtrl.SetCanAttackInput(false);
    }

    /// <summary>
    /// 플레이어는 적을 처치 시 경험치를 획득한다.
    /// 획득할 때, 만약 최대 경험치를 초과한 경험치를 받는다면,
    /// 우선 레벨업 후 나머지 경험치를 임지 저장 하여 마저 획득할 예정이다. 
    /// </summary>
    public void ExpUp(int getExp)
    {
        //추후 UI_Manager를 통해 경험치 바 상태 최신화 요구
        if(level == maxLevel)
        {
            Debug.Log("최대 레벨이므로, 경험치 획득이 제한됩니다.");
            return;
        }
        currentExp += getExp;

        //경험치
        int overExpValue = 0;

        //이후 일단 레벨업이 되는 지 확인후 레벨업 처리하자마자, 오버되는 경험치를 확인
        if(currentExp >= needExp[level - 1])
        {
            overExpValue = currentExp - needExp[level - 1];

            level++;
            playerLevelUp.LevelUp();
            currentExp = 0;
            ExpUp(overExpValue);

        }
        Debug.Log(level + "경험치가 오르긴 했어요~" + currentExp);

    }

    /// <summary>
    /// 레벨업에 의한 새로운 스킬을 획득하는 함수.
    /// E 스킬을 고정으로 활성화 한다.
    /// </summary>
    public void LevelUpForSkillOpen()
    {
        Debug.Log("스킬 해금해야함");
        playerCtrl.SetCanInputQ();

        //이뿐만 아니라 일정 주기로 스킬 또는 공격 성공 시,
        //해당 충돌 위치에 추가 공격 발동.
        //해당 공격은 원형으로 터지는 스킬 형태로 발동될 예정.
        //레퍼런스는 아델, 아크, 로아의 보주효과, 롤의 스태틱 등 존재한다.



        //이후 UI 상에서 Q스킬 사용할 수 있음을 표시할 것
    }

    /// <summary>
    /// 레벨업에 의해 플레이어의 모든 능력치가 소폭 상승한다
    /// </summary>
    public void LevelUpForStatUp()
    {
        Debug.Log("수치 상승해야함");

        float statUpValue = 5f;
        statDatas[(int)AddStatName.LevelUp].Damage += statUpValue;
        statDatas[(int)AddStatName.LevelUp].Defense += statUpValue;
        //statDatas[(int)AddStatName.LevelUp].MoveSpeed += statUpValue;
        statDatas[(int)AddStatName.LevelUp].MaxHp += statUpValue;

        equipmentManager.UpdateCharacterStatResult();

        //모든 스텟 조정 후, 체력의 경우 현재 체력을 상승한 최대 체력 만큼 상승한다
        SetHp(GetHp() + statUpValue);
    }



    public void SetWeaponType(string typeName) => weaponTypeString = typeName;

    public string GetWeaponType() => weaponTypeString;

    public int GetLevel() => level;

    public int GetNeedExp() => needExp[level - 1];

}
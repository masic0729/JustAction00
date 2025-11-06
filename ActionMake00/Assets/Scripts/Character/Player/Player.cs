using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player : Character
{
    PlayerController playerCtrl;
    SkillManager skillManager;
    Vector3 moveVector;
    //public Dictionary<string, GameObject> weaponsDic;

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

        playerCtrl = GetComponent<PlayerController>();
        skillManager = GetComponent<SkillManager>();
        hitAction += WeaponColDisable;

        transform.tag = "Player";
        hp = 100;
        rotateSpeed = 20f;
        //WeaponInit("Sword");
    }

    protected override void Update()
    {
        base.Update();

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
        if (weaponTypeString != null)
        {
            Destroy(weaponDic[weaponTypeString].gameObject);
        }

        weaponTypeString = weapon.weaponType.ToString();

        weaponTransform = FindTransformAtChild("PlayerWeapon");
        weaponDic[weaponTypeString] = Instantiate(weapon.gameObject, weaponTransform.position, weaponTransform.rotation).GetComponent<PlayerWeapon>();

        weaponDic[weaponTypeString].transform.parent = weaponTransform;
        weaponDic[weaponTypeString].SetDamage(GetResultDamage());

        Invoke("WeaponInitDelay", 0.1f);
    }

    void WeaponInitDelay()
    {
        skillManager.SetCurrentWeaponType(weaponTypeString);
        skillManager.WeaponSkillLoad(weaponTypeString);
    }

    public void WeaponColDisable()
    {
        weaponDic[weaponTypeString].ResetColiderDisable();
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

    public void SetWeaponType(string typeName) => weaponTypeString = typeName;

    public string GetWeaponType() => weaponTypeString;
}
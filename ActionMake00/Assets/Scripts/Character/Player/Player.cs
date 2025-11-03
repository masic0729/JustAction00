using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player : Character
{
    PlayerController playerCtrl;
    SkillManager skillManager;
    Vector3 moveVector;
    //public Dictionary<string, GameObject> weaponsDic;

    private Transform weaponTransform;

    [Header("Physics check info")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;
    string weaponType;

    // 초기화
    protected override void Start()
    {
        base.Start();
        Init();
    }

    protected override void Init()
    {
        base.Init();
        hitAction += WeaponColDisable;

        playerCtrl = GetComponent<PlayerController>();
        skillManager = GetComponent<SkillManager>();
        transform.tag = "Player";
        hp = 100;
        rotateSpeed = 20f;
        WeaponInit();

    }

    protected override void Update()
    {
        base.Update();

    }

    void WeaponInit()
    {
        weaponTransform = FindTransformAtChild("PlayerWeapon");


        //weaponsDic = new Dictionary<string, GameObject>();
        for (int i = 0; i < weapon.Length; i++)
        {
            weaponDic[weapon[i].gameObject.name] = Instantiate(weapon[i].gameObject, weaponTransform.position, weaponTransform.rotation).GetComponent<Weapon>();
            weaponDic[weapon[i].gameObject.name].gameObject.SetActive(false);
            weaponDic[weapon[i].gameObject.name].transform.parent = weaponTransform;
            weaponDic[weapon[i].gameObject.name].SetDamage(damage);
        }
        weaponType = "PlayerSword";
        //skillManager.SkillDataInit();
        skillManager.SetCurrentWeaponType(weaponType);
        skillManager.WeaponSkillLoad(weaponType);

        weaponDic[weaponType].gameObject.SetActive(true);

    }

    public void WeaponColDisable()
    {
        weaponDic[weaponType].ResetColiderDisable();
    }

    public void TransWeapon(string weaponName)
    {
        string weaponType = weaponName;
        //같은 무기 타입이면 그냥 반환한다. 바꿀 이유가 없기 때문이다.
        if (this.weaponType == weaponType)
            return;
        //기존에 쓰던 무기는 비활성화
        if (weaponDic[this.weaponType] != null)
        {
            weaponDic[this.weaponType].gameObject.SetActive(false);
        }

        this.weaponType = weaponName;

        //무기 타입에 맞는 스킬데이터 설정
        skillManager.SetCurrentWeaponType(weaponType);
        skillManager.WeaponSkillLoad(weaponType);

        weaponDic[weaponType].gameObject.SetActive(true);
        
    }

    public override void TakeDamage(float amount, int hitLevel = -1)
    {
        hitAction();

        if (isIgnoreDamage == true)
        {
            Debug.Log(this.gameObject.name + " 무적 : " + transform.name);
            return;
        }

        

        base.TakeDamage(amount, hitLevel);
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

    public void SetWeaponType(string typeName) => weaponType = typeName;

    public string GetWeaponType() => weaponType;
}
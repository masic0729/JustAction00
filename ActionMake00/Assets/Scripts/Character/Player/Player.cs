using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    PlayerController playerCtrl;
    SkillManager skillManager;
    Vector3 moveVector;


    private Transform weaponTransform;

    [Header("Physics check info")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;


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
        skillManager.SetCurrentWeaponType("Sword");
        commonDamage = 10;
        weaponTransform = FindTransformAtChild("PlayerWeapon");

        weaponDic["PlayerWeapon"] = Instantiate(weaponDic["PlayerWeapon"], weaponTransform.position, weaponTransform.rotation);
        weaponDic["PlayerWeapon"].transform.parent = weaponTransform;
        weaponDic["PlayerWeapon"].SetDamage(commonDamage);
    }

    public override void TakeDamage(int amount, int hitLevel = -1)
    {
        hitAction();

        if (isIgnoreDamage == true)
        {
            Debug.Log(this.gameObject.name + " 무적 : " + transform.name);
            return;
        }

        if (isSuperArmor == false || hitLevel != -1)
        {
            anim.SetTrigger("Hit");
        }

        base.TakeDamage(amount, hitLevel);
        if (hitLevel == -1)
            return;
        CameraController.instance.PlayCameraShake();                    //피격 시 카메라 다소 흔들림
        playerCtrl.SetCanInput(false);
    }

    
}
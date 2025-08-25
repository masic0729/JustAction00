using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Character
{
    [Header("Character default info")]
    [HideInInspector]
    private Vector3 moveVector;

    private Transform weaponTransform;

    [Header("Physics check info")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    float h, v;
    float rotateSpeed = 20f;
    float jumpPower = 5f;

    public IEnumerator sprintCoroutine;
    [SerializeField]float sprintTime;
    [SerializeField]float sprintSpeed;
    [SerializeField] float sprintCoolTime, sprintCoolTimer;

    int comboAttackIndex = 0;

    bool isGround = true;
    bool canCombo = false;
    public bool isEscapeAttackAnim = false;
    bool canInput = true;

    // 초기화
    protected override void Start()
    {
        base.Start();
        Init();
    }

    protected override void Init()
    {
        base.Init();
       
        hp = 100;

        WeaponInit();
        // 기본 무기 히트박스는 비활성화
        /*BoxCollider hitCol = weapon.gameObject.GetComponent<BoxCollider>();
        if (hitCol != null) hitCol.enabled = false;*/
    }

    protected override void Update()
    {
        base.Update();
        PlayerMove();
        PlayerAttack();
        ActionCoolTimer();
        CheckGround();
    }

    void WeaponInit()
    {
        commonDamage = 10;

        weaponTransform = FindTransformAtChild("PlayerWeapon");
        weaponDic["PlayerWeapon"] = Instantiate(weapon[0], weaponTransform.position, weaponTransform.rotation);
        weaponDic["PlayerWeapon"].transform.parent = weaponTransform;
        weaponDic["PlayerWeapon"].SetDamage(commonDamage);
    }

    

    void PlayerMove()
    {
        PlayerEscape();

        if (canInput == false)
            return;

        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        moveVector = new Vector3(h, 0, v);

        if (anim.GetBool("isAttacking") == false || isEscapeAttackAnim)
        {
            
            if (anim.GetBool("isAttacking") == true)
                anim.SetBool("isAttacking", false);

            
            anim.SetFloat("moveValue", moveVector.magnitude);
            // 이동 처리 (중복 제거)
            if (moveVector.magnitude > 0.1f)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveVector);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
                anim.SetBool("Move", true);

            }
            else
            {
                anim.SetBool("Move", false);

            }

            if (anim.GetBool("isSprint") == true)
            {
                transform.position += moveVector.normalized * sprintSpeed * Time.deltaTime;
                transform.Translate(0, 0, sprintSpeed * Time.deltaTime);
            }
            else
            {
                transform.position += moveVector.normalized * moveSpeed * Time.deltaTime;

            }


            // 점프
            /*if (Input.GetKeyDown(KeyCode.Space) && isGround)
            {
                CharacterJump();
            }*/
        }

        
    }

    void PlayerAttack()
    {
        if (canInput == false)
            return;

        // 공격
        if (Input.GetMouseButtonDown(0))
        {
            if (!anim.GetBool("isAttacking"))
            {
                anim.SetTrigger("Attack");
                anim.SetBool("isAttacking", true);
                isEscapeAttackAnim = false;
            }
            else if (canCombo && anim.GetBool("isAttacking"))                       //if(canCombo)
            {
                anim.SetBool("isReAttack", true);
            }
        }
    }

    void ActionCoolTimer()
    {
        sprintCoolTimer -= Time.deltaTime;
    }

    void PlayerEscape()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && sprintCoolTimer <= 0)
        {
            sprintCoroutine = Sprint();

            sprintCoolTimer = sprintCoolTime;
            StopCoroutine(sprintCoroutine);
            StartCoroutine(sprintCoroutine);
        }
    }

    public override void TakeDamage(int amount, int hitLevel = -1)
    {
        if (isIgnoreDamage == true)
        {
            Debug.Log("이건 무적 : " + transform.name);
            return;
        }
        base.TakeDamage(amount, hitLevel);
        
        CameraController.instance.PlayCameraShake();                    //피격 시 카메라 다소 흔들림
        canInput = false;
        //anim.SetTrigger("Hit");
    }

    void CheckGround()
    {
        isGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    IEnumerator Sprint()
    {
        anim.SetTrigger("Sprint");
        anim.SetBool("isSprint", true);
        isIgnoreDamage = true;
        yield return new WaitForSeconds(sprintTime);
        isIgnoreDamage = false;
        anim.SetBool("isSprint", false);

    }


    void CharacterJump()
    {
        isGround = false;
        rb.AddForce(Vector3.up * jumpPower * 2, ForceMode.Impulse);
        anim.SetTrigger("Jump");
    }

    

    /// <summary>
    /// 콤보 타이밍 허용 (애니메이션 이벤트에서 호출)
    /// </summary>
    public void EnableCombo()
    {
        canCombo = true;
        //StartCoroutine(ComboResetTimer());
    }

    public void DisableCombo() => canCombo = false;

    public void InterruptCombo()
    {
        anim.SetBool("isAttacking", false);
        anim.SetBool("isReAttack", false);
        canCombo = false;
    }



    
    /*public void DisableHitbox()
    {
        weapon.GetComponent<BoxCollider>().enabled = false;
    } */

    public void TransIdleState()
    {
        anim.SetBool("isAttacking", false);
        canInput = true;
    }

    public void EscapeAttackAnim() => isEscapeAttackAnim = true;

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }

    public void SetComboAttackIndex(int value) => comboAttackIndex = value;

    public int GetComboAttackIndex() => comboAttackIndex;
}
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
        /*BoxCollider col = weapon.gameObject.GetComponent<BoxCollider>();
        if (col != null) col.enabled = false;*/
    }

    protected override void Update()
    {
        base.Update();
        PlayerInput();
        CheckGround();
    }

    void WeaponInit()
    {
        weaponTransform = FindTransformAtChild("Weapon");
        weapon = Instantiate(weapon, weaponTransform.position, weaponTransform.rotation);
        weapon.transform.parent = weaponTransform;
        commonDamage = 10;

        weapon.GetComponent<Weapon>().SetDamage(commonDamage);
    }

    void PlayerInput()
    {
        if (canInput == false)
            return;

        if(anim.GetBool("isAttacking") == false || isEscapeAttackAnim)
        {
            
            if (anim.GetBool("isAttacking") == true)
                anim.SetBool("isAttacking", false);
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
            moveVector = new Vector3(h, 0, v);

            anim.SetFloat("moveValue", moveVector.magnitude);

            // 이동 처리 (중복 제거)
            if (moveVector.magnitude > 0.1f)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveVector);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
                transform.position += moveVector.normalized * moveSpeed * Time.deltaTime;
            }

            // 점프
            if (Input.GetKeyDown(KeyCode.Space) && isGround)
            {
                CharacterJump();
            }
        }

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

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        Debug.Log("플레이어 피해 받음");
        canInput = false;
        anim.SetTrigger("Hit");
    }

    void CheckGround()
    {
        isGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
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
}
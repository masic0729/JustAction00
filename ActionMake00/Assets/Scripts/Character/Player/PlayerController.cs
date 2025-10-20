using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject mainCamera;
    SkillManager skillManager;
    Player player;
    [Header("Character default info")]
    private Vector3 moveVector;

    float h, v;

    public IEnumerator sprintCoroutine;
    [SerializeField]float sprintTime;
    [SerializeField]float sprintSpeed;
    [SerializeField]float sprintCoolTime, sprintCoolTimer;

    int comboAttackIndex = 0;

    //bool isGround = true;
    bool canCombo = false;
    public bool isEscapeAttackAnim = false;
    [SerializeField]
    bool canAttackInput = true;
    bool canAnyInput = true;

    // 초기화
    void Start()
    {
        player = GetComponent<Player>();
        Init();
    }


    void Init()
    {
        transform.tag = "Player";
        skillManager = GetComponent<SkillManager>();
    }


    private void Update()
    {
        if (canAnyInput == false)
            return;
        PlayerEscape();
        WeapontestSwap();


        if (canAttackInput == false)
            return;
        PlayerAttack();
        PlayerSkillInput();
        MoveInput();

    }
    void FixedUpdate()
    {
        ActionCoolTimer();

    }


    void MoveInput()
    {
        Vector3 cameraVec = mainCamera.transform.position;

        

        // ▼▼ 여기만 카메라 기준으로 수정 ▼▼
        Transform camT = mainCamera.transform;
        Vector3 camFwd = Vector3.ProjectOnPlane(camT.forward, Vector3.up).normalized; // 카메라 전방(수평)
        Vector3 camRight = Vector3.ProjectOnPlane(camT.right, Vector3.up).normalized; // 카메라 우측(수평)
        moveVector = (camRight * h + camFwd * v);
                                                  

        if (player.anim.GetBool("isAttacking") == false)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
            if (player.anim.GetBool("isAttacking") == true)
                player.anim.SetBool("isAttacking", false);

            player.anim.SetFloat("moveValue", moveVector.magnitude);

            if (moveVector.magnitude > 0.1f)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveVector);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, player.GetRotateSpeed() * Time.deltaTime);
                player.anim.SetBool("Move", true);
            }
            else
            {
                player.anim.SetBool("Move", false);
            }

            if (player.anim.GetBool("isSprint") == true)
            {
                transform.position += moveVector.normalized * sprintSpeed * Time.deltaTime;
                transform.Translate(0, 0, sprintSpeed * Time.deltaTime);
            }
            else
            {
                transform.position += moveVector.normalized * player.GetMoveSpeed() * Time.deltaTime;
            }
        }
    }

    void WeapontestSwap()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            if (player.GetWeaponType() == "PlayerSword")
            {
                player.TransWeapon("PlayerStaff");
            }
            else if (player.GetWeaponType() == "PlayerStaff")
            {
                player.TransWeapon("PlayerSword");
            }
        }
    }



    void PlayerAttack()
    {
        /*if (canInput == false)
            return;*/
        // 공격
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (player.anim.GetBool("isAttacking") == false)
            {
                player.anim.SetTrigger("Attack");
                player.anim.SetBool("isAttacking", true);
                isEscapeAttackAnim = false;
            }

            if (canCombo && player.anim.GetBool("isAttacking") && player.GetWeaponType() == "PlayerSword")                       //if(canCombo)
            {
                player.anim.SetBool("isReAttack", true);
            }
        }
    }

    void PlayerSkillInput()
    {
        if(Input.GetKeyDown(KeyCode.E) && skillManager.isSkillCanUse("Skill0"))
        {
            player.anim.SetTrigger("Skill0");
            canAttackInput = false;
        }

        /*if (Input.GetKeyDown(KeyCode.Q) && skillManager.isSkillCanUse("Skill0"))
        {
            player.anim.SetTrigger("Skill0");
            canInput = false;
        }*/
        if (Input.GetKeyDown(KeyCode.Q))
        {
            player.anim.SetTrigger("Skill1");
            canAttackInput = false;
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



    IEnumerator Sprint()
    {
        player.hitAction();

        canAttackInput = true;
        player.anim.SetTrigger("Sprint");
        player.anim.SetBool("isSprint", true);
        player.SetIsIgnoreDamage(true);
        yield return new WaitForSeconds(sprintTime);
        player.SetIsIgnoreDamage(false);
        player.anim.SetBool("isSprint", false);

    }

    public void DisableCombo() => canCombo = false;



    /// <summary>
    /// 콤보 타이밍 허용 (애니메이션 이벤트에서 호출)
    /// </summary>
    public void EnableCombo()
    {
        canCombo = true;
    }


    public void InterruptCombo()
    {
        player.anim.SetBool("isAttacking", false);
        player.anim.SetBool("isReAttack", false);
        canCombo = false;
    }

    public void TransIdleState()
    {
        player.anim.SetBool("isAttacking", false);
        canAttackInput = true;
    }


    public void EscapeAttackAnim() => isEscapeAttackAnim = true;


    public void SetComboAttackIndex(int value) => comboAttackIndex = value;

    public int GetComboAttackIndex() => comboAttackIndex;

    public void SetCanAttackInput(bool state) => canAttackInput = state;
    public bool GetCanAttackInput() => canAttackInput;

    public void SetCanAnyInput(bool state) => canAnyInput = state;

    public bool GetCanAnyInput() => canAnyInput;
}
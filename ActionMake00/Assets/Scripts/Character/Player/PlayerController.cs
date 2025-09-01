using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Player player;
    [Header("Character default info")]
    [HideInInspector]
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
    bool canInput = true;

    // 초기화
    void Start()
    {
        player = GetComponent<Player>();
        Init();
    }


    void Init()
    {
        transform.tag = "Player";

    }


    private void Update()
    {
        PlayerAttack();
        PlayerEscape();

        MoveInput();
        ActionCoolTimer();
        //CheckGround();

    }



    void MoveInput()
    {

        if (canInput == false)
            return;

        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        moveVector = new Vector3(h, 0, v);

        if (player.anim.GetBool("isAttacking") == false || isEscapeAttackAnim)
        {
            
            if (player.anim.GetBool("isAttacking") == true)
                player.anim.SetBool("isAttacking", false);


            player.anim.SetFloat("moveValue", moveVector.magnitude);
            // 이동 처리 (중복 제거)
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

    void PlayerAttack()
    {
        if (canInput == false)
            return;
        // 공격
        if (Input.GetMouseButtonDown(0))
        {
            if (player.anim.GetBool("isAttacking") == false)
            {
                player.anim.SetTrigger("Attack");
                player.anim.SetBool("isAttacking", true);
                isEscapeAttackAnim = false;
            }

            if (canCombo && player.anim.GetBool("isAttacking"))                       //if(canCombo)
            {
                player.anim.SetBool("isReAttack", true);
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



    IEnumerator Sprint()
    {
        canInput = true;
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
        canInput = true;
    }


    public void EscapeAttackAnim() => isEscapeAttackAnim = true;


    public void SetComboAttackIndex(int value) => comboAttackIndex = value;

    public int GetComboAttackIndex() => comboAttackIndex;

    public void SetCanInput(bool state) => canInput = state;
    public bool GetConInput() => canInput;
}
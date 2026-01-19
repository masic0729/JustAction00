using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

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
    bool canAttackInput = true;                                                 //기본조작은 가능하나, 공격할 수 있는 지 확인하는 용도
    bool canAnyInput = true;                                                    //어쨋든 플레이어가 입력할 수 있는 지 확인한다. 보통 상태이상에 의해 움직이지 못하는 경우도 있다
    bool canKeyQ = false;                                                        //Q스킬을 사용할 수 있는 여부. 
    bool canInteraction = false;                                                 //플레이어의 상호작용 여부
    bool isInteracting = false;                                                   //상호작용 중인 지 따지는 데이터. 활성화 시 중첩 상호작용이 되지 않는다

    bool isGameEnd = false;                                                 //플레이어 사망 시 그 어떤 기능도 이용할 수 없음

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
        //플레이어 사망 시 조작 불가
        if (isGameEnd == true)
            return;

        InteractionNPC();

        if (player.anim.GetBool("isStating") == true)
            return;


        {
            
            

            PlayerEscape();
            WeapontestSwap();
        }

        {

            if (canAnyInput == false)   //여기 부분이 통제구역
            {
                //기본적으로 어떠한 이유로 입력이 통제되어야 하는 기능을 위해 수시로 처리해놓는다.
                player.anim.SetBool("Move", false);
                player.anim.SetFloat("moveValue", 0f);

                moveVector = Vector3.zero;

                h = 0f;
                v = 0f;
                return;
            }


            MoveInput();

            if (canAttackInput == false)
                return;

            PlayerAttack();
            PlayerSkillInput();
        }
    }

    void FixedUpdate()
    {
        ActionCoolTimer();
    }

    void InteractionNPC()
    {
        //기본적으로 상호작용할 수 있는 지 확인하고, 이미 상호작용 중인 지 확인한다
        if (Input.GetKeyDown(KeyCode.F) && canInteraction == true && isInteracting == false)
        {
            //상호작용 중이고, 이에 따라 중첩 상호작용을 막는다.
            isInteracting = true;
            GUI_PlayerInput.instance.EnableUI(GUI_PlayerInput.instance.NPC_InventoryView);
            //GetComponent<RayCastToNPC>().GetNpc().ShowView();         //여기에는 해당 인벤토리 뷰에 아이템 데이터를 넣어야함
        }
    }

    /// <summary>
    /// 상호작용을 어쨋든 다시 실행하게 만드는 함수
    /// </summary>
    public void SetCanIsInteraction()
    {
        isInteracting = false;
    }

    void MoveInput()
    {
        
        //카메라 관리 부분
        Vector3 cameraVec = mainCamera.transform.position;

        Transform camT = mainCamera.transform;
        Vector3 camFwd = Vector3.ProjectOnPlane(camT.forward, Vector3.up).normalized; // 카메라 전방(수평)
        Vector3 camRight = Vector3.ProjectOnPlane(camT.right, Vector3.up).normalized; // 카메라 우측(수평)

        

        if (player.anim.GetBool("isAttacking") == false)
        {
            moveVector = (camRight * h + camFwd * v);

            if (player.anim.GetBool("isAttacking") == true)
                player.anim.SetBool("isAttacking", false);

            player.anim.SetFloat("moveValue", moveVector.magnitude);

            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            

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
                transform.position += moveVector.normalized * player.GetResultMoveSpeed() * Time.deltaTime;
            }
        }
    }
    

    void WeapontestSwap()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            if (player.GetWeaponType() == "Sword")
            {
                player.TransWeapon("Staff");
            }
            else if (player.GetWeaponType() == "Staff")
            {
                player.TransWeapon("Sword");
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

            if (canCombo && player.anim.GetBool("isAttacking") && player.GetWeaponType() == "Sword")                       //if(canCombo)
            {
                player.anim.SetBool("isReAttack", true);
            }
        }
    }

    public void SetCanInputQ()
    {
        canKeyQ = true;
    }

    void PlayerSkillInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && skillManager.isSkillCanUse("Skill0"))
        {
            player.anim.SetBool("isAttacking", true);

            player.anim.SetTrigger("Skill0");
            canAttackInput = false;
            skillManager.SetSkillCoolTime(0);
        }
/*
        if (Input.GetKeyDown(KeyCode.E))
        {
            player.anim.SetBool("isAttacking", true);

            player.anim.SetTrigger("Skill0");
            canAttackInput = false;
            skillManager.SetSkillCoolTime(0);
        }*/

        /*if (Input.GetKeyDown(KeyCode.Q) && skillManager.isSkillCanUse("Skill0"))
        {
            player.anim.SetTrigger("Skill0");
            canInput = false;
        }*/
        if (Input.GetKeyDown(KeyCode.Q) && skillManager.isSkillCanUse("Skill1") && canKeyQ == true)
        {
            player.anim.SetBool("isAttacking", true);

            player.anim.SetTrigger("Skill1");
            canAttackInput = false;
            skillManager.SetSkillCoolTime(1);
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
        player.onHitAction();

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

    public bool GetCanInteraction() => canInteraction;

    public void SetCanInteraction(bool state) => canInteraction = state;

    public void SetIsGameEnd(bool state) => isGameEnd = state;
}
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

// 플레이어의 입력을 받아 이동, 공격, 스킬, 대쉬, 상호작용 등을 처리하는 컨트롤러
public class PlayerController : MonoBehaviour
{
    // 메인 카메라 오브젝트 참조 - 카메라 기준 이동 방향 계산에 사용
    public GameObject mainCamera;

    // 스킬 관리 컴포넌트 참조
    SkillManager skillManager;

    // 플레이어 스탯 및 상태 관리 컴포넌트 참조
    Player player;

    // 물리 기반 이동 및 충돌 처리를 위한 Rigidbody 참조
    private Rigidbody rb;

    [Header("Character default info")]

    // 카메라 기준으로 계산된 이동 벡터
    private Vector3 moveVector;

    // 수평(좌우) 입력값
    float h;

    // 수직(앞뒤) 입력값
    float v;

    // 현재 실행 중인 대쉬 코루틴 참조 - 중복 실행 방지 및 중단 제어용
    public IEnumerator sprintCoroutine;

    // 대쉬가 지속되는 시간(초)
    [SerializeField] float sprintTime;

    // 대쉬 중 이동 속도
    [SerializeField] float sprintSpeed;

    // 대쉬 쿨타임 총량
    [SerializeField] float sprintCoolTime;

    // 현재 남은 대쉬 쿨타임 타이머 - 0 이하가 되면 재사용 가능
    [SerializeField] float sprintCoolTimer;

    // 대쉬 시작 시점에 저장한 이동 방향 - 대쉬 중 방향 전환의 기준값
    private Vector3 dashDirection;

    // 대쉬 진행 여부 플래그 - true일 때 FixedUpdate에서 물리 기반 이동 실행
    private bool isDashing = false;

    // 대쉬 중 방향키 입력에 의해 방향이 전환되는 회전 속도
    [SerializeField] float dashRotateSpeed = 5f;

    // 현재 콤보 공격 인덱스
    int comboAttackIndex = 0;

    // 콤보 입력 허용 여부 - 애니메이션 이벤트를 통해 제어됨
    bool canCombo = false;

    // 공격 애니메이션에서 이탈했는 지 여부
    public bool isEscapeAttackAnim = false;

    // 공격 입력 가능 여부 - 기본 조작은 가능하나 공격 가능 상태인지 확인하는 용도
    [SerializeField]
    bool canAttackInput = true;

    // 전체 입력 가능 여부 - 상태이상 등으로 움직이지 못하는 경우에도 사용
    bool canAnyInput = true;

    // Q 스킬 사용 가능 여부
    bool canKeyQ = false;

    // 상호작용 가능 범위 안에 있는 지 여부
    bool canInteraction = false;

    // 현재 상호작용 진행 중 여부 - 중첩 상호작용 방지용
    bool isInteracting = false;

    // 플레이어 사망 여부 - true이면 모든 입력 차단
    bool isGameEnd = false;


    // 초기화
    void Start()
    {
        player = GetComponent<Player>();
        Init();
    }


    // 컴포넌트 참조 초기화 함수
    void Init()
    {
        transform.tag = "Player";
        skillManager = GetComponent<SkillManager>();

        // 물리 기반 이동을 위한 Rigidbody 캐싱
        rb = GetComponent<Rigidbody>();
    }


    // 매 프레임 입력 처리 및 상태 분기
    private void Update()
    {
        // 플레이어 사망 시 조작 불가
        if (isGameEnd == true)
        {
            player.anim.SetFloat("moveValue", 0f);
            return;
        }

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
                // 기본적으로 어떠한 이유로 입력이 통제되어야 하는 기능을 위해 수시로 처리해놓는다.
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


    // 물리 연산 주기 - 쿨타임 감소 및 대쉬 이동 처리
    void FixedUpdate()
    {
        ActionCoolTimer();

        // 대쉬 진행 중일 때만 물리 기반 이동 실행
        if (isDashing)
            DashMove();
    }


    // NPC 상호작용 입력 처리 함수
    void InteractionNPC()
    {
        // 기본적으로 상호작용할 수 있는 지 확인하고, 이미 상호작용 중인 지 확인한다
        if (Input.GetKeyDown(KeyCode.F) && canInteraction == true && isInteracting == false)
        {
            // 상호작용 중이고, 이에 따라 중첩 상호작용을 막는다.
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


    // 카메라 기준 이동 입력 처리 함수 - 대쉬 중에는 이동 입력만 수집하고 실제 이동은 DashMove가 담당
    void MoveInput()
    {
        // 카메라 관리 부분
        Vector3 cameraVec = mainCamera.transform.position;

        Transform camT = mainCamera.transform;
        Vector3 camFwd = Vector3.ProjectOnPlane(camT.forward, Vector3.up).normalized; // 카메라 전방(수평)
        Vector3 camRight = Vector3.ProjectOnPlane(camT.right, Vector3.up).normalized; // 카메라 우측(수평)

        if (player.anim.GetBool("isAttacking") == false)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            moveVector = (camRight * h + camFwd * v);

            if (player.anim.GetBool("isAttacking") == true)
                player.anim.SetBool("isAttacking", false);

            player.anim.SetFloat("moveValue", moveVector.magnitude);

            if (moveVector.magnitude > 0.1f)
            {
                // 대쉬 중이 아닐 때만 일반 회전 적용
                if (!isDashing)
                {
                    Quaternion targetRot = Quaternion.LookRotation(moveVector);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, player.GetRotateSpeed() * Time.deltaTime);
                }
                player.anim.SetBool("Move", true);
            }
            else
            {
                player.anim.SetBool("Move", false);
            }

            // 대쉬 중이 아닐 때만 일반 이동 적용 - 대쉬 이동은 DashMove에서 처리
            if (!isDashing)
            {
                transform.position += moveVector.normalized * player.GetResultMoveSpeed() * Time.deltaTime;
            }
        }
    }


    // 대쉬 중 물리 기반 이동 및 방향 전환 처리 함수 - FixedUpdate에서 호출
    void DashMove()
    {
        // 방향키 입력이 있으면 dashDirection을 입력 방향으로 점진적으로 회전
        if (moveVector.magnitude > 0.1f)
        {
            dashDirection = Vector3.Slerp(
                dashDirection,
                moveVector.normalized,
                dashRotateSpeed * Time.fixedDeltaTime
            );
        }

        // 플레이어 시각적 회전 - dashDirection 기준으로 Slerp 적용하여 즉시 꺾임 방지
        Quaternion targetRot = Quaternion.LookRotation(dashDirection);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            dashRotateSpeed * Time.fixedDeltaTime
        );

        // Rigidbody.MovePosition으로 이동 - 콜라이더 충돌을 물리 엔진이 처리하므로 벽 관통 없음
        rb.MovePosition(rb.position + dashDirection * sprintSpeed * Time.fixedDeltaTime);
    }


    // 무기 타입 교체 테스트 함수 (L키)
    void WeapontestSwap()
    {
        if (Input.GetKeyDown(KeyCode.L))
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


    // 마우스 좌클릭 공격 입력 처리 함수
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


    // Q 스킬 입력 허용 상태로 전환하는 함수
    public void SetCanInputQ()
    {
        canKeyQ = true;
    }


    // 스킬 입력 처리 함수 - E키(Skill0), Q키(Skill1)
    void PlayerSkillInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && skillManager.isSkillCanUse("Skill0"))
        {
            player.anim.SetBool("isAttacking", true);

            player.anim.SetTrigger("Skill0");
            canAttackInput = false;
            skillManager.SetSkillCoolTime(0);
        }

        if (Input.GetKeyDown(KeyCode.Q) && skillManager.isSkillCanUse("Skill1") /*&& canKeyQ == true*/)
        {
            player.anim.SetBool("isAttacking", true);

            player.anim.SetTrigger("Skill1");
            canAttackInput = false;
            skillManager.SetSkillCoolTime(1);
        }
    }


    // 대쉬 쿨타임 감소 함수 - FixedUpdate에서 매 프레임 호출
    void ActionCoolTimer()
    {
        sprintCoolTimer -= Time.deltaTime;
    }


    // LeftShift 입력을 감지하여 대쉬 코루틴을 실행하는 함수
    void PlayerEscape()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && sprintCoolTimer <= 0)
        {
            // 이전 코루틴이 남아있으면 먼저 중단한 뒤 새로 시작
            if (sprintCoroutine != null)
                StopCoroutine(sprintCoroutine);

            player.SetIsIgnoreDamage(true);
            Debug.Log(player.GetIsIgnoreDamage());
            

            DOTween.Complete("ROLL");
            DOTween.Restart("ROLL");

            sprintCoolTimer = sprintCoolTime;
            sprintCoroutine = Sprint();
            StartCoroutine(sprintCoroutine);
        }
    }


    // 대쉬 실행 코루틴 - 시작 방향 저장, 무적 처리, sprintTime 후 상태 복귀 
    IEnumerator Sprint()
    {
        player.onTransStatData();

        canAttackInput = true;
        // 대쉬 시작 시점의 바라보는 방향을 저장 - DashMove의 초기 이동 방향으로 사용
        dashDirection = transform.forward;
        isDashing = true;

        player.anim.SetTrigger("Sprint");
        player.anim.SetBool("isSprint", true);

        Debug.Log("무적 시작");
        Debug.Log(player.GetIsIgnoreDamage());
        yield return new WaitForSeconds(sprintTime);
        Debug.Log("무적 종료");

        player.SetIsIgnoreDamage(false);
        player.anim.SetBool("isSprint", false);

        // 대쉬 종료 - FixedUpdate의 DashMove 실행 중단
        isDashing = false;
    }


    // 콤보 입력을 비활성화하는 함수
    public void DisableCombo() => canCombo = false;


    /// <summary>
    /// 콤보 타이밍 허용 (애니메이션 이벤트에서 호출)
    /// </summary>
    public void EnableCombo()
    {
        canCombo = true;
    }


    // 콤보를 강제로 중단하는 함수 - 공격 상태 및 콤보 플래그 초기화
    public void InterruptCombo()
    {
        player.anim.SetBool("isAttacking", false);
        player.anim.SetBool("isReAttack", false);
        canCombo = false;
    }


    // 공격 애니메이션 종료 후 Idle 상태로 복귀하는 함수
    public void TransIdleState()
    {
        player.anim.SetBool("isAttacking", false);
        canAttackInput = true;
    }


    // 공격 애니메이션 이탈 플래그 활성화
    public void EscapeAttackAnim() => isEscapeAttackAnim = true;


    // 현재 콤보 인덱스 설정
    public void SetComboAttackIndex(int value) => comboAttackIndex = value;

    // 현재 콤보 인덱스 반환
    public int GetComboAttackIndex() => comboAttackIndex;

    // 공격 입력 가능 여부 설정
    public void SetCanAttackInput(bool state) => canAttackInput = state;

    // 공격 입력 가능 여부 반환
    public bool GetCanAttackInput() => canAttackInput;

    // 전체 입력 가능 여부 설정
    public void SetCanAnyInput(bool state) => canAnyInput = state;

    // 전체 입력 가능 여부 반환
    public bool GetCanAnyInput() => canAnyInput;

    // 상호작용 가능 여부 반환
    public bool GetCanInteraction() => canInteraction;

    // 상호작용 가능 여부 설정
    public void SetCanInteraction(bool state) => canInteraction = state;

    // 게임 종료(사망) 상태 설정
    public void SetIsGameEnd(bool state) => isGameEnd = state;
}
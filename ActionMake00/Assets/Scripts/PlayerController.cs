using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header ("Character default info")]
    private GameObject mainCamera;
    private Animator anim;
    public GameObject weapon;
    private Transform weaponTransform;
    private Rigidbody rb;
    private Vector3 moveVector;

    [Header("Physics check info")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    float h, v;
    float moveSpeed = 5f;
    float rotateSpeed = 20f;
    float jumpPower = 5f;

    bool isGround = true;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
        CheckGround();
    }

    void Init()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        weaponTransform = FindTransformAtChild("Weapon");
        weapon = Instantiate(weapon, weaponTransform.position, weaponTransform.rotation);
        weapon.transform.parent = weaponTransform;
    }

    /// <summary>
    /// 자식 오브젝트의 transform을 불러옴
    /// </summary>
    /// <param name="transformName"></param>
    /// <returns></returns>
    Transform FindTransformAtChild(string transformName)
    {
        Transform[] instance = GetComponentsInChildren<Transform>();
        foreach (Transform t in instance)
        {
            if (t.name == transformName)
            {
                return t;
            }
        }
        Debug.Log("not found");
        return null;
    }

    /// <summary>
    /// 플레이어의 이동, 공격, 점프가 존재함
    /// </summary>
    void PlayerInput()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        moveVector = new Vector3(h, 0, v);

        //이동 애니메이션에 필요한 값 입력
        anim.SetFloat("moveValue", moveVector.magnitude);

        //이동 값이 존재해야 움직인다
        if (moveVector.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveVector);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);

            // 이동 (앞 방향으로 전진)
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
            transform.Translate(moveVector * moveSpeed * Time.deltaTime);
        }

        //땅에 있어야 점프할 수 있음
        if(Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            CharacterJump();
        }

        //땅에 있어야 되며, 공격 가능상태여야 함
        if (Input.GetMouseButtonDown(0) && isGround && anim.GetBool("isAttacking") == false)
        {
            CharacterAttack();
        }
    }

    void CheckGround()
    {
        isGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    /// <summary>
    /// 캐릭터 점프
    /// </summary>
    void CharacterJump()
    {
        isGround = false;
        rb.AddForce(Vector3.up * jumpPower * 2, ForceMode.Impulse);
        anim.SetTrigger("Jump");
    }


    /// <summary>
    /// 캐릭터의 일반 공격
    /// </summary>
    void CharacterAttack()
    {
        anim.SetTrigger("Attack");
        anim.SetBool("isAttacking", true);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}

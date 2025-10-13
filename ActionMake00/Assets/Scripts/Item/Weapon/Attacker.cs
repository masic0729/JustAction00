using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum AttackType
{
    Weapon = 0,
    Projectile,
    Skill
}

public class Attacker : MonoBehaviour, IAttacker
{
    [SerializeField] protected Character owner;                                                        //공격수단의 사용자
    [SerializeField] protected AttackType attackType;
    [SerializeField] protected Collider objectCol;
    [SerializeField] protected ParticleSystem[] hitEffect;
    [SerializeField] protected float damage = 1;
    protected string target;
    protected int hitLevel = -1;
    protected string tagName;

    protected virtual void Awake()
    {
        if (objectCol == null)
        {
            objectCol = GetComponent<Collider>();
        }
        
    }

    protected virtual void Start()
    {

    }

    /// <summary>
    /// WeaponCol같은 경우 스킬스크립트부분에서는 반.드.시. 물리적으로 할당한다(인스펙터 드래그)
    /// 
    /// </summary>
    protected virtual void Init()
    {
        if (objectCol == null || attackType == AttackType.Projectile || attackType == AttackType.Skill)
        {
            return;
        }
        ColliderTransEnable();
        //weaponCol.enabled = false; // 시작은 꺼두기
    }

    /// <summary>
    /// 시전자의 정보를 불러온다.
    /// 보통 해당 시전자의 공격력을 참조하거나, 대상이 사망한 이후엔 공격처리가 되지 않기 위함이기도 하다.
    /// </summary>
    /// <param name="character"></param>
    public void SetOwner(Character character)
    {
        owner = character;
    }

    /// <summary>
    /// 특정 조건(피격, 사망, 스킬 등등)에 의해 애니메이션 변경 시, 무기 콜라이더가 정상적으로 비활성화가 안된다.
    /// 이를 대응하기 위한 함수
    /// </summary>
    public void ResetColiderDisable()
    {
        if (objectCol != null)
            objectCol.enabled = false;
        Debug.Log(this.gameObject.name + "콜라이더 초기화됨");
    }

    /// <summary>
    /// 기본적으로 보든 캐릭터의 물리공격은 콜라이더로 관리한다.
    /// 현재 콜라이더 상태에 따라 활성화를 관리한다
    /// </summary>
    public void ColliderTransEnable()
    {
        if (objectCol == null)
        {
            Debug.Log("걍 없음");
            return;
        }

        if (objectCol.enabled == true)
        {
            objectCol.enabled = false;

        }
        else if (objectCol.enabled == false)
        {
            objectCol.enabled = true;

        }

    }

    virtual protected void OnTriggerEnter(Collider other)
    {
        //시전자가 미등록 시 미실행
        if (owner == null)
            return;

        if (other.GetComponent<Character>() != null && target == other.transform.tag)
        {
            Character hitTarget = other.GetComponent<Character>();

            //시전자가 안죽어야 공격처리가 된다.
            if (owner.GetIsDead() == true)
                return;



            if (hitTarget.GetIsParring() == true &&
                this.attackType == AttackType.Weapon)
            {
                //대상이 패링상태이면서, 현재 공격타입의 무기라면(물리공격), 패링 효과가 발생한다
                hitTarget.anim.SetTrigger("ParringAttack");
                hitTarget.SetIsParring(false);
                hitTarget.SetParringAction(true);
                return;
            }

            hitTarget.TakeDamage(damage, hitLevel);

            if (hitEffect.Length != 0)
            {
                PlayEffect(other);
            }

            if (attackType == AttackType.Projectile)
            {
                Destroy(this.gameObject);
            }
        }
    }

    void PlayEffect(Collider other)
    {
        Vector3 weaponColPoint = objectCol.ClosestPoint(other.bounds.center);
        Vector3 targetColPoint = other.ClosestPoint(weaponColPoint);
        Vector3 contactPoint = (weaponColPoint + targetColPoint) * 0.5f;

        Vector3 dir;
        float dist;

        //충돌 위치 결과
        Vector3 normal = (contactPoint - objectCol.bounds.center).normalized;


        if (Physics.ComputePenetration(
            objectCol, objectCol.transform.position, objectCol.transform.rotation,
            other, other.transform.position, other.transform.rotation,
            out dir, out dist))
        {
            normal = dir;  // 충돌 표면 방향
            // 접점 위치를 penetration 깊이만큼 보정
            contactPoint = contactPoint + normal * (dist * 0.5f);
        }

        Quaternion particleRotate = Quaternion.LookRotation(normal);

        int psDataIndex = Random.Range(0, hitEffect.Length);

        PoolManager.instance.Spawn(hitEffect[psDataIndex].name, contactPoint, particleRotate);
    }

    public float GetDamage() => damage;
    public void SetDamage(float value) => damage = value;
}
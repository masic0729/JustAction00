using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackerType
{
    Weapon = 0,
    Projectile,
    Skill
}

public class Attacker : MonoBehaviour, IAttacker
{
    [SerializeField] protected AttackerType attackType;
    [SerializeField] protected Collider objectCol;
    [SerializeField] protected ParticleSystem[] hitEffect;
    [SerializeField] protected int damage = 1;
    protected string target;
    protected int hitLevel = -1;
    protected string tagName;

    protected virtual void Start()
    {

    }

    /// <summary>
    /// WeaponCol같은 경우 스킬스크립트부분에서는 반.드.시. 물리적으로 할당한다(인스펙터 드래그)
    /// 
    /// </summary>
    protected virtual void Init()
    {
        
        if(objectCol == null)
        {
            objectCol = GetComponent<Collider>();
        }

        if (objectCol == null || attackType == AttackerType.Projectile || attackType == AttackerType.Skill)
        {
            return;
        }
        ColliderTransEnable();
        //weaponCol.enabled = false; // 시작은 꺼두기
    }

    /// <summary>
    /// 특정 조건(피격, 사망, 스킬 등등)에 의해 애니메이션 변경 시, 무기 콜라이더가 정상적으로 비활성화가 안된다.
    /// 이를 대응하기 위한 함수
    /// </summary>
    public void ResetColiderDisnable()
    {
        if (objectCol != null)
            objectCol.enabled = false;
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

        Debug.Log("Collider Transed");
    }

    virtual protected void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == target)
        {
            other.GetComponent<Character>().TakeDamage(damage, hitLevel);
            if(hitEffect.Length != 0)
            {
                PlayEffect(other);
            }
            if (attackType == AttackerType.Projectile)
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

        //파티클 생성 및 삭제 명령
        Quaternion particleRotate = Quaternion.LookRotation(normal);

        int psDataIndex = Random.Range(0, hitEffect.Length);
        //ParticleSystem ps = Instantiate(hitEffect[psDataIndex], contactPoint, particleRotate);
        //ps.Play();
        PoolManager.instance.Spawn(hitEffect[psDataIndex].name, contactPoint, particleRotate);
        //Destroy(ps, ps.main.duration);
    }

    public int GetDamage() => damage;
    public void SetDamage(int value) => damage = value;
}
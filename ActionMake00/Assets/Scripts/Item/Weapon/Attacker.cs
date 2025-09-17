using Drakkar.GameUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackerType
{
    Weapon = 0,
    Projectile,
    Wave
}

public class Attacker : MonoBehaviour, IAttacker
{
    [SerializeField] protected AttackerType attackType;
    [SerializeField] protected Collider weaponCol;
    [SerializeField] protected ParticleSystem hitEffect;
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
        
        if(weaponCol == null)
        {
            weaponCol = GetComponent<Collider>();
        }

        if (weaponCol == null || attackType == AttackerType.Projectile || attackType == AttackerType.Wave)
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
        if (weaponCol != null)
            weaponCol.enabled = false;
    }

    /// <summary>
    /// 기본적으로 보든 캐릭터의 물리공격은 콜라이더로 관리한다.
    /// 현재 콜라이더 상태에 따라 활성화를 관리한다
    /// </summary>
    public void ColliderTransEnable()
    {
        if (weaponCol == null)
        {
            Debug.Log("걍 없음");
            return;
        }

        if (weaponCol.enabled == true)
        {
            weaponCol.enabled = false;

        }
        else if (weaponCol.enabled == false)
        {
            weaponCol.enabled = true;

        }

        Debug.Log("Collider Transed");
    }

    virtual protected void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == target)
        {
            other.GetComponent<Character>().TakeDamage(damage, hitLevel);
            if(hitEffect != null)
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
        Vector3 weaponColPoint = weaponCol.ClosestPoint(other.bounds.center);
        Vector3 targetColPoint = other.ClosestPoint(weaponColPoint);
        Vector3 contactPoint = (weaponColPoint + targetColPoint) * 0.5f;

        Vector3 dir;
        float dist;

        //충돌 위치 결과
        Vector3 normal = (contactPoint - weaponCol.bounds.center).normalized;


        if (Physics.ComputePenetration(
            weaponCol, weaponCol.transform.position, weaponCol.transform.rotation,
            other, other.transform.position, other.transform.rotation,
            out dir, out dist))
        {
            normal = dir;  // 충돌 표면 방향
            // 접점 위치를 penetration 깊이만큼 보정
            contactPoint = contactPoint + normal * (dist * 0.5f);
        }

        //파티클 생성 및 삭제 명령
        Quaternion particleRotate = Quaternion.LookRotation(normal);
        ParticleSystem ps = Instantiate(hitEffect, contactPoint, particleRotate);
        ps.Play();
        Destroy(ps, ps.main.duration);
    }

    public int GetDamage() => damage;
    public void SetDamage(int value) => damage = value;
}
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

    protected int damage = 1;
    protected string target;
    protected int hitLevel = -1;
    protected string tagName;

    protected virtual void Start()
    {
        //Init();
    }

    protected virtual void Init()
    {
        weaponCol = GetComponent<Collider>();
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
            if (attackType == AttackerType.Projectile)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public int GetDamage() => damage;
    public void SetDamage(int value) => damage = value;
}
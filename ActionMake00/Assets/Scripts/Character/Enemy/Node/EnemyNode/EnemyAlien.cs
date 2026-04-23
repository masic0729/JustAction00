using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAlien : FollwingPlayerEnemyBT
{
    AttackColManager attackmanage;

    protected override void Start()
    {
        base.Start();
        Init();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Init()
    {
        base.Init();
        attackmanage = new AttackColManager();
        pEffectDic["CommonEnemyAttack"] = pEffect[0];
        playerFindDistance  = 5f;
        activityAllowValue  = 10f;
        attackReadyDistance = 1.2f;
    }

    /// <summary>
    /// 일반 몬스터의 일반 공격
    /// </summary>
    void Attack01()
    {
        Transform attackTrans = FindTransformAtChild("mixamorig1:LeftToeBase");
        //공격하는 위치가 없거나, 공격 중 상태가 아니면 사용되지 말 것
        if (attackTrans == null)
        {
            Debug.Log("데이터 없음");
            return;
        }

        ParticleSystem ps = Instantiate(pEffectDic["CommonEnemyAttack"], attackTrans.position, attackTrans.rotation);
        ps.Play();
        Destroy(ps.gameObject, ps.main.duration);

        Collider[] playerCol = attackmanage.CheckPlayerAttackAround(attackTrans, 2f, playerLayerMask);
        if (playerCol != null)
        {
            for (int i = 0; i < playerCol.Length; i++)
            {
                // 기존: damage 필드 직접 접근
                // 변경: GetResultDamage()로 버프와 장비 보정치가 반영된 최종 공격력을 사용한다
                // damage 필드는 AttributeSet 내부로 이동됐으므로 직접 접근 불가
                playerCol[i].GetComponent<Character>().TakeDamage(GetResultDamage(), this);
            }
        }

        attackAudio.Play();
    }

    public override void TakeDamage(float amount, Character attacker, int hitLevel = -1)
    {
        base.TakeDamage(amount, attacker, hitLevel);
        if (isSuperArmor == false || hitLevel != -1)
        {
            anim.SetTrigger("Hit");
        }
    }

    Collider CheckPlayerAttackAround(Transform trans)
    {
        Collider[] collider = Physics.OverlapSphere(trans.position, 2.0f, playerLayerMask);

        if (collider.Length <= 0)
        {
            return null;
        }
        return collider[0];
    }
}

using UnityEngine;

public class EnemyTest : FollwingPlayerEnemyBT
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
        playerFindDistance = 5f;
        activityAllowValue = 10f;
        attackReadyDistance = 2f;
    }

    

    /// <summary>
    /// 일반 몬스터의 일반 공격
    /// </summary>
    void Attack01()
    {
        Transform attackTrans = FindTransformAtChild("mixamorig1:LeftToeBase");
        if(attackTrans == null)
        {
            Debug.Log("데이터 없음");
            return;
        }
        ParticleSystem ps = Instantiate(pEffectDic["CommonEnemyAttack"], attackTrans.position, attackTrans.rotation);
        ps.Play();
        Destroy(ps.gameObject, ps.main.duration);
        //Collider playerCol = CheckPlayerAttackAround(attackTrans);
        Collider[] playerCol = attackmanage.CheckPlayerAttackAround(attackTrans, 2f, playerLayerMask);

        if (playerCol != null)
        {
            for (int i = 0; i < playerCol.Length; i++)
            {
                playerCol[i].GetComponent<Character>().TakeDamage(damage);

            }
        }
    }

    public override void TakeDamage(float amount, int hitLevel = -1)
    {
        base.TakeDamage(amount, hitLevel);
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


    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    
}

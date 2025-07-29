using UnityEngine;

public class EnemyTest : FollwingPlayerEnemyBT
{
    int playerLayerMask = 1 << 6;

    public ParticleSystem pEffect;
    //private int aiStateIndex = 0; // 0: 추적, 1: 대기

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// 일반 몬스터의 일반 공격
    /// </summary>
    void Attack01()
    {
        Transform attackTrans = FindTransformAtChild("B-toe.L");
        if(attackTrans == null)
        {
            Debug.Log("데이터 없음");
            return;
        }
        ParticleSystem ps = Instantiate(pEffect, attackTrans.position, attackTrans.rotation);
        ps.Play();
        Destroy(ps.gameObject, ps.main.duration);
        Collider playerCol = CheckPlayerAttackAround(attackTrans);
        if (playerCol != null)
        {
            playerCol.GetComponent<Character>().TakeDamage(damage);
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

    Transform FindTransformAtChild(string name)
    {
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.name == name) return t;
        }
        Debug.LogWarning("Child transform not found: " + name);
        return null;
    }
}

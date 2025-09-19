using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordSkill0 : Skill
{
    
    protected override void Start()
    {
        base.Start();
        Init();
    }
    private void OnEnable()
    {
        

    }

    protected override void Init()
    {
        base.Init();

        /*Collider enemyCol = CheckPlayerAttackAround(this.gameObject.transform);
        if (enemyCol != null)
        {
            enemyCol.GetComponent<Character>().TakeDamage(damage);
        }
        weaponCol.enabled = false;*/
    }

    

    /*Collider CheckPlayerAttackAround(Transform trans)
    {
        BoxCollider boxCol = weaponCol.GetComponent<BoxCollider>();
        Collider[] collider = Physics.OverlapBox(trans.position, boxCol.size, trans.rotation, enemyMask);

        if (collider.Length <= 0)
        {
            return null;
        }
        Debug.Log("나 된거 맞아");
        return collider[0];
    }*/

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackColManager
{


    public Collider[] CheckPlayerAttackAround(Transform trans, float attackDis, int targetLayer)
    {
        Collider[] collider = Physics.OverlapSphere(trans.position, attackDis, targetLayer);

        if (collider.Length <= 0)
        {
            return null;
        }
        
        return collider;
    }
}

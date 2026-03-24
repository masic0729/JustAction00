using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyReturnPositionNode : Node
{
    Vector3 targetPosition;

    public EnemyReturnPositionNode(Vector3 targetTransform, Transform transform) : base(transform)
    {
        targetPosition = targetTransform;
    }

    public override NodeState Evaluate()
    {
        if (Vector3.Distance(targetPosition, enemy.transform.position) > 0.3f && enemy.GetIsAttack() == false)
        {
            anim.SetBool("Move", true);


            enemy.MoveTarget(enemy.GetSpawnPosition());
            return state = NodeState.Running;
        }
        else
        {
            enemy.transform.position = targetPosition;
            anim.SetBool("Move", false);
            enemy.isDefault = true;
            enemy.isPlayerFound = false;
            
            return state = NodeState.Success;

        }
        
    }
}

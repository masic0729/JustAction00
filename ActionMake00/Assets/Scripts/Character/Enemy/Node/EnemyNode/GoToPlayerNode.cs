using UnityEngine;

public class GoToPlayerNode : Node
{
    Transform player;

    public GoToPlayerNode(Transform player, Transform transform) : base(transform)
    {
        this.player = player;

    }
    public override NodeState Evaluate()
    {
        //transform.LookAt(player);
        //enemy.MoveForward();
        if (Vector3.Distance(transform.position, player.transform.position) < enemy.GetAttackReadyDistance() || enemy.isDefault == false)
        {
            anim.SetBool("Move", false);
            enemy.MoveTarget(null);

            return state = NodeState.Success;
        }
        enemy.MoveTarget(player);

        anim.SetBool("Move", true);
        return state = NodeState.Running;
    }
}

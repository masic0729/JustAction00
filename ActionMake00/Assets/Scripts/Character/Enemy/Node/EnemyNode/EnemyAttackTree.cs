using UnityEngine;

public class EnemyAttackTree : Node
{
    protected Transform player;

    public EnemyAttackTree(Transform player, Transform transform) : base(transform)
    {
        this.player = player;
    }


    /// <summary>
    /// not used
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public override NodeState Evaluate()
    {
        throw new System.NotImplementedException();
    }
}

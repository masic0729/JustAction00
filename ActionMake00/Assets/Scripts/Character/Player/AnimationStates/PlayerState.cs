using UnityEngine;

public class PlayerState : CharacterStateMachine
{
    protected Player player;
    protected PlayerController playerCtrl;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);               
        if(player == null)
        {
            player = animator.gameObject.GetComponent<Player>();
            playerCtrl = animator.gameObject.GetComponent<PlayerController>();
        }

    }
}

using UnityEngine;

public class PlayerState : CharacterStateMachine
{
    protected PlayerController player;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);               
        if(player == null)
        {
            player = animator.gameObject.GetComponent<PlayerController>();
        }

    }
}

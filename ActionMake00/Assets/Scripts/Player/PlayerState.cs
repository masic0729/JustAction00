using UnityEngine;

public class PlayerState : StateMachineBehaviour
{
    protected PlayerController player;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //base.OnStateEnter(animator, stateInfo, layerIndex);               //void
        if(player == null)
        {
            player = animator.gameObject.GetComponent<PlayerController>();
        }

    }
}

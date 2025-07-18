using UnityEngine;

public class ResetAttackFlag : PlayerState
{
    /*override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isAttacking", false);
        animator.SetBool("isReAttack", false);  // 콤보도 같이 초기화
    }*/

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.SetBool("isReAttack", false);
        PlayerController instance = animator.GetComponent<PlayerController>();
        /*if(instance != null)
        {
        }*/
        player.DisableCombo();

    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}

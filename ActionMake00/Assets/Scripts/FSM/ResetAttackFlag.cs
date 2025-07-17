using UnityEngine;

public class ResetAttackFlag : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isAttacking", false);
        animator.SetBool("isReAttack", false);  // 콤보도 같이 초기화
    }
}

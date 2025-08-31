
using UnityEngine;

public class ResetEndAttackFlag : PlayerState
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //animator.SetBool("isAttacking", false);
        animator.SetBool("isReAttack", false);                                                              // 콤보도 같이 초기화
        //player.isEscapeAttackAnim = false;
        playerCtrl.isEscapeAttackAnim = false;
        playerCtrl.DisableCombo();
        //playerCtrl.DisableCombo();


    }
}

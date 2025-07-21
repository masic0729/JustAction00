
using UnityEngine;

public class ResetEndAttackFlag : PlayerState
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        Debug.Log("실행");
        //animator.SetBool("isAttacking", false);
        animator.SetBool("isReAttack", false);                                                              // 콤보도 같이 초기화
        PlayerController instance = animator.gameObject.GetComponent<PlayerController>();
        player.isEscapeAttackAnim = false;
        player.DisableCombo();


    }
}

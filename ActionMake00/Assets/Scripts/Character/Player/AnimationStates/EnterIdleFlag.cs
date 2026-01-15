using UnityEngine;

public class EnterIdleFlag : PlayerState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        playerCtrl.TransIdleState();
        playerCtrl.SetComboAttackIndex(0);
        playerCtrl.SetCanAnyInput(true);

        player.WeaponColDisable();
        player.SetIsParring(false);
        player.SetParringAction(false);
    }
}

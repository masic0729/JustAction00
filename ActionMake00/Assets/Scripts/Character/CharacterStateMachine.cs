using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateMachine : StateMachineBehaviour
{
    protected Character character;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        character = animator.gameObject.GetComponent<Character>();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KBS
{
    public class CombatStateMachineBehaviour : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            CharacterBase actor = animator.GetComponent<CharacterBase>();
            actor.CombatComplete();
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KBS
{
    public class RollingStateMachineBehaviour : StateMachineBehaviour
    {
        private CharacterBase linkedCharacter;
        public void Initialize(CharacterBase character)
        {
            linkedCharacter = character;

        }
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            linkedCharacter.RollingComplete();
        }
    }
}

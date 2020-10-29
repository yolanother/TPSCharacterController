using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations;

namespace DoubTech.TPSCharacterController.Animation.State
{
    public class AnimationStartStop : StateMachineBehaviour
    {
        [SerializeField] private string tag;
        
        private AnimationEventReceiver receiver;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex,
            AnimatorControllerPlayable controller)
        {   
            base.OnStateEnter(animator, stateInfo, layerIndex, controller);

            receiver = animator.gameObject.GetComponent<AnimationEventReceiver>();
            if(receiver) receiver.StateEnterEvent(tag);
        }

        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            base.OnStateMachineExit(animator, stateMachinePathHash);
            
            if(receiver) receiver.StateExitEvent(tag);
        }
    }
}

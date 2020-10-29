using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations;

namespace DoubTech.TPSCharacterController.Animation.State
{
    public class AnimationStartStop : StateMachineBehaviour
    {
        [SerializeField] private string tag;
        [SerializeField] private bool useStateExit = false;
        
        private AnimationEventReceiver receiver;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex,
            AnimatorControllerPlayable controller)
        {   
            base.OnStateEnter(animator, stateInfo, layerIndex, controller);

            Debug.Log("State machine enter: " + tag);
            receiver = animator.gameObject.GetComponent<AnimationEventReceiver>();
            if(receiver) receiver.StateEnterEvent(tag);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            Debug.Log("State machine exit: " + tag);
            if (stateInfo.length < 0.01f || useStateExit)
            {
                if(receiver) receiver.StateExitEvent(tag);
            }
        }

        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            base.OnStateMachineExit(animator, stateMachinePathHash);
            Debug.Log("State machine exit: " + tag);
            
            if(receiver) receiver.StateExitEvent(tag);
        }
    }
}

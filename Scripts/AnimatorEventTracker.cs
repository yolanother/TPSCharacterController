using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DoubTech.TPSCharacterController
{
    public class AnimatorEventTracker : MonoBehaviour
    {
        public delegate void AnimatorMoveEvent();
        public delegate void AnimatorIKEvent(int layerIndex);

        public AnimatorMoveEvent OnAnimatorMoveEvent;
        public AnimatorIKEvent OnAnimatorIKEvent;

        private void OnAnimatorMove() {
            OnAnimatorMoveEvent?.Invoke();
        }

        private void OnAnimatorIK(int layerIndex) {
            OnAnimatorIKEvent?.Invoke(layerIndex);
        }
    }
}

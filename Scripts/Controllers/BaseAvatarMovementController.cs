using System;
using DoubTech.TPSCharacterController.Animation.Control;
using UnityEngine;
using UnityEngine.Events;

namespace DoubTech.TPSCharacterController
{
    public abstract class BaseAvatarMovementController : MonoBehaviour
    {
        private bool isReady;
        private AvatarAnimationController animController;
        private AnimatorEventTracker animatorEventTracker;
        private OnCharacterReadyEvent onCharacterReadyEvent = new OnCharacterReadyEvent();

        public AvatarAnimationController AvatarController => animController;

        protected virtual void Awake()
        {
            animController = GetComponent<AvatarAnimationController>();
        }

        public void CharacterReady()
        {
            if (isReady || !animController.IsReady) return;
            
            isReady = true;
            if (!animController.Animator.TryGetComponent(out animatorEventTracker)) {
                animatorEventTracker = animController.Animator.gameObject.AddComponent<AnimatorEventTracker>();
            }
            
            if(animatorEventTracker) animatorEventTracker.OnAnimatorMoveEvent += OnAnimatorMove;
            OnCharacterReady();
            onCharacterReadyEvent.Invoke();
        }

        public void AddCharacterReadyEvent(UnityAction listener)
        {
            onCharacterReadyEvent.AddListener(listener);
            if (isReady) listener();
        }

        public void RemoveCharacterReadyEvent(UnityAction listener)
        {
            onCharacterReadyEvent.RemoveListener(listener);
        }

        protected virtual void OnEnable() {
            CharacterReady();
        }

        protected abstract void OnCharacterReady();

        protected virtual void OnDisable()
        {
            isReady = false;
            if(animatorEventTracker) animatorEventTracker.OnAnimatorMoveEvent -= OnAnimatorMove;
        }

        protected virtual void OnAnimatorMove() { }

        protected void Update()
        {
            if (!animController.IsReady) return;
            if(!isReady) CharacterReady();

            if (isReady) OnUpdate();
        }

        protected void FixedUpdate()
        {
            if (!isReady) return;
            
            OnFixedUpdate();
        }

        protected void LateUpdate()
        {
            if (!isReady) return;

            OnLateUpdate();
        }

        protected abstract void OnLateUpdate();

        protected abstract void OnFixedUpdate();

        protected abstract void OnUpdate();
    }
    
    [SerializeField]
    public class OnCharacterReadyEvent : UnityEvent {}
}
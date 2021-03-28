using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Animation.Control;
using UnityEngine.Events;

namespace DoubTech.TPSCharacterController.Damage
{
    public class DamageReceiver : CoordinatorReferenceMonoBehaviour
    {
        [SerializeField] private float receiverDamageMultiplier = 1;

        [SerializeField] private bool attachToBone = false;
        [SerializeField] private HumanBodyBones bone;
        
        [SerializeField] private OnDamageReceived onDamageReceived = new OnDamageReceived();

        public OnDamageReceived OnDamageReceived => onDamageReceived;

        private void OnEnable()
        {
            if (attachToBone)
            {
                Coordinator.AvatarAnimator.ExecuteWhenReady(() =>
                {
                    var target = Coordinator.AvatarAnimator.Animator.GetBoneTransform(bone);
                    transform.parent = target;
                });
            }
        }

        public void Damage(TPSCharacterCoordinator owner, Vector3 position, float damageMultiplier)
        {
            var finalDamage = damageMultiplier * receiverDamageMultiplier;
            
            if (Coordinator.AvatarAnimator)
            {
                Coordinator.AvatarAnimator.Hit(position);
            }

            
            owner.OnDamaged(Coordinator, finalDamage);

            var health = Coordinator.Health;
            if (Coordinator.Health)
            {
                Coordinator.Health.OnDamaged(finalDamage);;
                if (!Coordinator.Health.IsAlive)
                {
                    owner.OnKilled(Coordinator);
                }
            }
            
            onDamageReceived.Invoke(finalDamage);
        }
    }
    
    [Serializable]
    public class OnDamageReceived : UnityEvent<float> {}
}

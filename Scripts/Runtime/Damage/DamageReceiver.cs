using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DoubTech.TPSCharacterController.Animation.Control;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

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
            if (attachToBone && Coordinator && Coordinator.AvatarAnimator)
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
            Debug.Log("Damaged by " + owner.name);
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
            
            onDamageReceived.Invoke(owner, position, finalDamage);
        }
    }
    
    [Serializable]
    public class OnDamageReceived : UnityEvent<TPSCharacterCoordinator, Vector3, float> {}
}

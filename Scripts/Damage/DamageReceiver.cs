using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Animation.Control;
using UnityEngine.Events;

namespace DoubTech.TPSCharacterController.Damage
{
    public class DamageReceiver : MonoBehaviour
    {
        [SerializeField] private float receiverDamageMultiplier = 1;

        [SerializeField] private bool attachToBone = false;
        [SerializeField] private HumanBodyBones bone;
        
        [SerializeField] private OnDamageReceived onDamageReceived = new OnDamageReceived();
        
        private AvatarAnimationController anim;

        public OnDamageReceived OnDamageReceived => onDamageReceived;

        private void OnEnable()
        {
            if (!anim)
            {
                anim = GetComponentInParent<AvatarAnimationController>();

                if (attachToBone)
                {
                    anim.ExecuteWhenReady(() =>
                    {
                        var target = anim.Animator.GetBoneTransform(bone);
                        transform.parent = target;
                    });
                }
            }
        }

        public void Damage(Vector3 position, float damageMultiplier)
        {
            onDamageReceived.Invoke(damageMultiplier * receiverDamageMultiplier);
            if (anim)
            {
                anim.Hit(position);
            }
        }
    }
    
    [Serializable]
    public class OnDamageReceived : UnityEvent<float> {}
}

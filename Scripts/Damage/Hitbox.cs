using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Animation.Control;
using DoubTech.TPSCharacterController.Demo;
using DoubTech.TPSCharacterController.Inventory.Weapons;

namespace DoubTech.TPSCharacterController.Damage
{
    public class Hitbox : MonoBehaviour
    {
        [SerializeField] private float damageMultiplier = 1;
        
        [SerializeField] private bool useSpeedMultiplier = false;
        [SerializeField] private float minSpeedForMaxDamage = 1;

        [SerializeField] private bool disableAfterImpact = true;
        
        [SerializeField] private Collider[] colliders;
        
        private AvatarAnimationController anim;
        private float speed;
        private Vector3 lastPosition;
        private TPSCharacterCoordinator owner;

        public Weapon Weapon { get; set; }

        private void Awake()
        {
            if (null == colliders || colliders.Length == 0)
            {
                colliders = GetComponents<Collider>();
            }

            if (colliders.Length == 0)
            {
                Debug.LogError("No colliders assigned to hit box " + name);
            }

            foreach (var colllider in colliders)
            {
                colllider.enabled = false;
            }
        }

        public void EnableHitbox(TPSCharacterCoordinator owner)
        {
            this.owner = owner;
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = true;
            }

            lastPosition = transform.position;
        }

        public void DisableHitbox()
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var damageReduction = 1.0f;
            var blocker = other.gameObject.GetComponent<Blockbox>();
            if (blocker)
            {
                damageReduction = blocker.DamageReductionMultiplier;
            
                if(disableAfterImpact) DisableHitbox();
            }
            
            var receiver = other.gameObject.GetComponent<DamageReceiver>();
            if (receiver)
            {
                var speedMult = 1f;
                if (useSpeedMultiplier)
                {
                    speedMult = Mathf.Min(minSpeedForMaxDamage, speed) / minSpeedForMaxDamage;
                }
                receiver.Damage(owner, transform.position, damageMultiplier * damageReduction * Weapon.Stats.damage * speedMult);
            
                if(disableAfterImpact) DisableHitbox();
            }
        }

        private void Update()
        {
            if (useSpeedMultiplier)
            {
                speed = Vector3.Distance(transform.position, lastPosition) / Time.deltaTime;
                lastPosition = transform.position;
            }
        }
    }
}

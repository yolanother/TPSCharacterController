using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Animation.Control;
using DoubTech.TPSCharacterController.Damage;
using DoubTech.TPSCharacterController.Inventory.Items;
using DoubTech.TPSCharacterController.Inventory.Slots;

namespace DoubTech.TPSCharacterController.Inventory.Weapons
{
    public class Weapon : MonoBehaviour, SlotEquippedListener
    {
        [SerializeField] private WeaponStats weaponStats;
        [SerializeField] private Hitbox[] hitboxes;
        [SerializeField] private Blockbox[] blockboxes;
        
        public WeaponStatsData Stats => weaponStats.Stats;

        private void Awake()
        {
            foreach (var hitbox in hitboxes)
            {
                hitbox.Weapon = this;
            }
        }

        public void OnItemEquipped(AvatarAnimationController avatar, Slot slot, Item item)
        {
            if (avatar)
            {
                avatar.OnAttackStarted.AddListener(EnableHitboxes);
                avatar.OnAttackStopped.AddListener(DisableHitboxes);
            }
            else
            {
                Debug.Log(name + " does not have an animated parent. Hitboxes are enabled by default.");
                EnableHitboxes();
            }
        }

        public void OnItemUnequipped(AvatarAnimationController avatar, Slot slot, Item item)
        {
            if (avatar)
            {
                avatar.OnAttackStarted.RemoveListener(EnableHitboxes);
                avatar.OnAttackStopped.RemoveListener(DisableHitboxes);
            }
            DisableHitboxes();
        }

        private void EnableHitboxes()
        {
            for (int i = 0; i < hitboxes.Length; i++)
            {
                hitboxes[i].EnableHitbox();
            }
        }

        private void DisableHitboxes()
        {
            for (int i = 0; i < hitboxes.Length; i++)
            {
                hitboxes[i].DisableHitbox();
            }
        }
    }
}

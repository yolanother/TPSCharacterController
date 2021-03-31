using UnityEngine;
using DoubTech.TPSCharacterController.Damage;
using DoubTech.TPSCharacterController.Inventory.Items;
using DoubTech.TPSCharacterController.Inventory.Slots;
using Sirenix.OdinInspector;

namespace DoubTech.TPSCharacterController.Inventory.Weapons
{
    public class Weapon : CoordinatorReferenceMonoBehaviour, SlotEquippedListener
    {
        [BoxGroup("Stats")]
        [SerializeField] private WeaponStats weaponStats;
        
        [BoxGroup("Collision Boxes")]
        [SerializeField] private Hitbox[] hitboxes;
        [BoxGroup("Collision Boxes")]
        [SerializeField] private Blockbox[] blockboxes;
        
        public WeaponStatsData Stats => weaponStats.Stats;

        private void Awake()
        {
            foreach (var hitbox in hitboxes)
            {
                hitbox.Weapon = this;
            }
        }

        public void OnItemEquipped(TPSCharacterCoordinator owner, Slot slot, Item item)
        {
            if (owner && owner.AvatarAnimator)
            {
                owner.AvatarAnimator.OnAttackStarted.AddListener(EnableHitboxes);
                owner.AvatarAnimator.OnAttackStopped.AddListener(DisableHitboxes);
            }
            else
            {
                Debug.Log(name + " does not have an animated parent. Hitboxes are enabled by default.");
                EnableHitboxes();
            }
        }

        public void OnItemUnequipped(TPSCharacterCoordinator owner, Slot slot, Item item)
        {
            if (owner && owner.AvatarAnimator)
            {
                owner.AvatarAnimator.OnAttackStarted.RemoveListener(EnableHitboxes);
                owner.AvatarAnimator.OnAttackStopped.RemoveListener(DisableHitboxes);
            }
            DisableHitboxes();
        }

        private void EnableHitboxes()
        {
            for (int i = 0; i < hitboxes.Length; i++)
            {
                hitboxes[i].EnableHitbox(Coordinator);
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

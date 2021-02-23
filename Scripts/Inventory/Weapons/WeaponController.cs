using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DoubTech.TPSCharacterController.Animation.Control;
using DoubTech.TPSCharacterController.Inventory.Slots;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace DoubTech.TPSCharacterController.Inventory.Weapons
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private Slot[] unequippedWeaponSlots;
        [SerializeField] private Slot[] equippedWeaponSlots;
        
        private Dictionary<Slot, Slot> equippedSlots = new Dictionary<Slot, Slot>();

        public Weapon[] EquippedWeapons
        {
            get
            {
                List<Weapon> weapons = new List<Weapon>();
                foreach (var slot in equippedWeaponSlots)
                {
                    Weapon weapon = slot.Item?.GetComponent<Weapon>();
                    if (weapon)
                    {
                        weapons.Add(weapon);
                    }
                }

                return weapons.ToArray();
            }
        }
        
        private void Awake()
        {
            bool equipped = false;
            foreach (var slot in equippedWeaponSlots)
            {
                if (slot.Item)
                {
                    Equip(slot, slot);
                    equipped = true;
                }
            }

            if (equipped)
            {
                var avatar = GetComponent<AvatarAnimationController>();
                if (avatar)
                {
                    avatar.ExecuteWhenReady(() => avatar.Equip());
                }
            }
        }

        public void Equip(Slot unequippedSlot, Slot equippedSlot)
        {
            Unequip(equippedSlot);
            var item = unequippedSlot.Item;
            equippedSlot.Item = item;
            equippedSlots[equippedSlot] = unequippedSlot;
        }

        public void Unequip(Slot equipSlot)
        {
            if (equippedSlots.ContainsKey(equipSlot))
            {
                var unequippedSlot = equippedSlots[equipSlot];
                var item = equipSlot.Item;
                equipSlot.Item = null;
                unequippedSlot.Item = item;
                equippedSlots.Remove(equipSlot);
            }
        }

#if ODIN_INSPECTOR
        [Button]
#endif
        public void EquipNext()
        {
            Slot firstSlot = null;
            foreach (var slot in equippedWeaponSlots)
            {
                if (slot.Item)
                {
                    firstSlot = slot;
                    break;
                }
            }

            int index = 0;
            if (firstSlot)
            {
                var ues = equippedSlots[firstSlot];
                index = Array.IndexOf(unequippedWeaponSlots, ues) + 1;
                if (index >= unequippedWeaponSlots.Length) index = -1;
            }

            if (index == -1)
            {
                Unequip();
            }
            else
            {
                Equip(0, true);
            }
        }

#if ODIN_INSPECTOR
        [Button]
#endif
        public void Equip(int slotIndex, bool replace)
        {
            var slot = unequippedWeaponSlots[slotIndex];

            for (int i = 0; i < equippedWeaponSlots.Length; i++)
            {
                var targetSlot = equippedWeaponSlots[i];
                if(targetSlot.CanHoldItem(slot.Item) && (!targetSlot.Item || replace)) 
                {
                    Equip(slot, targetSlot);
                    break;
                }
            }
        }
        
#if ODIN_INSPECTOR
        [Button]
#endif
        public void Unequip()
        {
            foreach (var slot in equippedSlots.Keys)
            {
                Unequip(slot);
            }
        }

#if ODIN_INSPECTOR
        [Button]
#endif
        public void Throw()
        {
            if (equippedSlots.Count > 0)
            {
                var slot = equippedSlots.First();
                var item = slot.Key.Item;
                item.PickupDelay = Time.realtimeSinceStartup + .5f;
                var t = item.transform;
                var pos = t.position;
                var rot = t.rotation;
                Unequip(slot.Key);
                slot.Value.Item = null;
                t.position = pos;
                t.rotation = rot;

                var rb = item.GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.AddForce(item.transform.position + transform.forward * 10);
                }
            }
        }
    }
}

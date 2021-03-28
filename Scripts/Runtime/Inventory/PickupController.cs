using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Inventory.Items;
using DoubTech.TPSCharacterController.Inventory.Slots;
using UnityEngine.Events;

namespace DoubTech.TPSCharacterController.Inventory
{
    [RequireComponent(typeof(Collider))]
    public class PickupController : MonoBehaviour
    {
        [SerializeField] private bool autopickup;
        [SerializeField] private Slot[] managedSlots;
        
        [SerializeField] OnEnteredPickup OnEnteredPickup = new OnEnteredPickup();

        private void OnTriggerEnter(Collider other)
        {
            var item = other.GetComponent<Item>();
            if (item && item.PickupDelay < Time.realtimeSinceStartup)
            {
                if (autopickup) Pickup(item);
                else OnEnteredPickup.Invoke(item);
            }
        }

        private void Pickup(Item item)
        {
            for (int i = 0; i < managedSlots.Length; i++)
            {
                var slot = managedSlots[i];
                if (slot.IsSlotAvailable(item))
                {
                    slot.Item = item;
                    break;
                }
            }
        }
    }
    
    [Serializable] public class OnEnteredPickup : UnityEvent<Item> { }
}

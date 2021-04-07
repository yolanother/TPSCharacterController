using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Animation.Control;
using DoubTech.TPSCharacterController.Inventory.Items;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.Utilities;
#endif
using UnityEngine.Events;

namespace DoubTech.TPSCharacterController.Inventory.Slots
{
    public class Slot : CoordinatorReferenceMonoBehaviour
    {
        [Header("Item")]
        [SerializeField] private Item item;

        [Header("Slot Configuration")]
        [SerializeField] private bool isSlotVisible;
        [SerializeField] private bool sendsEquippedEvent = false;

        [SerializeField] public SlotConfigurationPreset configurationPreset;
        [SerializeField] private SlotConfiguration configuration;

        private AvatarAnimationController avatar;
        
        [Header("Events")]
        public OnItemAddedToSlot onItemAddedToSlot = new OnItemAddedToSlot();
        public OnItemAddedToSlot OnItemAdded => onItemAddedToSlot;
        
        public OnItemRemovedFromSlot onItemRemovedFromSlot = new OnItemRemovedFromSlot();
        public OnItemRemovedFromSlot OnItemRemoved => onItemRemovedFromSlot;

        private class InitialConfig
        {
            public bool wasKinematic;
            public List<Collider> enabledColliders = new List<Collider>();
            public Rigidbody rigidbody;
            public bool modelWasVisible;
        }
        private InitialConfig initialItemConfig;

        public Item Item
        {
            get => item;
            set
            {
                if (item && item != value) {
                    ItemRemovedFromSlot(item);
                }

                if (value != item) {
                    ItemAddedToSlot(value);
                }
            }
        }

        public SlotConfiguration Config => configurationPreset ? configurationPreset.configuration : configuration;

        private void Awake()
        {
            if (configurationPreset)
            {
                configuration = configurationPreset.configuration;
            }
            
            avatar = GetComponentInParent<AvatarAnimationController>();

            if (null == avatar)
            {
                enabled = false;
                throw new Exception("Slot disabled. No associated AvatarAnimationController could be found. Slots must be placed within the hierarchy under the AvatarAnimationController.");
            }

            if (item)
            {
                // If slot's item is preassigned with a prefab go ahead and instantiate it and add it to the slot.
                var i = Instantiate(item);
                
                if (!avatar.IsReady)
                {
                    avatar.OnAvatarReady.AddListener(() =>
                    {
                        item = null;
                        Item = i;
                    });
                }
                else
                {
                    item = null;
                    Item = i;
                }
            }
        }

        private void ItemAddedToSlot(Item pickup)
        {
            item = pickup;
            onItemAddedToSlot.Invoke(pickup);
            OnItemAddedToSlot(pickup);

            item.transform.parent = transform;

            var rigidbody = item.GetComponent<Rigidbody>();            
            initialItemConfig = new InitialConfig()
            {
                rigidbody = rigidbody,
                wasKinematic = rigidbody.isKinematic,
                modelWasVisible = item.Model && item.Model.activeInHierarchy
            };

            if (item.Model)
            {
                if (isSlotVisible)
                {
                    var pos = configuration.GetPosition(pickup);
                    if (null == pos)
                    {
                        Debug.LogError("No position target could be found for " + pickup.Type + " " + pickup.name);
                    }
                    else
                    {
                        item.Model.transform.parent = avatar.Animator.GetBoneTransform(pos.bone);
                        item.Model.transform.localPosition = pos.offset;
                        item.Model.transform.localEulerAngles = pos.rotation;
                        item.Model.SetActive(true);
                    }
                }
                else
                {
                    item.Model.SetActive(false);
                }
            }


            rigidbody.isKinematic = true;
            foreach (Collider c in item.GetComponents<Collider>())
            {
                if (c.enabled)
                {
                    initialItemConfig.enabledColliders.Add(c);
                    c.enabled = false;
                }
            }

            if (sendsEquippedEvent)
            {
                var equippedLiseners = item.GetComponentsInChildren<SlotEquippedListener>();
                foreach (var listener in equippedLiseners)
                {
                    listener.OnItemEquipped(Coordinator, this, item);
                }
            }
        }

        protected virtual void OnItemAddedToSlot(Item pickup) {

        }

        protected virtual void ItemRemovedFromSlot(Item pickup)
        {
            item.transform.parent = null;
            
            if (item.Model)
            {
                if (isSlotVisible)
                {
                    item.Model.transform.parent = item.transform;
                    item.Model.transform.localPosition = Vector3.zero;
                    item.Model.transform.localEulerAngles = Vector3.zero;
                }
                
                item.Model.SetActive(initialItemConfig.modelWasVisible);
            }

            initialItemConfig.rigidbody.isKinematic = initialItemConfig.wasKinematic;
            foreach (var c in initialItemConfig.enabledColliders)
            {
                c.enabled = true;
            }
            
            onItemRemovedFromSlot.Invoke(pickup);
            OnItemRemovedFromSlot(pickup);

            if (sendsEquippedEvent)
            {
                var equippedLiseners = item.GetComponentsInChildren<SlotEquippedListener>();
                foreach (var listener in equippedLiseners)
                {
                    listener.OnItemUnequipped(Coordinator, this, item);
                }
            }
            
            item = null;
        }

        protected virtual void OnItemRemovedFromSlot(Item pickup) {

        }

        public virtual bool CanHoldItem(Item item)
        {
            return configuration.SupportedItemTypes.Count == 0 || configuration.SupportedItemTypes.Contains(item.Type);
        }

        public virtual bool IsSlotAvailable(Item item)
        {
            return !Item && CanHoldItem(item);
        }
        
        public override string ToString()
        {
            return name;
        }

        #if ODIN_INSPECTOR
        [Button]
        #endif
        public void Drop()
        {
            if (Item)
            {
                var item = Item;
                item.PickupDelay = Time.realtimeSinceStartup + 1;
                Item = null;
                item.transform.position = item.transform.position + transform.up * 2;
            }
        }
        
        #if ODIN_INSPECTOR
        // TODO: This needs a custom inspector, hacking with Odin for now.
        [Button]
        #endif
        public void SavePosition()
        {
            var position = item.Model.transform.localPosition;
            var rot = item.Model.transform.localEulerAngles;
            var pos = configuration.GetPosition(item);
            pos.offset = position;
            pos.rotation = rot;
        }
    }

    [Serializable] public class OnItemAddedToSlot : UnityEvent<Item> { }
    [Serializable] public class OnItemRemovedFromSlot : UnityEvent<Item> { }
}

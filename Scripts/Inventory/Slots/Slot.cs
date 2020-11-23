using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Animation.Control;
using DoubTech.TPSCharacterController.Inventory.Items;
using Sirenix.Utilities;
using UnityEngine.Events;

namespace DoubTech.TPSCharacterController.Inventory.Slots
{
    public class Slot : MonoBehaviour
    {
        [Header("Item")]
        [SerializeField] private Item item;

        [Header("Slot Configuration")]
        [SerializeField] private bool isSlotVisible;

        [SerializeField] private SlotConfigurationPreset configurationPreset;
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
                    ItemRemovedFromSlot(value);
                }

                if (value) {
                    ItemAddedToSlot(value);
                }
            }
        }

        private void Awake()
        {
            if (configurationPreset)
            {
                configuration = configurationPreset.configuration;
            }
            
            var parent = transform;
            while (parent && !avatar)
            {
                avatar = parent.GetComponent<AvatarAnimationController>();
                parent = parent.parent;
            }

            if (null == avatar)
            {
                enabled = false;
                throw new Exception("Slot disabled. No associated AvatarAnimationController could be found. Slots must be placed within the hierarchy under the AvatarAnimationController.");
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
                    item.Model.SetActive(true);
                    var pos = configuration.GetPosition(pickup);
                    item.Model.transform.parent = avatar.Animator.GetBoneTransform(pos.bone);
                    item.Model.transform.localPosition = pos.offset;
                    item.Model.transform.localEulerAngles = pos.rotation;
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
            item = null;
        }

        protected virtual void OnItemRemovedFromSlot(Item pickup) {

        }

        public virtual bool CanHoldItem(Item item)
        {
            return !Item && (configuration.SupportedItemTypes.Count == 0 || configuration.SupportedItemTypes.Contains(item.Type));
        }
        
        public override string ToString()
        {
            return name;
        }
    }

    [Serializable] public class OnItemAddedToSlot : UnityEvent<Item> { }
    [Serializable] public class OnItemRemovedFromSlot : UnityEvent<Item> { }
}

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DoubTech.TPSCharacterController.Inventory.Items;
using Sirenix.Utilities;

namespace DoubTech.TPSCharacterController.Inventory.Slots
{
    [CreateAssetMenu(menuName = "TPS Character Controller/Slots/Slot Configuration")]
    public class SlotConfigurationPreset : ScriptableObject {
        public SlotConfiguration configuration;
    }
    
    [Serializable]
    public class SlotConfiguration
    {
        [Tooltip("The types of items that this slot can hold. If left empty all item types can be placed here.")]
        [Header("Slot Restrictions")]
        [SerializeField] public List<ItemType> supportedItemTypes = new List<ItemType>();

        [Header("Slot Positioning")]
        [SerializeField] public HumanBodyBones defaultBone;
        [SerializeField] public List<TypedSlotPosition> positions = new List<TypedSlotPosition>();
        [SerializeField] public List<ModelSlotPosition> modelSlotPositions = new List<ModelSlotPosition>();

        private Dictionary<ItemType, TypedSlotPosition> itemTypeToPositions;
        public Dictionary<ItemType, TypedSlotPosition> ItemTypePositions
        {
            get
            {
                if (null == itemTypeToPositions)
                {
                    itemTypeToPositions = new Dictionary<ItemType, TypedSlotPosition>();
                    foreach (var position in positions)
                    {
                        if (position.Type)
                        {
                            itemTypeToPositions[position.Type] = position;
                        }
                    }
                }

                return itemTypeToPositions;
            }
        }

        public List<ModelSlotPosition> ModelSlotPositions => modelSlotPositions;
        private Dictionary<string, ModelSlotPosition> modelSlotPositionsSet;
        public Dictionary<string, ModelSlotPosition> ModelSlotPositionsSet
        {
            get
            {
                if (null == modelSlotPositionsSet)
                {
                    modelSlotPositionsSet = new Dictionary<string, ModelSlotPosition>();
                    foreach (var position in modelSlotPositions)
                    {
                        modelSlotPositionsSet[position.Key] = position;
                    }
                }

                return modelSlotPositionsSet;
            }
        }

        public ModelSlotPosition GetModelSlotPosition(Item item)
        {
            return ModelSlotPositionsSet.TryGetValue(ModelSlotPosition.CreateKey(item), out var result) ? result : null;
        }

        public TypedSlotPosition GetTypedSlotPosition(Item item)
        {
            return ItemTypePositions.TryGetValue(item.Type, out var result) ? result : null;
        }

        private Dictionary<string, List<ModelSlotPosition>> configuredModels;
        public Dictionary<string, List<ModelSlotPosition>> ConfiguredModels
        {
            get
            {
                if (null == configuredModels)
                {
                    configuredModels = new Dictionary<string, List<ModelSlotPosition>>();
                    foreach (var position in modelSlotPositions)
                    {
                        List<ModelSlotPosition> models;
                        if (!configuredModels.TryGetValue(position.modelName, out models))
                        {
                            models = new List<ModelSlotPosition>();
                            configuredModels[position.modelName] = models;
                        }
                        models.Add(position);
                    }
                }
                return configuredModels;
            }
        }
        
        private HashSet<ItemType> typeIndex;
        public HashSet<ItemType> SupportedItemTypes
        {
            get
            {
                if (null == typeIndex)
                {
                    typeIndex = new HashSet<ItemType>();
                    typeIndex.AddRange(supportedItemTypes);
                }

                return typeIndex;
            }
        }

        public ItemSlotPosition GetPosition(Item item)
        {
            return GetPosition(item.Type, item.Model ? item.ModelName : "");
        }

        public void AddPosition(TypedSlotPosition position)
        {
            if (!ItemTypePositions.ContainsKey(position.Type))
            {
                positions.Add(position);
                OnValidate();
            }
        }

        public ItemSlotPosition GetPosition(ItemType itemType, string modelName = "")
        {
            var key = ModelSlotPosition.CreateKey(itemType, modelName);
            if (ModelSlotPositionsSet.ContainsKey(key))
            {
                return ModelSlotPositionsSet[key].position;
            }
            else if (ItemTypePositions.ContainsKey(itemType))
            {
                return ItemTypePositions[itemType].position;
            }

            return new ItemSlotPosition()
            {
                type = itemType,
                bone = defaultBone
            };
        }

        private void OnValidate()
        {
            modelSlotPositionsSet = null;
            itemTypeToPositions = null;
            configuredModels = null;
            typeIndex = null;
        }

        public void AddItemType(ItemType type)
        {
            if (!SupportedItemTypes.Contains(type))
            {
                supportedItemTypes.Add(type);
                typeIndex.Add(type);
                configuredModels = null;
            }
        }

        public void RemoveItemType(ItemType type)
        {
            supportedItemTypes.Remove(type);
            for (int i = positions.Count - 1; i >= 0; i--)
            {
                if(positions[i].Type == type) {}
                positions.RemoveAt(i);
            }
            
            for (int i = modelSlotPositions.Count - 1; i >= 0; i--)
            {
                if(modelSlotPositions[i].Type == type) {}
                positions.RemoveAt(i);
            }
            
            OnValidate();
        }

        public void AddPosition(ModelSlotPosition position)
        {
            if (!ModelSlotPositionsSet.ContainsKey(position.Key))
            {
                modelSlotPositions.Add(position);
                OnValidate();
            }
        }
    }
}

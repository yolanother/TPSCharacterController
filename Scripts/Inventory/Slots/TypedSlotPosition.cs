using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Inventory.Items;
using Object = UnityEngine.Object;

namespace DoubTech.TPSCharacterController.Inventory.Slots
{
    [Serializable]
    public class TypedSlotPosition
    {
        [SerializeField] public ItemSlotPosition position;

        public ItemType Type => position.type;
    }

    [Serializable]
    public class ModelSlotPosition
    {
        [SerializeField] public ItemSlotPosition position;
        [SerializeField] public string modelName;

        public string Key => CreateKey(position.type, modelName);
        public ItemType Type => position.type;

        public static string CreateKey(ItemType itemType, string modelName)
        {
            return itemType.ID + "::" + modelName;
        }

        public static string CreateKey(Item item)
        {
            return CreateKey(item.Type, item.ModelName);
        }
    }

    [Serializable]
    public class ItemSlotPosition
    {
        public ItemType type;
        public SlotSide side;
        public HumanBodyBones bone;
        public Vector3 offset;
        public Vector3 rotation;
    }
    
    public enum SlotSide
    {
        Center,
        Left,
        Right,
        Back,
        Front
    }
}

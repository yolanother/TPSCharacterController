using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Inventory.Items;
using Object = UnityEngine.Object;

namespace DoubTech.TPSCharacterController.Inventory.Slots
{
    [CreateAssetMenu(menuName = "TPS Character Controller/Slots/Typed Slot Position")]
    public class TypedSlotPosition : ScriptableObject
    {
        [SerializeField] public ItemSlotPosition position;

        public ItemType Type => position.type;
    }

    [Serializable]
    public class ModelSlotPosition
    {
        [SerializeField] public string modelName;
        [SerializeField] public ItemSlotPosition position;

        public string Key => CreateKey(position.type, modelName);
        public ItemType Type => position.type;

        public static string CreateKey(ItemType itemType, string modelName)
        {
            return itemType.ID + "::" + modelName;
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

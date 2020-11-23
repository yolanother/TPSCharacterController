using System;
using UnityEngine;

namespace DoubTech.TPSCharacterController.Inventory.Items
{
    [CreateAssetMenu(menuName = "TPS Character Controller/Items/Item Type")]
    public class ItemType : ScriptableObject
    {
        [SerializeField] private string typeName;

        public string ID => name;
        public string TypeName => typeName;
    }
}

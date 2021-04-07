using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DoubTech.TPSCharacterController.Inventory.Items
{
    public class Item : CoordinatorReferenceMonoBehaviour
    {
        [Header("Item UI Details")]
        [SerializeField] public string displayName;
        [SerializeField] public string description;
        [SerializeField] public Texture2D thumbnail;
        
        [Header("Item Configuration")]
        [SerializeField] public ItemType type;
        [SerializeField] public GameObject model;

        public GameObject Model => model;

        public string ModelName => name + "::" + model.name +
                                   (model.transform.childCount > 0 ? "::" + model.transform.GetChild(0).name : "");

        public ItemType Type => type;
        public float PickupDelay { get; set; } = 0;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DoubTech.TPSCharacterController.Inventory.Items
{
    public class Item : MonoBehaviour
    {
        [SerializeField] private ItemType type;
        [SerializeField] private GameObject model;

        public GameObject Model => model;

        public string ModelName => name + "::" + model.name +
                                   (model.transform.childCount > 0 ? "::" + model.transform.GetChild(0).name : "");

        public ItemType Type => type;
        public float PickupDelay { get; set; } = 0;
    }
}

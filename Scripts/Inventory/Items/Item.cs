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

        public ItemType Type => type;
    }
}

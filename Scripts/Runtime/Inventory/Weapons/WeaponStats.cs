using System;
using DoubTech.TPSCharacterController.Utilities;
using UnityEngine;

namespace DoubTech.TPSCharacterController.Inventory.Weapons
{
    [Preset]
    [Serializable]
    public class WeaponStats
    {
        [SerializeField] private WeaponStatsPreset preset;
        [SerializedClassField]
        [SerializeField] private WeaponStatsData data;

        public WeaponStatsData Stats => preset ? preset.Data : data;
    }

    [Serializable]
    public class WeaponStatsData
    {
        public float damage;
        public float attackCost;
    }
}

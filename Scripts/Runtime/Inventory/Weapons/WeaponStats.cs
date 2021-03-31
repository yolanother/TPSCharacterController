using System;
using DoubTech.TPSCharacterController.Utilities;
using UnityEngine;
using Sirenix.OdinInspector;

namespace DoubTech.TPSCharacterController.Inventory.Weapons
{
    [Serializable]
    public class WeaponStats
    {
        [InlineEditor]
        [SerializeField] private WeaponStatsPreset preset;
        [HideIf("preset")]
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

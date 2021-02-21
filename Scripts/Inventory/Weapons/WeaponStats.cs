using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DoubTech.TPSCharacterController.Inventory.Weapons
{
    [Serializable]
    public class WeaponStats
    {
        [SerializeField] private WeaponStatsPreset preset;
        [SerializeField] private WeaponStatsData data;

        public WeaponStatsData Stats => preset ? preset.Stats : data;
    }

    [Serializable]
    public class WeaponStatsData
    {
        public float damage;
        public float attackCost;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DoubTech.TPSCharacterController.Inventory.Weapons
{
    [CreateAssetMenu(menuName = "TPS Character Controller/Items/Weapon Stats Preset")]
    public class WeaponStatsPreset : ScriptableObject
    {
        [SerializeField] private WeaponStatsData stats;
        public WeaponStatsData Stats => stats;
    }
}

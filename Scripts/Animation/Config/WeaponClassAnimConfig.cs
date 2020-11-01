using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using Random = System.Random;

namespace DoubTech.TPSCharacterController.Animation
{
    [CreateAssetMenu(menuName = "TPS Character Controller/Weapon Animation Config")]
    public class WeaponClassAnimConfig : ScriptableObject
    {
        [SerializeField] public AnimatorOverrideController weaponClassController;
        [HideInInspector]
        [SerializeField] public OverrideDictionary overrides = new OverrideDictionary();

        [SerializeField] public AnimationConfigOverride[] primaryAttacks = new AnimationConfigOverride[9];
        [SerializeField] public AnimationConfigOverride[] secondaryAttacks = new AnimationConfigOverride[9];
        [SerializeField] public AnimationConfigOverride[] blocks = new AnimationConfigOverride[9];
        [SerializeField] public AnimationConfigOverride[] hits = new AnimationConfigOverride[9];

        [SerializeField] public AnimationConfigOverride equip;
        [SerializeField] public AnimationConfigOverride unequip;

        public AnimationConfig GetPrimaryAttack(int attackHorizontal, int attackVertical)
        {
            var attack = (1 + attackHorizontal) * 3 + (1 + attackVertical);
            if (attack >= primaryAttacks.Length) attack = UnityEngine.Random.Range(0, primaryAttacks.Length - 1);
            return null != primaryAttacks[attack] && attack < primaryAttacks.Length ? primaryAttacks[attack].Config : null;
        }
        public AnimationConfig GetSecondaryAttack(int attackHorizontal, int attackVertical)
        {
            var attack = (1 + attackHorizontal) * 3 + (1 + attackVertical);
            if (attack >= secondaryAttacks.Length) attack = UnityEngine.Random.Range(0, secondaryAttacks.Length - 1);
            return null != secondaryAttacks[attack] && attack < secondaryAttacks.Length ? secondaryAttacks[attack].Config : null;
        }

        public AnimationConfig GetBlock(int attackHorizontal, int attackVertical)
        {
            var block = (1 + attackHorizontal) * 3 + (1 + attackVertical);
            if (block >= blocks.Length) block = UnityEngine.Random.Range(0, blocks.Length - 1);
            return null != blocks[block] && block < blocks.Length ? blocks[block].Config : null;
        }

        public AnimationConfig GetHit(int attackHorizontal, int attackVertical)
        {
            var hit = (1 + attackHorizontal) * 3 + (1 + attackVertical);
            if (hit >= hits.Length) hit = UnityEngine.Random.Range(0, hits.Length - 1);
            if (null == hits[hit])
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    if (null != hits[i]) hit = i;
                }
            }
            return null != hits[hit] ? hits[hit].Config : null;
        }
    }

    [Serializable]
    public class OverrideDictionary : SerializableDictionary<AnimationConfigOverride>
    {
    }

    [Serializable]
    public class AnimationConfigOverride
    {
        [SerializeField] public string slot;
        [SerializeField] public AnimationConfigPreset preset;
        [SerializeField] private AnimationConfig config = new AnimationConfig();

        public AnimationConfig Config => preset ? preset.config : config;
    }
}

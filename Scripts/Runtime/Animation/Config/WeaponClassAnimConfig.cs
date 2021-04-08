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
        [SerializeField] public AnimationConfigOverride throwAnimation;

        [SerializeField] public AnimationConfigOverride equip;
        [SerializeField] public AnimationConfigOverride unequip;

        private void OnEnable()
        {
            primaryAttacks[0].slot = "Primary Attack - 0";
            primaryAttacks[0].Config.animationSlot = "Primary Attack - 0";
            primaryAttacks[1].slot = "Primary Attack - 1";
            primaryAttacks[1].Config.animationSlot = "Primary Attack - 0";
            primaryAttacks[2].slot = "Primary Attack - 2";
            primaryAttacks[2].Config.animationSlot = "Primary Attack - 0";
            primaryAttacks[3].slot = "Primary Attack - 3";
            primaryAttacks[3].Config.animationSlot = "Primary Attack - 0";
            primaryAttacks[4].slot = "Primary Attack - 4";
            primaryAttacks[4].Config.animationSlot = "Primary Attack - 0";
            primaryAttacks[5].slot = "Primary Attack - 5";
            primaryAttacks[5].Config.animationSlot = "Primary Attack - 0";
            primaryAttacks[6].slot = "Primary Attack - 6";
            primaryAttacks[6].Config.animationSlot = "Primary Attack - 0";
            primaryAttacks[7].slot = "Primary Attack - 7";
            primaryAttacks[7].Config.animationSlot = "Primary Attack - 0";
            primaryAttacks[8].slot = "Primary Attack - 8";
            primaryAttacks[8].Config.animationSlot = "Primary Attack - 0";
        }

        private string HorizontalName(int value)
        {
            switch (value)
            {
                case -1:
                    return "Left";
                case 0:
                    return "Mid";
                case 1:
                    return "Right";
            }

            return "";
        }

        private string VerticalName(int value)
        {
            switch (value)
            {
                case -1:
                    return "Down";
                case 0:
                    return "Mid";
                case 1:
                    return "Up";
            }

            return "";
        }

        public AnimationConfig GetPrimaryAttack(int attackHorizontal, int attackVertical)
        {
            var attack = (1 + attackHorizontal) * 3 + (1 + attackVertical);
            if (attack >= primaryAttacks.Length) attack = UnityEngine.Random.Range(0, primaryAttacks.Length - 1);
            var config = null != primaryAttacks[attack] && attack < primaryAttacks.Length ? primaryAttacks[attack].Config : null;
            if (null != config)
            {
                config.animationSlot = "Primary Attack - " + HorizontalName(attackHorizontal) + " " +
                                       VerticalName(attackVertical);
            }

            return config;
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

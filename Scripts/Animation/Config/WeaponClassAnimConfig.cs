using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = System.Random;

namespace DoubTech.TPSCharacterController.Animation
{
    [CreateAssetMenu(menuName = "TPS Character Controller/Weapon Animation Config")]
    public class WeaponClassAnimConfig : ScriptableObject
    {
        [SerializeField] public AnimatorOverrideController weaponClassController;
        [HideInInspector]
        [SerializeField] public OverrideDictionary overrides = new OverrideDictionary();

        [SerializeField] public AnimationClip[] primaryAttacks;
        [SerializeField] public AnimationClip[] secondaryAttacks;

        public AnimationClip GetPrimaryAttack(int attackHorizontal, int attackVertical)
        {
            var attack = (1 + attackHorizontal) * 3 + (1 + attackVertical);
            if (attack >= primaryAttacks.Length) attack = UnityEngine.Random.Range(0, primaryAttacks.Length - 1);
            return primaryAttacks[attack];
        }
        public AnimationClip GetSecondaryAttack(int attackHorizontal, int attackVertical)
        {
            var attack = (1 + attackHorizontal) * 3 + (1 + attackVertical);
            if (attack >= secondaryAttacks.Length) attack = UnityEngine.Random.Range(0, secondaryAttacks.Length - 1);
            return secondaryAttacks[attack];
        }
    }

    [Serializable]
    public class OverrideDictionary : SerializableDictionary<AnimationConfigOverride>
    {
    }

    [Serializable]
    public class AnimationConfigOverride
    {
        public string slot;
        public AnimationConfig config;
    }
}

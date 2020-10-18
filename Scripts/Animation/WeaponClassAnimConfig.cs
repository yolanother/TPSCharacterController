using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DoubTech.TPSCharacterController.Animation
{
    [CreateAssetMenu(menuName = "TPS Character Controller/Weapon Animation Config")]
    public class WeaponClassAnimConfig : ScriptableObject
    {
        [SerializeField] public AnimatorOverrideController weaponClassController;
        [SerializeField] public OverrideDictionary overrides = new OverrideDictionary();
    }

    [Serializable]
    public class OverrideDictionary : SerializableDictionary<AnimationConfig>
    {
    }
}

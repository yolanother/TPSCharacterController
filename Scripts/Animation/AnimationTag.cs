using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DoubTech.TPSCharacterController.Animation
{
    [Serializable]
    public class AnimationTag
    {
        [SerializeField] public AnimationTagType tagType;
        [SerializeField] public string tag;
        [SerializeField] public float time;
    }

    public enum AnimationTagType
    {
        Custom,
        AttackStart,
        AttackEnd,
        Recover,
        Windup,
        EquipGrab,
        UnequipRelease,
        Throw
    }
}

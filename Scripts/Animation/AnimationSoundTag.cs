using System;
using UnityEngine;

namespace DoubTech.TPSCharacterController.Scripts.Animation
{
    [Serializable]
    public class AnimationSoundTag
    {
        [SerializeField] public AudioClip sound;
        [SerializeField] public float time;
    }
}
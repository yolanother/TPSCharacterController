using System;
using UnityEngine;

namespace DoubTech.TPSCharacterController.Scripts.Animation
{
    [Serializable]
    public class AnimationSoundTag
    {
        [SerializeField] public AudioClip sound;
        [SerializeField] public float time;
        [Range(0f, 1f)]
        [SerializeField] public float volume = 1;
    }
}
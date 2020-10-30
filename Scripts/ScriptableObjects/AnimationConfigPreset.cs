using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Scripts.Animation;

namespace DoubTech.TPSCharacterController.Animation
{
    [CreateAssetMenu(menuName = "TPS Character Controller/Animation Config")]
    public class AnimationConfigPreset : ScriptableObject
    {
        [SerializeField] public AnimationConfig config;
    }

    [Serializable]
    public class AnimationConfig
    {
        [SerializeField] public string name;
        [SerializeField] public string animationSlot;
        [SerializeField] public AnimationClip animation;
        [SerializeField] public float enterTransition = .1f;
        [SerializeField] public float exitTransition = 0f;

        [Header("Override Layers")]
        [SerializeField] public LayerConfig fullBody = new LayerConfig(2);
        [SerializeField] public LayerConfig lowerBody = new LayerConfig(3);
        [SerializeField] public LayerConfig upperBody = new LayerConfig(4);

        [Header("Animation Tags")]
        [SerializeField] public AnimationTag[] animationTags;

        [Header("Sound Tags")] 
        [SerializeField] public AnimationSoundTag[] soundTags;
    }
}

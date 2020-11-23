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
        [Range(-0.1f, 2.0f)]
        [SerializeField] public float speed = 1;
        [SerializeField] public bool mirror = false;

        [Header("Override Layers")]
        [SerializeField] public LayerConfig lowerBody = new LayerConfig()
        {
            layerWeight = 0
        };
        [SerializeField] public LayerConfig upperBody = new LayerConfig()
        {
            layerWeight = 0
        };
        [SerializeField] public LayerConfig fullBody = new LayerConfig()
        {
            layerWeight = 1
        };
        
        [Tooltip("These will replace any weights provided for lower, upper, or full body.")]
        [SerializeField] public LayerConfigOverride[] layerOverrides;

        [Header("Animation Tags")]
        [SerializeField] public AnimationTag[] animationTags;

        [Header("Sound Tags")] 
        [SerializeField] public AnimationSoundTag[] soundTags;

        private Dictionary<AnimationTagType, List<AnimationTag>> tags;

        private Dictionary<AnimationTagType, List<AnimationTag>> Tags
        {
            get
            {
                if (null == tags)
                {
                    tags = new Dictionary<AnimationTagType, List<AnimationTag>>();
                    foreach (var tag in animationTags)
                    {
                        List<AnimationTag> tagset;
                        if (!tags.TryGetValue(tag.tagType, out tagset))
                        {
                            tagset = new List<AnimationTag>();
                            tags[tag.tagType] = tagset;
                        }
                        tagset.Add(tag);
                    }
                }

                return tags;
            }
        }

        public List<AnimationTag> GetTags(AnimationTagType type)
        {
            return Tags.TryGetValue(type, out var tags) ? tags : new List<AnimationTag>();
        }

        public bool HasTagType(AnimationTagType type)
        {
            return Tags.ContainsKey(type);
        }
    }
}

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DoubTech.TPSCharacterController.Footsteps
{
    
    [CreateAssetMenu(menuName = "TPS Character Controller/Materials/Material Type")]
    public class MaterialType : ScriptableObject
    {
        [SerializeField] public string[] materialNameAliases;
        [SerializeField] public GameObject impactEffect;
        [SerializeField] public AudioClip[] genericFootStepSounds;
        [Header("Foot Step Sounds by Boot Material")]
        [SerializeField] public FootStepSound[] footStepSounds;

        private Dictionary<string, FootStepSound> footMaterialMap;

        public AudioClip this[string footMaterial]
        {
            get
            {
                if (null == footMaterialMap)
                {
                    footMaterialMap = new Dictionary<string, FootStepSound>();
                    foreach (var sound in footStepSounds)
                    {
                        footMaterialMap[sound.footMaterial] = sound;
                    }
                }

                AudioClip[] sounds = genericFootStepSounds;
                if (null != footMaterial && footMaterialMap.TryGetValue(footMaterial, out var soundSets))
                {
                    sounds = soundSets.clips;
                }

                return sounds[UnityEngine.Random.Range(0, sounds.Length - 1)];
            }
        }
    }

    [Serializable]
    public class FootStepSound
    {
        [Tooltip("The material type of the shoe/boot making an impact")]
        public string footMaterial;
        public AudioClip[] clips;
    }
}

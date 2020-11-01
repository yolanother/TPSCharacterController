using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Animation.Control;
using UnityEngine.Audio;
using UnityEngine.Events;
using Random = System.Random;

namespace DoubTech.TPSCharacterController.Footsteps
{
    public class FootstepDetector : MonoBehaviour
    {
        [Header("Detection Properties")]
        [SerializeField] public LayerMask detectionMask;
        [SerializeField] public float touchThreshold = .05f;
        [SerializeField] public string bootMaterial = null;
        
        [Header("Audio Properties")]
        [SerializeField] private AudioMixerGroup mixerGroup;

        [SerializeField] private AudioClip[] unknownMaterialFootstepSounds;

        [SerializeField] private MaterialRegistry materialRegistry;
        
        [Header("Events")]
        [SerializeField] FootstepEvent onFootDown = new FootstepEvent();
        public FootstepEvent OnFootDown => onFootDown;

        private AvatarAnimationController controller;
        private List<Transform> footTransforms = new List<Transform>();
        private List<AudioSource> footAudio = new List<AudioSource>();
        private List<bool> footDown =new List<bool>();
        private RaycastHit hit;

        private void Awake()
        {
            controller = GetComponent<AvatarAnimationController>();
            controller.OnAvatarReady.AddListener(OnAvatarReady);
        }

        private void OnAvatarReady()
        {
            footTransforms.Clear();
            footAudio.Clear();
            footDown.Clear();
            PrepFoot(HumanBodyBones.LeftFoot);
            PrepFoot(HumanBodyBones.RightFoot);
        }

        private void PrepFoot(HumanBodyBones footbone)
        {
            var foot = controller.Animator.GetBoneTransform(footbone);
            if (foot)
            {
                footTransforms.Add(foot);
                footDown.Add(false);
                var audioSource = foot.GetComponent<AudioSource>();
                if (!audioSource)
                {
                    audioSource = foot.gameObject.AddComponent<AudioSource>();
                    audioSource.outputAudioMixerGroup = mixerGroup;
                    audioSource.spatialize = true;
                    audioSource.spatialBlend = 1;
                }
                footAudio.Add(audioSource);
            }
            
        }

        public void Update()
        {
            for (int i = 0; i < footDown.Count; i++)
            {
                var transform = footTransforms[i];
                var footDown = this.footDown[i];
                var footAudioSource = this.footAudio[i];
                if (Physics.Linecast(transform.position + Vector3.up * touchThreshold,
                    transform.position - Vector3.up * touchThreshold,
                    out hit) && hit.collider.gameObject != gameObject)
                {
                    if (!footDown)
                    {
                        footDown = true;

                        var renderer = hit.collider.GetComponentInChildren<Renderer>();
                        onFootDown.Invoke(transform, hit.collider.gameObject, renderer.material);

                        if (!footAudioSource.isPlaying)
                        {
                            MaterialType materialType = null;
                            Terrain terrain = hit.collider.GetComponent<Terrain>();
                            if (terrain)
                            {
                                materialType = materialRegistry[terrain, transform.position];
                            }

                            if (!materialType)
                            {
                                materialType = materialRegistry[renderer.material.name];
                            }

                            AudioClip clip = null;
                            if (materialType)
                            {
                                clip = materialType[bootMaterial];
                            }
                            
                            if(!clip && unknownMaterialFootstepSounds.Length > 0)
                            {
                                var sndIdx = UnityEngine.Random.Range(0, unknownMaterialFootstepSounds.Length - 1);
                                clip = unknownMaterialFootstepSounds[sndIdx];
                            }
                            
                            if (clip)
                            {
                                footAudioSource.PlayOneShot(clip);
                            }
                        }
                    }
                }
                else
                {
                    footDown = false;
                }

                this.footDown[i] = footDown;
            }
        }
        
        public void OnDestroy()
        {
        }
    }

    [Serializable]
    public class FootstepEvent : UnityEvent<Transform, GameObject, Material>
    {
    }
}

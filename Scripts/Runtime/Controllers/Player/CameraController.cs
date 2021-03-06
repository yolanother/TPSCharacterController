using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Inputs;

namespace DoubTech.TPSCharacterController
{
    public class CameraController : MonoBehaviour
    {
        [Header("Camera")]
        [SerializeField] private Camera camera;

        [SerializeField] private Vector3 cameraOffset;
        [SerializeField] private Vector3 cameraRotation;

        [Header("Input")] [SerializeField] private bool enableLook = true;
        [SerializeField]
        private bool invertMouse = false;
        [SerializeField]
        private float rotationSpeed = 2;

        [SerializeField]
        private Transform cameraPivot = null;

        [SerializeField]
        private bool targetHead = true;
        
        [SerializeField] 
        private Transform lookTarget;

        [SerializeField]
        private PlayerInput playerInput;
        
        private Vector3 rotation;
        private Vector3 trackedPosition;

        public Camera PlayerCamera
        {
            get => camera;
            set
            {
                camera = value;
                if (camera)
                {
                    var t = camera.transform;
                    t.parent = cameraPivot;
                    t.localEulerAngles = cameraRotation;
                    t.localPosition = cameraOffset;
                }
            }
        }

        private void OnValidate()
        {
            PlayerCamera = camera;
        }

        private void Awake()
        {
            if (camera)
            {
                PlayerCamera = camera;
            }
            else
            {
                PlayerCamera = Camera.main;
            }
            
            if(!playerInput) playerInput = GetComponent<PlayerInput>();
            if (targetHead)
            {
                var animator = GetComponentInChildren<Animator>();
                if(animator)
                {
                    var head = animator.GetBoneTransform(HumanBodyBones.Head);
                    if (head)
                    {
                        lookTarget = head;
                    }
                }
            } 
            else if (lookTarget)
            {
                cameraPivot.parent = lookTarget;
            }
        }

        public void LateUpdate()
        {
            if (!enableLook) return;
            
            rotation.x += playerInput.Look.Value * rotationSpeed * (invertMouse ? 1 : -1);
            rotation.x = Mathf.Clamp(rotation.x, -30, 70);
            cameraPivot.localEulerAngles = rotation;
            if (lookTarget)
            {
                trackedPosition = cameraPivot.position;
                trackedPosition.y = lookTarget.position.y;
                cameraPivot.position = trackedPosition;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!camera)
            {
                Gizmos.DrawWireSphere(cameraPivot.position - cameraOffset, .25f);
            }
        }
    }
}

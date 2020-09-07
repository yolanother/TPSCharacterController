using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Inputs;

namespace DoubTech.TPSCharacterController
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private bool invertMouse;
        [SerializeField]
        private float rotationSpeed = 2;

        [SerializeField]
        private Transform cameraPivot;

        [SerializeField]
        private bool targetHead = true;
        
        [SerializeField] 
        private Transform lookTarget;

        private PlayerInput playerInput;
        private Vector3 rotation;

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
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
            rotation.x += playerInput.Look * rotationSpeed * (invertMouse ? 1 : -1);
            rotation.x = Mathf.Clamp(rotation.x, -30, 70);
            cameraPivot.localEulerAngles = rotation;
            if (lookTarget)
            {
                cameraPivot.transform.localPosition = Vector3.up * lookTarget.position.y;
            }
        }
    }
}

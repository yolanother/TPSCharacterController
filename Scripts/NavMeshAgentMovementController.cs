using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Configuration;
using System;
using DoubTech.TPSCharacterController.Animation.Control;
using DoubTech.TPSCharacterController.Inputs;
using UnityEngine.AI;

namespace DoubTech.TPSCharacterController
{
    [RequireComponent(typeof(AvatarAnimationController))]
    public class NavMeshAgentMovementController : MonoBehaviour
    {
        [SerializeField] private float runSpeed = 6;
        [SerializeField] private float walkSpeed = 3;
        
        [SerializeField] private bool isRunning;
 
        private AvatarAnimationController anim;
        private NavMeshAgent agent;

        public bool IsRunning
        {
            get => isRunning;
            set
            {
                isRunning = value;
                agent.speed = isRunning ? runSpeed : walkSpeed;
            }
        }

        private void Awake()
        {
            anim = GetComponentInChildren<AvatarAnimationController>();
            agent = GetComponentInChildren<NavMeshAgent>();

            anim.OnAvatarReady.AddListener(() => IsRunning = isRunning);
        }

        private void Update()
        {
            if(anim.IsReady) {        
                var forwardSpeed = (Vector3.Dot(agent.velocity,transform.forward) * transform.forward.normalized).magnitude;
                var horizontalSpeed = (Vector3.Dot(agent.velocity,transform.right) * transform.right.normalized).magnitude;
                anim.IsRunning = isRunning;
                anim.Vertical = isRunning ? forwardSpeed / runSpeed : forwardSpeed / walkSpeed;
                anim.Hoizontal = isRunning ? horizontalSpeed / runSpeed : horizontalSpeed / walkSpeed;
            }
        }
    }
}

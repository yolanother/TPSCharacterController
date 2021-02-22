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

        public Vector3 Destination
        {
            get => agent.destination;
            set => MoveTo(value);
        }

        public float StoppingDistance
        {
            get => agent.stoppingDistance;
            set => agent.stoppingDistance = value;
        }

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

        public void MoveTo(Vector3 position) => MoveTo(position, IsRunning);

        public void MoveTo(Vector3 position, bool run)
        {
            IsRunning = run;
            agent.speed = run ? runSpeed : walkSpeed;
            if (agent.destination != position)
            {
                agent.SetDestination(position);
            }
        }

        public void Stop()
        {
            agent.SetDestination(transform.position);
        }
    }
}

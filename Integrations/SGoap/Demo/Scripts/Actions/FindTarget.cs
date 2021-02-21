using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Stats;
using SGoap;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace DoubTech.TPSCharacterController.SGoap.Actions
{
    public class FindTarget : BasicAction
    {
        [SerializeField] private NavMeshAgentMovementController agent;
        [SerializeField] private float radius;
        [SerializeField] private LayerMask layerMask;

        [SerializeField] private bool wander = true;
        [SerializeField] private float wanderStopDistance;
        [SerializeField] private float wanderRadius = 20;
        
        private Transform detected;

        public override float CooldownTime => 1;

        public override bool AlwaysIncludeInPlan => true;

        private void Awake()
        {
            var trigger = gameObject.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = radius;
        }

        public override EActionStatus Perform()
        {
            if (AgentData.Target)
            {
                return EActionStatus.Success;
            }
            
            if (detected)
            {
                Debug.Log("Found target: " + detected.name);
                AgentData.Target = detected;
                return EActionStatus.Success;
            }

            if (wander)
            {
                var distance = Vector3.Distance(transform.position, agent.Destination);
                if (distance == float.PositiveInfinity || distance < wanderStopDistance)
                {
                    var position = Random.insideUnitSphere * wanderRadius;
                    var height = position.y;
                    if (Terrain.activeTerrain)
                    {
                        height = Terrain.activeTerrain.SampleHeight(position);
                    }
                    else if(Physics.Raycast(position + Vector3.up * 1000, Vector3.down, out var pos))
                    {
                        position = pos.point;
                    }

                    agent.StoppingDistance = wanderStopDistance;
                    agent.MoveTo(new Vector3(position.x, height, position.z));
                }

                return EActionStatus.Running;
            }

            return EActionStatus.Failed;
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((layerMask & 1 << other.gameObject.layer) > 0)
            {
                Health health = other.transform.GetComponentInChildren<Health>();
                if (!health || health.IsAlive)
                {
                    detected = other.transform;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform == detected)
            {
                detected = null;
            }
        }
    }
}

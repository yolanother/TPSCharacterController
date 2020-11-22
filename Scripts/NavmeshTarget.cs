using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

namespace DoubTech.TPSCharacterController
{
    public class NavmeshTarget : MonoBehaviour
    {
        [SerializeField] private bool isActive;
        [SerializeField] private NavMeshAgent agent;

        public void Update()
        {
            if (isActive && agent.destination != transform.position)
            {
                agent.destination = transform.position;
            }
        }
    }
}

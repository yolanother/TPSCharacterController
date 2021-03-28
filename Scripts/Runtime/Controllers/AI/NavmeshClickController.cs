using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

namespace DoubTech.TPSCharacterController
{
    public class NavmeshClickController : MonoBehaviour
    {
        [SerializeField] private bool inEditorOnly;
        
        private NavMeshAgent agent;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        public void Update()
        {
            #if !UNITY_EDITOR
            if(inEditorOnly) return;
            #endif
            
            
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, 100000))
                {
                    agent.destination = hit.point;
                }
            }
        }
        
        public void OnDestroy()
        {
        }
    }
}

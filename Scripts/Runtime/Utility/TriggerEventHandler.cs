using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace DoubTech.TPSCharacterController.Utilities
{
    public class TriggerEventHandler : MonoBehaviour
    {
        public UnityEvent onTriggerEnter = new UnityEvent();
        public UnityEvent onTriggerExit = new UnityEvent();
        
        void OnTriggerEnter(Collider other)
        {
            onTriggerEnter.Invoke();
        }

        void OnTriggerExit(Collider other)
        {
            onTriggerExit.Invoke();
        }
    }
}

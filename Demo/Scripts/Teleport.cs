using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoubTech.TPSCharacterController.Demo
{
    public class Teleport : MonoBehaviour
    {
        [SerializeField] private Transform targetPosition;

        private CharacterController pendingTeleport;

        private void OnTriggerEnter(Collider other)
        {
            pendingTeleport = other.transform.GetComponent<CharacterController>();
        }

        private void LateUpdate()
        {
            if (pendingTeleport)
            {
                Debug.Log("Teleporting " + pendingTeleport.name + " to " + targetPosition.position);
                pendingTeleport.transform.position = targetPosition.position;
                pendingTeleport.transform.rotation = targetPosition.rotation;
                pendingTeleport = null;
            }
        }
    }
}

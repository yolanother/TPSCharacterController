using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DoubTech.TPSCharacterController
{
    public class CameraLockManager : MonoBehaviour
    {
        [SerializeField] private bool startLocked = true;
        
        private int lockCount;
        public bool IsLocked => lockCount > 0;

        public void Lock()
        {
            lockCount++;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void Unlock()
        {
            lockCount--;

            if (lockCount == 0)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void Awake()
        {
            if(startLocked) Lock();
        }
    }
}

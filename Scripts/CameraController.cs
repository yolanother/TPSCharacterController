using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DoubTech.TPSCharacterController
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private bool invertMouse;
        [SerializeField]
        private float rotationSpeed = 2;

        private Vector3 rotation;
        public void LateUpdate()
        {
            rotation.x += Input.GetAxis("Mouse Y") * rotationSpeed * (invertMouse ? 1 : -1);
            rotation.x = Mathf.Clamp(rotation.x, -30, 70);
            transform.localEulerAngles = rotation;
        }
    }
}

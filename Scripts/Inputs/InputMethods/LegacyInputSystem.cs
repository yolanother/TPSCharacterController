using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController;
using DoubTech.TPSCharacterController.Configuration;
using UnityEngine.Events;

namespace DoubTech.TPSCharacterController.Inputs.InputMethods
{
    public class LegacyInputSystem : PlayerInput
    {
        [SerializeField]
        private LegacyInputConfiguration config;

        private float horizontal;
        private float vertical;
        private float turn;
        private float look;
        
        public override float Horizontal => horizontal;
        public override float Vertical => vertical;
        public override float Turn => turn;
        public override float Look => look;

        private void Update()
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
            turn = Input.GetAxis("Mouse X");
            look = Input.GetAxis("Mouse Y");

            SendEvent(OnJump, config.jump);
            SendEvent(OnCrouch, config.crouch);
            SendEvent(OnRun, config.run);
        }

        private void SendEvent(ButtonEvent buttonEvent, KeyCode key)
        {
            if (Input.GetKeyDown(key))
            {
                buttonEvent.Invoke(ButtonEventTypes.Down);
            }
            if (Input.GetKey(key))
            {
                buttonEvent.Invoke(ButtonEventTypes.Held);
            }
            if (Input.GetKeyUp(key))
            {
                buttonEvent.Invoke(ButtonEventTypes.Up);
            }
        }
    }
}

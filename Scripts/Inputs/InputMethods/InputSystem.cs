#if ENABLE_INPUT_SYSTEM
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Inputs;
using UnityEngine.InputSystem;

namespace DoubTech.TPSCharacterController.Inputs.InputMethods
{
    public class InputSystem : PlayerInput
    {
        public InputActions inputActions;

        private void OnEnable()
        {
            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        protected override void Awake()
        {
            inputActions = new InputActions();
            InitializeButton(inputActions.Player.Jump, OnJump);
            InitializeButton(inputActions.Player.Crouch, OnCrouch);
            InitializeButton(inputActions.Player.Run, OnRun);
            InitializeButton(inputActions.Player.Attack, OnAttack);
            InitializeButton(inputActions.Player.Block, OnBlock);
        }

        private void InitializeButton(InputAction action, ButtonEvent onJump)
        {
            action.started += ctx => onJump.Invoke(ButtonEventTypes.Down);
            action.performed += ctx => onJump.Invoke(ButtonEventTypes.Held);
            action.canceled += ctx => onJump.Invoke(ButtonEventTypes.Up);
        }

        public override float Horizontal => inputActions.Player.Movement.ReadValue<Vector2>().x;
        public override float Vertical => inputActions.Player.Movement.ReadValue<Vector2>().y;
        public override float Turn => inputActions.Player.Look.ReadValue<Vector2>().x;
        public override float Look => inputActions.Player.Look.ReadValue<Vector2>().y;
    }
}
#endif
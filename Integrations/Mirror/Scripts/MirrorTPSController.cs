using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Inputs;
using Mirror;

namespace DoubTech.TPSCharacterController.Inputs.InputMethods.Mirror
{
    [RequireComponent(typeof(AuthoritativeInput))]
    [RequireComponent(typeof(CharacterMovement))]
    public class MirrorTPSController : NetworkBehaviour
    {
        [SerializeField] private Behaviour[] serverOnlyBehaviors = new Behaviour[0];
        [SerializeField] private Behaviour[] clientOnlyBehaviors = new Behaviour[0];

        private NetworkIdentity identity;
        private AuthoritativeInput authoritativeInput;
        private NetworkAnimator networkAnimator;

        private void Awake()
        {
            authoritativeInput = GetComponent<AuthoritativeInput>();
            identity = GetComponent<NetworkIdentity>();
            networkAnimator = GetComponentInChildren<NetworkAnimator>(true);
            networkAnimator.animator = GetComponentInChildren<Animator>();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            if (!isLocalPlayer)
            {
                authoritativeInput.LocalInput.enabled = false;
                var characterMovement = GetComponent<CharacterMovement>();
                characterMovement.playerInput = authoritativeInput;
                foreach (var behavior in clientOnlyBehaviors) behavior.enabled = false;

            }

            networkAnimator.enabled = true;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (isServer) return;
            
            networkAnimator.enabled = true;
            base.OnStartLocalPlayer();
            var characterMovement = GetComponent<CharacterMovement>();
            
            // Disable components that will be controlled only on the server side.
            characterMovement.enabled = false;
            foreach (var collider in GetComponentsInChildren<Collider>()) collider.enabled = false;
            foreach (var behavior in serverOnlyBehaviors) behavior.enabled = false;

            // Movement
            authoritativeInput.LocalInput.Horizontal.OnValueChanged.AddListener(UpdateHorizontal);
            authoritativeInput.LocalInput.Vertical.OnValueChanged.AddListener(UpdateVertical);
            authoritativeInput.LocalInput.Turn.OnValueChanged.AddListener(UpdateTurn);
            authoritativeInput.LocalInput.Look.OnValueChanged.AddListener(UpdateLook);
            authoritativeInput.LocalInput.Run.OnButtonEvent.AddListener(UpdateRun);
            authoritativeInput.LocalInput.Jump.OnButtonEvent.AddListener(UpdateJump);
            authoritativeInput.LocalInput.Crouch.OnButtonEvent.AddListener(UpdateCrouch);

            // Combat
            authoritativeInput.LocalInput.AttackStrong.OnButtonEvent.AddListener(UpdateAttackStrong);
            authoritativeInput.LocalInput.AttackWeak.OnButtonEvent.AddListener(UpdateAttackWeak);
            authoritativeInput.LocalInput.Block.OnButtonEvent.AddListener(UpdateBlock);
            authoritativeInput.LocalInput.CombatDirection.OnValueChanged.AddListener(UpdateCombatDirection);

            // Interaction
            authoritativeInput.LocalInput.Equip.OnButtonEvent.AddListener(UpdateEquip);
            authoritativeInput.LocalInput.Use.OnButtonEvent.AddListener(UpdateUse);
        }

        [Command] private void UpdateHorizontal(float value) => authoritativeInput.Horizontal.Value = value;
        [Command] private void UpdateVertical(float value) => authoritativeInput.Vertical.Value = value;
        [Command] private void UpdateTurn(float value) => authoritativeInput.Turn.Value = value;
        [Command] private void UpdateLook(float value) => authoritativeInput.Look.Value = value;

        [Command] private void UpdateRun(ButtonEventTypes evt) => authoritativeInput.Run.Invoke(evt);

        [Command] private void UpdateJump(ButtonEventTypes evt) => authoritativeInput.Jump.Invoke(evt);

        [Command] private void UpdateCrouch(ButtonEventTypes evt) => authoritativeInput.Crouch.Invoke(evt);

        [Command] private void UpdateAttackStrong(ButtonEventTypes evt) => authoritativeInput.AttackStrong.Invoke(evt);

        [Command] private void UpdateAttackWeak(ButtonEventTypes evt) => authoritativeInput.AttackWeak.Invoke(evt);

        [Command] private void UpdateBlock(ButtonEventTypes evt) => authoritativeInput.Block.Invoke(evt);

        [Command] private void UpdateCombatDirection(Vector2 value) => authoritativeInput.CombatDirection.Value = value;

        [Command] private void UpdateEquip(ButtonEventTypes evt) => authoritativeInput.Equip.Invoke(evt);

        [Command] private void UpdateUse(ButtonEventTypes evt) => authoritativeInput.Equip.Invoke(evt);
    }
}

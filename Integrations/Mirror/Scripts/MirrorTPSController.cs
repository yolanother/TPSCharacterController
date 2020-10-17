using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Inputs;
#if MIRROR
using Mirror;
#endif
using UnityEngine.Serialization;

namespace DoubTech.TPSCharacterController.Inputs.InputMethods.Mirror
{
#if MIRROR
    [RequireComponent(typeof(AuthoritativeInput))]
    [RequireComponent(typeof(CharacterMovement))]
    public class MirrorTPSController : NetworkBehaviour
    {
        [SerializeField] private Behaviour[] serverOnlyBehaviors = new Behaviour[0];
        [Tooltip("Behaviours that should only be enabled for local players (like camera controllers)")]
        [FormerlySerializedAs("clientOnlyBehaviors")]
        [SerializeField]
        private Behaviour[] localPlayerBehaviours = new Behaviour[0];
        [Tooltip("Game objects that should only be enabled for local players (like camera gameobjects)")]
        [SerializeField]
        private GameObject[] localPlayerGameObjects = new GameObject[0];

        private NetworkIdentity identity;
        private AuthoritativeInput authoritativeInput;
        private NetworkAnimator networkAnimator;
        private CharacterMovement characterMovement;

        private void Awake()
        {
            authoritativeInput = GetComponent<AuthoritativeInput>();
            identity = GetComponent<NetworkIdentity>();
            networkAnimator = GetComponentInChildren<NetworkAnimator>(true);
            characterMovement = GetComponent<CharacterMovement>();
        }

        private void OnValidate()
        {
            // This allows us to drop any model in to the model component and still be
            // fully wired for animation controls. If we set animator after playing
            // networkAnimator throws an error and anims are not synced.
            networkAnimator = GetComponent<NetworkAnimator>();
            if (!networkAnimator.animator)
            {
                networkAnimator.animator = GetComponentInChildren<Animator>();
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            name = "Player " + identity.netId;
            Log("Client started"
                       + "\n  isLocalPlayer: " + isLocalPlayer
                       + "\n  isClient: " + isClient
                       + "\n  isServer: " + isServer
                       + "\n  isClientOnly: " + isClientOnly
                       + "\n  isServerOnly: " + isServerOnly);
            
            DisableNonAuthoritativeComponents();
            ConfigureInputPassthrough();
        }

        private void DisableNonAuthoritativeComponents()
        {
            authoritativeInput.LocalInput.enabled = isLocalPlayer;
            
            if (!isServer)
            {
                characterMovement.enabled = false;
                foreach (var collider in GetComponentsInChildren<Collider>()) collider.enabled = false;
                foreach (var behavior in serverOnlyBehaviors) behavior.enabled = false;
            }

            if (!isLocalPlayer)
            {
                foreach (var behavior in localPlayerBehaviours) behavior.enabled = false;
                foreach (var go in localPlayerGameObjects) go.SetActive(false);
            }
        }

        private void ConfigureInputPassthrough()
        {
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
            
            // Debug...
            authoritativeInput.LocalInput.Jump.OnButtonEvent.AddListener((e) => Debug.Log("Got Local Input Jump Event: " + e));
            authoritativeInput.Jump.OnButtonEvent.AddListener((e) => Debug.Log("Got Relayed Jump Event: " + e));
        }

        private void Log(string message)
        {
            Debug.Log("[" + identity.netId + "] " + message);
        }

    #region Server Commands
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

        [Command] private void UpdateUse(ButtonEventTypes evt) => authoritativeInput.Use.Invoke(evt);
    #endregion
    }
#else
    [RequireComponent(typeof(AuthoritativeInput))]
    [RequireComponent(typeof(CharacterMovement))]
    public class MirrorTPSController : MonoBehaviour
    {
        [SerializeField] private Behaviour[] serverOnlyBehaviors = new Behaviour[0];
        [Tooltip("Behaviours that should only be enabled for local players (like camera controllers)")]
        [FormerlySerializedAs("clientOnlyBehaviors")]
        [SerializeField]
        private Behaviour[] localPlayerBehaviours = new Behaviour[0];
        [Tooltip("Game objects that should only be enabled for local players (like camera gameobjects)")]
        [SerializeField]
        private GameObject[] localPlayerGameObjects = new GameObject[0];

        private void Awake() {
            throw new Exception("Mirror has not been added to project. Make sure MIRROR is in your project defines.");
        }
    }
#endif
}
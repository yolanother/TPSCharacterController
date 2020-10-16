using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Configuration;
using System;
using DoubTech.TPSCharacterController.Inputs;

namespace DoubTech.TPSCharacterController
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMovement : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField]
        private PlayerInput playerInput;
        
        [Header("Movement")] 
        [SerializeField]
        private bool holdToRun = true;
        [SerializeField] 
        private bool holdToCrouch = false;
        [SerializeField]
        private float characterSpeed = 1.8f;
        [SerializeField] 
        private float rotationSpeed = 360;
        [SerializeField]
        private float inputLerpSpeed = 0;

        [Header("Fall")]
        [SerializeField] private float fallTransition = .1f;
        [Tooltip("The distance the player will fall before they start flailing their arms")]
        [SerializeField] private float fallDistanceUncontrolled = 3;
        [Tooltip("The distance the player will fall before they come to a hard landing and have to regain their posture before moving again")]
        [SerializeField] private float fallDistanceHardLanding = 3;
        [Tooltip("The distance the player will fall before they fall down on impact")]
        [SerializeField] private float fallDistanceFall = 5;
        [Tooltip("The distance the player will fall before they die on impact")]
        [SerializeField] private float fallDistanceDead = 20;

        [Header("Jump")]
        [SerializeField]
        private float stepDown = .5f;
        [SerializeField]
        private float jumpHeight = 3;
        [SerializeField]
        private float gravity = 25;
        [SerializeField]
        private float airControl = .5f;
        [SerializeField]
        private float jumpDampTime = 1.5f;
        [SerializeField]
        private float groundCastDistance = 0.25f;

        [Header("Weapons")] 
        [SerializeField] 
        private float equipTransition = .1f;
        [SerializeField] 
        private float unequipTransition = .1f;
        [SerializeField] 
        private float combatLayerTransitionSpeed = 10;
        
        private readonly int AnimRun = Animator.StringToHash("Run");
        private readonly int AnimCrouch = Animator.StringToHash("Crouch");
        private readonly int AnimFallDistance = Animator.StringToHash("FallDistance");
        private readonly int AnimIsJumping = Animator.StringToHash("IsJumping");
        private readonly int AnimIsFalling = Animator.StringToHash("IsFalling");
        private readonly int AnimHorizontal = Animator.StringToHash("Horizontal");
        private readonly int AnimVertical = Animator.StringToHash("Vertical");
        private readonly int AnimSpeed = Animator.StringToHash("Speed");
        private readonly int AnimTurn = Animator.StringToHash("Turn");
        private readonly int AnimEquip = Animator.StringToHash("Equip");
        private readonly int AnimUnequip = Animator.StringToHash("Unequip");

        private readonly int StateIdleJump = Animator.StringToHash("Idle Jump");
        
        private AnimStateSet StatesWalking = new AnimStateSet("Walking");
        private AnimStateSet StatesRunning = new AnimStateSet("Running");

        private AnimStateSet activeSet;

        private class AnimStateSet
        {
            public readonly int Jump;
            public readonly int ControlledFall;
            public readonly int UncontrolledFall;
            public readonly int LandToMove;
            public readonly int LandToStop;
            public readonly int LandHardStop;
            public readonly int LandFall;
            public readonly int LandFallDead;

            public AnimStateSet(string prefix)
            {
                Jump = Animator.StringToHash(prefix + " Jump Start");
                ControlledFall = Animator.StringToHash(prefix + " Controlled Fall");
                UncontrolledFall = Animator.StringToHash(prefix + " Falling Loop");
                LandToMove = Animator.StringToHash(prefix + " Land To Move");
                LandToStop = Animator.StringToHash(prefix + " Land To Stop");
                LandHardStop = Animator.StringToHash(prefix + " Land Hard Stop");
                LandFall = Animator.StringToHash(prefix + " Land Fall");
                LandFallDead = Animator.StringToHash(prefix + " Land Fall Dead");
            }
        }

        private const int AnimLayerDefault = 0;
        private const int AnimLayerCombat = 1;

        // Child Components
        private Animator animator;
        private AnimatorEventTracker animatorEventTracker;
        private CharacterController controller;
        
        private Vector3 rootMotion;
        private Vector3 velocity;
        private Quaternion newRotation;

        private bool isRunning;
        private bool isCrouching;
        private bool isJumping;
        private bool isGrounded;
        private bool isNearGround;
        
        private float turnValue;
        private float rotationY;
        
        private float previousHorizontal;
        private float previousVertical;

        private int activeLayer = 0;
        private float activeLayerWeight;
        private bool isFalling;
        private Vector3 fallStart;
        private bool isIdleJump;
        private float fallDistance;
        private bool isControlledFall;

        private bool IsInAir => isFalling || isJumping || isIdleJump;

        private void Awake()
        {
            if(!playerInput) playerInput = GetComponent<PlayerInput>();
            controller = GetComponent<CharacterController>();
            animator = GetComponentInChildren<Animator>();
            if (!animator.TryGetComponent(out animatorEventTracker))
            {
                animatorEventTracker = animator.gameObject.AddComponent<AnimatorEventTracker>();
            }
        }

        private void OnEnable()
        {
            activeSet = StatesWalking;
            playerInput.Crouch.OnButtonEvent.AddListener(evt => HandleStateChange(evt, !holdToCrouch, ref isCrouching, AnimCrouch));
            playerInput.Run.OnButtonEvent.AddListener(evt =>
            {
                HandleStateChange(evt, !holdToRun, ref isRunning, AnimRun);
                activeSet = isRunning ? StatesRunning : StatesWalking;
            });
            playerInput.Jump.OnPressed.AddListener(Jump);
            playerInput.Equip.OnPressed.AddListener(OnEquip);
            animatorEventTracker.OnAnimatorMoveEvent += OnAnimatorMove;
        }

        private void OnDisable()
        {
            animatorEventTracker.OnAnimatorMoveEvent -= OnAnimatorMove;

            playerInput.Jump.OnPressed.RemoveListener(Jump);
            playerInput.Equip.OnPressed.RemoveListener(OnEquip);
        }

        private void OnDrawGizmosSelected() {
            Gizmos.DrawLine(transform.position, transform.up * -1 * groundCastDistance);
        }

        private void Update()
        {
            RaycastHit hit;
            isNearGround = controller.isGrounded || Physics.Linecast(transform.position, transform.position - groundCastDistance * transform.up, out hit) && hit.collider.gameObject != gameObject;

            UpdateDirection();

            UpdateSpeed();

            var rotation = transform.eulerAngles; 
            transform.eulerAngles = new Vector3(
                rotation.x,
                Mathf.Lerp(rotation.y, rotationY, Time.deltaTime),
                rotation.z);

            HandleFall();
            HandleLand();
        }

        private void HandleLand()
        {
            if (isNearGround && IsInAir && velocity.y < 0)
            {
                isFalling = false;
                isJumping = false;
                isIdleJump = false;
                isControlledFall = false;
                animator.SetBool(AnimIsJumping, false);
                animator.SetBool(AnimIsFalling, false);

                if (fallDistance < fallDistanceHardLanding)
                {
                    if (playerInput.MovementMagnitude > 0)
                    {
                        animator.CrossFade(activeSet.LandToMove, fallTransition, activeLayer);
                    }
                    else
                    {
                        animator.CrossFade(activeSet.LandToStop, fallTransition, activeLayer);
                    }
                } else if (fallDistance < fallDistanceFall)
                {
                    animator.CrossFade(activeSet.LandHardStop, fallTransition, activeLayer);
                } else if (fallDistance < fallDistanceDead)
                {
                    animator.CrossFade(activeSet.LandFall, fallTransition, activeLayer);
                }
                else
                {
                    animator.CrossFade(activeSet.LandFallDead, fallTransition, activeLayer);
                }
            }
        }

        private void HandleFall()
        {
            if (!isNearGround && !IsInAir)
            {
                fallDistance = 0;
                fallStart = transform.position;
                Debug.Log("Falling!");
                isControlledFall = true;
                isFalling = true;
                isJumping = true;
                animator.SetBool(AnimIsJumping, true);
                animator.CrossFade(activeSet.ControlledFall, fallTransition);
            }

            if (isControlledFall && fallDistance > fallDistanceUncontrolled)
            {
                isControlledFall = false;
                animator.CrossFade(activeSet.UncontrolledFall, fallTransition);
            }
        }

        private void OnEquip()
        {
            if (activeLayer == AnimLayerCombat)
            {
                activeLayer = AnimLayerDefault;
                animator.CrossFade(AnimUnequip, equipTransition);
            }
            else
            {
                activeLayer = AnimLayerCombat;
                animator.CrossFade(AnimEquip, unequipTransition);
            }

            activeLayerWeight = animator.GetLayerWeight(AnimLayerCombat);
        }

        private float HandleInputLerp(float previous, float newValue)
        {
            float result = newValue;
            if (inputLerpSpeed > .001f)
            {
                newValue = Mathf.Sign(newValue) * Mathf.Ceil(Mathf.Abs(newValue));
                result = Mathf.Lerp(previous, newValue, Time.deltaTime * inputLerpSpeed);
            }

            return result;
        }

        private void UpdateDirection()
        {
            var horizontal = HandleInputLerp(previousHorizontal, playerInput.Horizontal.Value);
            var vertical = HandleInputLerp(previousVertical, playerInput.Vertical.Value);

            animator.SetFloat(AnimHorizontal, horizontal);
            animator.SetFloat(AnimVertical, vertical);
            
            previousHorizontal = horizontal;
            previousVertical = vertical;
        }

        private void UpdateSpeed() {
            float speed = playerInput.MovementMagnitude;
            if (isCrouching) speed /= 2.0f;
            if (isRunning) speed *= 2f;
            animator.SetFloat(AnimSpeed, speed);
        }

        private void Jump() {
            Debug.Log("Jump!");
            if(!isCrouching && !IsInAir) {
                isJumping = true;
                velocity = animator.velocity * jumpDampTime * characterSpeed;
                velocity.y = Mathf.Sqrt(2 * gravity * jumpHeight);
                if (playerInput.MovementMagnitude < 0.1f)
                {
                    isIdleJump = true;
                    animator.CrossFade(StateIdleJump, .1f, activeLayer);
                }
                else
                {
                    animator.SetBool(AnimIsJumping, true);
                    animator.CrossFade(activeSet.Jump, .1f, activeLayer);
                }
            }
        }

        private void OnAnimatorMove() {
            rootMotion += animator.deltaPosition;
        }

        private void FixedUpdate() {
            if(IsInAir || !controller.isGrounded) {
                UpdateInAir();
            } else {
                UpdateOnGround();
            }
        }

        private void Trigger(KeyCode key, int animHash) {
            if (Input.GetKeyDown(key)) {
                animator.SetTrigger(animHash);
            }
        }

        private void HandleStateChange(ButtonEventTypes evt, bool hold, ref bool active, int animHash) {
            switch (evt)
            {
                case ButtonEventTypes.Down:
                    if (active) {
                        active = false;
                        animator.SetBool(animHash, false);
                    } else {
                        animator.SetBool(animHash, true);
                        active = true;
                    }
                    break;
                case ButtonEventTypes.Up:
                    if (!hold) {
                        animator.SetBool(animHash, false);
                        active = false;
                    }

                    break;
            }
        }

        private void UpdateOnGround() {
            Vector3 stepForwardAmount = rootMotion * characterSpeed;
            Vector3 stepDownAmount = Vector3.down * stepDown;

            controller.Move(stepForwardAmount + stepDownAmount);
            rootMotion = Vector3.zero;

            UpdateRotation();
        }

        private void UpdateRotation()
        {
            var turnDelta = playerInput.Turn.Value * rotationSpeed;
            rotationY = transform.eulerAngles.y + turnDelta;
            if (turnDelta > 0) turnValue = Mathf.Lerp(turnValue, 1.0f, Time.deltaTime);
            else if (turnDelta < 0) turnValue = Mathf.Lerp(turnValue, -1.0f, Time.deltaTime);
            else turnValue = Mathf.Lerp(turnValue, 0, Time.deltaTime);
            animator.SetFloat(AnimTurn, turnValue);
        }

        private void LateUpdate()
        {
            UpdateLayerWeight();
        }

        private void UpdateLayerWeight()
        {
            activeLayerWeight = Mathf.Lerp(activeLayerWeight,
                activeLayer == AnimLayerCombat ? 1 : 0, Time.deltaTime * combatLayerTransitionSpeed);
            animator.SetLayerWeight(AnimLayerCombat, activeLayerWeight);
        }

        private void UpdateInAir()
        {
            var lastY = velocity.y;
            velocity.y -= gravity * Time.fixedDeltaTime;
            if (velocity.y > 0)
            {
                fallStart = transform.position;
            } else if (lastY > 0 && velocity.y < 0)
            {
                fallStart = transform.position;
            }

            fallDistance = fallStart.y - transform.position.y;
            animator.SetFloat(AnimFallDistance, fallDistance);

            Vector3 displacement = velocity * Time.fixedDeltaTime;
            displacement += CalculateAirControl();

            controller.Move(displacement);

            rootMotion = Vector3.zero;
        }

        private Vector3 CalculateAirControl() {
            return (transform.forward * playerInput.Vertical.Value + transform.right * playerInput.Horizontal.Value) * airControl / 100;
        }
    }
}

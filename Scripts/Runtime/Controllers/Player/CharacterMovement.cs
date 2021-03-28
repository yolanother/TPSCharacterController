using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Configuration;
using System;
using DoubTech.TPSCharacterController.Animation.Control;
using DoubTech.TPSCharacterController.Inputs;

namespace DoubTech.TPSCharacterController
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AvatarAnimationController))]
    public class CharacterMovement : BaseAvatarMovementController
    {
        [Header("Input")]
        [SerializeField]
        private PlayerInput playerInput;
        
        [Header("Movement")]
        [SerializeField]
        private bool strafe = true;
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
        private bool canJump = true;
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



        // Child Components
        private CharacterController controller;
        
        private Vector3 rootMotion;
        private Quaternion newRotation;

        private bool isGrounded;
        private bool isNearGround = true;
        
        private float turnValue;
        private float rotationY;
        
        private float previousHorizontal;
        private float previousVertical;

        private Vector3 fallStart;
        private bool isIdleJump;
        private float fallDistance;
        private bool isControlledFall;
        private Vector3 inAirVelocity;

        private bool IsInAir => AvatarController.IsFalling || AvatarController.IsJumping || isIdleJump;

        protected override void Awake()
        {
            base.Awake();
            if(!playerInput) playerInput = GetComponent<PlayerInput>();
            controller = GetComponent<CharacterController>();
        }

        protected override void OnCharacterReady()
        {
            AvatarController.IsRunning = false;
            playerInput.Crouch.OnButtonEvent.AddListener(evt => AvatarController.IsCrouching = HandleStateChange(evt, !holdToCrouch, AvatarController.IsCrouching));
            playerInput.Run.OnButtonEvent.AddListener(evt => AvatarController.IsRunning = HandleStateChange(evt, !holdToRun, AvatarController.IsRunning));
            playerInput.Jump.OnPressed.AddListener(Jump);
            playerInput.Throw.OnPressed.AddListener(Throw);
        }

        private void Throw()
        {
            AvatarController.Throw();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            playerInput.Jump.OnPressed.RemoveListener(Jump);
            playerInput.Throw.OnPressed.AddListener(Throw);
        }

        private void OnDrawGizmosSelected() {
            Gizmos.DrawLine(transform.position, transform.up * -1 * groundCastDistance);
        }

        protected override void OnUpdate()
        {
            RaycastHit hit;
            isNearGround = controller.isGrounded || Physics.Linecast(transform.position + transform.up * groundCastDistance / 2.0f, transform.position - groundCastDistance * transform.up, out hit) && hit.collider.gameObject != gameObject;
            
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
            if (isNearGround && IsInAir && AvatarController.Velocity.y < 0)
            {
                AvatarController.IsFalling = false;
                AvatarController.IsJumping = false;
                isIdleJump = false;
                isControlledFall = false;
                AvatarController.IsJumping = false;
                AvatarController.IsFalling = false;

                if (fallDistance < fallDistanceHardLanding)
                {
                    AvatarController.LandOnFeet(playerInput.MovementMagnitude > 0.01f);
                } else if (fallDistance < fallDistanceFall)
                {
                    AvatarController.LandHard();
                } else if (fallDistance < fallDistanceDead)
                {
                    AvatarController.LandAndFall();
                }
                else
                {
                    AvatarController.LandAndDie();
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
                AvatarController.IsFalling = true;
                AvatarController.IsJumping = true;
                AvatarController.StartControlledFall();
            }

            if (isControlledFall && fallDistance > fallDistanceUncontrolled)
            {
                isControlledFall = false;
                AvatarController.StartUncontrolledFall();
            }
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
            if (strafe)
            {
                var horizontal = HandleInputLerp(previousHorizontal, playerInput.Horizontal.Value);
                AvatarController.Hoizontal = horizontal;
                previousHorizontal = horizontal;
            }

            var vertical = HandleInputLerp(previousVertical, playerInput.Vertical.Value);
            AvatarController.Vertical = vertical;
            previousVertical = vertical;
        }

        private void UpdateSpeed() {
            float speed = playerInput.MovementMagnitude;
            if (AvatarController.IsCrouching) speed /= 2.0f;
            if (AvatarController.IsRunning) speed *= 2f;
            AvatarController.Speed = speed;
        }

        private void Jump() {
            if(canJump && !AvatarController.IsCrouching && !IsInAir) {
                Debug.Log("Jump!");
                AvatarController.IsJumping = true;
                inAirVelocity = AvatarController.Velocity * jumpDampTime * characterSpeed;
                inAirVelocity.y = Mathf.Sqrt(2 * gravity * jumpHeight);
                isIdleJump = playerInput.MovementMagnitude < 0.01f;
                AvatarController.Jump(!isIdleJump);
            }
        }

        protected override void OnAnimatorMove() {
            rootMotion += AvatarController.Animator.deltaPosition;
        }

        protected override void OnLateUpdate()
        {
            
        }

        protected override void OnFixedUpdate() {
            if (IsInAir || !isNearGround) {
                UpdateInAir();
            } else {
                UpdateOnGround();
            }
        }

        private bool HandleStateChange(ButtonEventTypes evt, bool hold, bool active) {
            switch (evt)
            {
                case ButtonEventTypes.Down:
                    if (active) {
                        active = false;
                    } else {
                        active = true;
                    }
                    break;
                case ButtonEventTypes.Up:
                    if (!hold) {
                        active = false;
                    }

                    break;
            }

            return active;
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
            if (!strafe && turnDelta == 0)
            {
                turnDelta = playerInput.Horizontal.Value * rotationSpeed * 10;
            }
            rotationY = transform.eulerAngles.y + turnDelta;
            if (turnDelta > 0) turnValue = Mathf.Lerp(turnValue, 1.0f, Time.deltaTime);
            else if (turnDelta < 0) turnValue = Mathf.Lerp(turnValue, -1.0f, Time.deltaTime);
            else turnValue = Mathf.Lerp(turnValue, 0, Time.deltaTime);
            AvatarController.Turn = turnValue;
        }

        private void UpdateInAir()
        {
            var lastY = inAirVelocity.y;
            inAirVelocity.y -= gravity * Time.fixedDeltaTime;
            if (inAirVelocity.y > 0)
            {
                fallStart = transform.position;
            } else if (lastY > 0 && inAirVelocity.y < 0)
            {
                fallStart = transform.position;
            }

            fallDistance = fallStart.y - transform.position.y;
            AvatarController.FallDistance = fallDistance;

            Vector3 displacement = inAirVelocity * Time.fixedDeltaTime;
            displacement += CalculateAirControl();

            controller.Move(displacement);

            rootMotion = Vector3.zero;
        }

        private Vector3 CalculateAirControl() {
            return (transform.forward * playerInput.Vertical.Value + transform.right * playerInput.Horizontal.Value) * airControl / 100;
        }
    }
}

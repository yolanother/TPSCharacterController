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
        private AnimatorEventTracker animatorEventTracker;
        private CharacterController controller;
        private AvatarAnimationController animController;
        
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
        private bool isReady;
        private Vector3 inAirVelocity;

        private bool IsInAir => animController.IsFalling || animController.IsJumping || isIdleJump;

        private void Awake()
        {
            if(!playerInput) playerInput = GetComponent<PlayerInput>();
            controller = GetComponent<CharacterController>();
            animController = GetComponent<AvatarAnimationController>();
        }

        public void CharacterReady()
        {
            if (isReady || !animController.IsReady) return;
            
            isReady = true;
            if (!animController.Animator.TryGetComponent(out animatorEventTracker)) {
                animatorEventTracker = animController.Animator.gameObject.AddComponent<AnimatorEventTracker>();
            }

            animController.IsRunning = false;
            playerInput.Crouch.OnButtonEvent.AddListener(evt => animController.IsCrouching = HandleStateChange(evt, !holdToCrouch, animController.IsCrouching));
            playerInput.Run.OnButtonEvent.AddListener(evt => animController.IsRunning = HandleStateChange(evt, !holdToRun, animController.IsRunning));
            playerInput.Jump.OnPressed.AddListener(Jump);
            playerInput.Equip.OnPressed.AddListener(OnEquip);
            animatorEventTracker.OnAnimatorMoveEvent += OnAnimatorMove;
        }

        private void OnEnable() {
            CharacterReady();
        }

        private void OnDisable()
        {
            isReady = false;
            animatorEventTracker.OnAnimatorMoveEvent -= OnAnimatorMove;

            playerInput.Jump.OnPressed.RemoveListener(Jump);
            playerInput.Equip.OnPressed.RemoveListener(OnEquip);
        }

        private void OnDrawGizmosSelected() {
            Gizmos.DrawLine(transform.position, transform.up * -1 * groundCastDistance);
        }

        private void Update()
        {
            if (!animController.IsReady) return;
            if(!isReady) CharacterReady();

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
            if (isNearGround && IsInAir && animController.Velocity.y < 0)
            {
                animController.IsFalling = false;
                animController.IsJumping = false;
                isIdleJump = false;
                isControlledFall = false;
                animController.IsJumping = false;
                animController.IsFalling = false;

                if (fallDistance < fallDistanceHardLanding)
                {
                    animController.LandOnFeet(playerInput.MovementMagnitude > 0.01f);
                } else if (fallDistance < fallDistanceFall)
                {
                    animController.LandHard();
                } else if (fallDistance < fallDistanceDead)
                {
                    animController.LandAndFall();
                }
                else
                {
                    animController.LandAndDie();
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
                animController.IsFalling = true;
                animController.IsJumping = true;
                animController.StartControlledFall();
            }

            if (isControlledFall && fallDistance > fallDistanceUncontrolled)
            {
                isControlledFall = false;
                animController.StartUncontrolledFall();
            }
        }

        private void OnEquip()
        {
            animController.Equip();
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

            animController.Hoizontal = horizontal;
            animController.Vertical = vertical;
            
            previousHorizontal = horizontal;
            previousVertical = vertical;
        }

        private void UpdateSpeed() {
            float speed = playerInput.MovementMagnitude;
            if (animController.IsCrouching) speed /= 2.0f;
            if (animController.IsRunning) speed *= 2f;
            animController.Speed = speed;
        }

        private void Jump() {
            if(canJump && !animController.IsCrouching && !IsInAir) {
                Debug.Log("Jump!");
                animController.IsJumping = true;
                inAirVelocity = animController.Velocity * jumpDampTime * characterSpeed;
                inAirVelocity.y = Mathf.Sqrt(2 * gravity * jumpHeight);
                isIdleJump = playerInput.MovementMagnitude < 0.01f;
                animController.Jump(!isIdleJump);
            }
        }

        private void OnAnimatorMove() {
            rootMotion += animController.Animator.deltaPosition;
        }

        private void FixedUpdate() {
            if (!isReady) return;

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
            rotationY = transform.eulerAngles.y + turnDelta;
            if (turnDelta > 0) turnValue = Mathf.Lerp(turnValue, 1.0f, Time.deltaTime);
            else if (turnDelta < 0) turnValue = Mathf.Lerp(turnValue, -1.0f, Time.deltaTime);
            else turnValue = Mathf.Lerp(turnValue, 0, Time.deltaTime);
            animController.Turn = turnValue;
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
            animController.FallDistance = fallDistance;

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

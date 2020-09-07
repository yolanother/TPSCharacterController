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
        private PlayerInput playerInput;

        [Header("Movement")] 
        [SerializeField]
        private bool holdToRun;
        [SerializeField] 
        private bool holdToCrouch;
        [SerializeField]
        private float characterSpeed = 1.8f;
        [SerializeField] 
        private float rotationSpeed = 360;
        [SerializeField]
        private float inputLerpSpeed = 0;

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


        private readonly int AnimRun = Animator.StringToHash("Run");
        private readonly int AnimCrouch = Animator.StringToHash("Crouch");
        private readonly int AnimJump = Animator.StringToHash("Jump");
        private readonly int AnimJumpSpeed = Animator.StringToHash("JumpSpeed");
        private readonly int AnimIsJumping = Animator.StringToHash("IsJumping");
        private readonly int AnimHorizontal = Animator.StringToHash("Horizontal");
        private readonly int AnimVertical = Animator.StringToHash("Vertical");
        private readonly int AnimSpeed = Animator.StringToHash("Speed");
        private readonly int AnimTurn = Animator.StringToHash("Turn");

        private readonly int StateWalkngJump = Animator.StringToHash("Walking Jump Start");
        private readonly int StateRunningJump = Animator.StringToHash("Running Jump Start");

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

        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();
            controller = GetComponent<CharacterController>();
            animator = GetComponentInChildren<Animator>();
            if (!animator.TryGetComponent(out animatorEventTracker)) {
                animatorEventTracker = animator.gameObject.AddComponent<AnimatorEventTracker>();
            }
            animatorEventTracker.OnAnimatorMoveEvent += () => OnAnimatorMove();
            
            playerInput.OnJump.AddListener(evt =>
            {
                if(evt == ButtonEventTypes.Down) Jump();
            });
            playerInput.OnCrouch.AddListener(evt => HandleStateChange(evt, holdToCrouch, ref isCrouching, AnimCrouch));
            playerInput.OnRun.AddListener(evt => HandleStateChange(evt, holdToRun, ref isRunning, AnimRun));
        }

        private void OnDrawGizmosSelected() {
            Gizmos.DrawLine(transform.position, transform.up * -1 * groundCastDistance);
        }

        private void Update()
        {
            isNearGround = Physics.Linecast(transform.position,  -1 * groundCastDistance * transform.up);

            UpdateDirection();

            UpdateSpeed();

            var rotation = transform.eulerAngles; 
            transform.eulerAngles = new Vector3(
                rotation.x,
                Mathf.Lerp(rotation.y, rotationY, Time.deltaTime),
                rotation.z);
        }

        private void UpdateDirection()
        {
            var horizontal = Mathf.Lerp(previousHorizontal, playerInput.Horizontal, Time.deltaTime * inputLerpSpeed);
            var vertical = Mathf.Lerp(previousVertical, playerInput.Vertical, Time.deltaTime * inputLerpSpeed);

            if (inputLerpSpeed < .001f)
            {
                horizontal = playerInput.Horizontal;
                vertical = playerInput.Vertical;
            }
            
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
            if(!isJumping && !isCrouching) {
                isJumping = true;
                velocity = animator.velocity * jumpDampTime * characterSpeed;
                velocity.y = Mathf.Sqrt(2 * gravity * jumpHeight);
                animator.SetBool(AnimIsJumping, true);

                if (isRunning) {
                    animator.CrossFade(StateRunningJump, .1f, 0);
                } else {
                    animator.CrossFade(StateWalkngJump, .1f, 0);
                }
            }
        }

        private void OnAnimatorMove() {
            rootMotion += animator.deltaPosition;
        }

        private void FixedUpdate() {
            if(isJumping) {
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
        }

        private void LateUpdate()
        {
            var turnDelta = playerInput.Turn * rotationSpeed;
            rotationY = transform.eulerAngles.y + turnDelta;
            if (turnDelta > 0) turnValue = Mathf.Lerp(turnValue, 1.0f, Time.deltaTime);
            else if (turnDelta < 0) turnValue = Mathf.Lerp(turnValue, -1.0f, Time.deltaTime);
            else turnValue = Mathf.Lerp(turnValue, 0, Time.deltaTime);
            animator.SetFloat(AnimTurn, turnValue);
        }

        private void UpdateInAir() {
            velocity.y -= gravity * Time.fixedDeltaTime;
            animator.SetFloat(AnimJumpSpeed, Mathf.Abs(velocity.y));

            Vector3 displacement = velocity * Time.fixedDeltaTime;
            displacement += CalculateAirControl();

            Debug.Log("Update in air: " + velocity + ", displacement=" + displacement);

            controller.Move(displacement);
            isJumping = !controller.isGrounded;
            rootMotion = Vector3.zero;
            if (isNearGround && velocity.y < 0 || !isJumping) {
                animator.SetBool(AnimIsJumping, false);
            }
            if(!isJumping) {
                Debug.Log("Speed on impact: " + velocity.y);
            }
        }

        private Vector3 CalculateAirControl() {
            return (transform.forward * playerInput.Vertical + transform.right * playerInput.Horizontal) * airControl / 100;
        }
    }
}

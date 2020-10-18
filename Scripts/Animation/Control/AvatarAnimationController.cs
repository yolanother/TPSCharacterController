using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DoubTech.TPSCharacterController.Animation.Control
{
    public class AvatarAnimationController : MonoBehaviour
    {
        [Header("Animations")]
        [SerializeField] private AnimatorOverrideController baseLocomotionController;
        [SerializeField] private WeaponClassAnimConfig equippedWeaponAnimConfig;

        [Header("Controller Config")]
        [SerializeField] private bool calculateSpeed;

        [Header("Equip/Unequip")] 
        [SerializeField] 
        private float equipTransition = .1f;
        [SerializeField] 
        private float unequipTransition = .1f;
        [SerializeField] 
        private float combatLayerTransitionSpeed = 10;
        
        private readonly int AnimRun = UnityEngine.Animator.StringToHash("Run");
        private readonly int AnimCrouch = UnityEngine.Animator.StringToHash("Crouch");
        private readonly int AnimFallDistance = UnityEngine.Animator.StringToHash("FallDistance");
        private readonly int AnimIsJumping = UnityEngine.Animator.StringToHash("IsJumping");
        private readonly int AnimIsFalling = UnityEngine.Animator.StringToHash("IsFalling");
        private readonly int AnimHorizontal = UnityEngine.Animator.StringToHash("Horizontal");
        private readonly int AnimVertical = UnityEngine.Animator.StringToHash("Vertical");
        private readonly int AnimSpeed = UnityEngine.Animator.StringToHash("Speed");
        private readonly int AnimTurn = UnityEngine.Animator.StringToHash("Turn");
        private readonly int AnimEquip = UnityEngine.Animator.StringToHash("Equip");
        private readonly int AnimUnequip = UnityEngine.Animator.StringToHash("Unequip");

        private readonly int StateIdleJump = UnityEngine.Animator.StringToHash("Idle Jump");
        
        private AnimStateSet StatesWalking = new AnimStateSet("Walking");
        private AnimStateSet StatesRunning = new AnimStateSet("Running");
        
        private readonly int AnimAttackStrong = Animator.StringToHash("AttackStrong");
        private readonly int AnimAttackWeak = Animator.StringToHash("AttackWeak");
        private readonly int AnimBlock = Animator.StringToHash("Block");
        private readonly int AnimCombatDirectionVertical = Animator.StringToHash("CombatDirectionVertical");
        private readonly int AnimCombatDirectionHorizontal = Animator.StringToHash("CombatDirectionHorizontal");

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
                Jump = UnityEngine.Animator.StringToHash(prefix + " Jump Start");
                ControlledFall = UnityEngine.Animator.StringToHash(prefix + " Controlled Fall");
                UncontrolledFall = UnityEngine.Animator.StringToHash(prefix + " Falling Loop");
                LandToMove = UnityEngine.Animator.StringToHash(prefix + " Land To Move");
                LandToStop = UnityEngine.Animator.StringToHash(prefix + " Land To Stop");
                LandHardStop = UnityEngine.Animator.StringToHash(prefix + " Land Hard Stop");
                LandFall = UnityEngine.Animator.StringToHash(prefix + " Land Fall");
                LandFallDead = UnityEngine.Animator.StringToHash(prefix + " Land Fall Dead");
            }
        }

        private const int AnimLayerDefault = 0;
        private const int AnimLayerCombat = 1;

        private Animator animator;
        private int activeLayer = 0;
        private float activeLayerWeight;
        private float fallTransition = .1f;
        private AnimatorOverrideController activeController;
        
        private Vector3 lastPosition;
        private Vector3 lastRotation;

        private bool isReady;
        public bool IsReady => isReady;

        public Animator Animator => animator;

        private float speed;
        public float Speed
        {
            get => speed;
            set
            {
                speed = value;
                animator.SetFloat(AnimSpeed, speed);
            }
        }
        
        private Vector3 velocity;
        public Vector3 Velocity
        {
            get => velocity;
        }

        private float fallDistance;
        public float FallDistance
        {
            get => fallDistance;
            set
            {
                fallDistance = value;
                animator.SetFloat(AnimFallDistance, value);
            }
        } 

        private bool isRunning;
        public bool IsRunning
        {
            get => isRunning;
            set
            {
                isRunning = value;
                activeSet = isRunning ? StatesRunning : StatesWalking;
                animator.SetBool(AnimRun, value);
            }
        }

        private bool isCrouching;

        public bool IsCrouching
        {
            get => isCrouching;
            set
            {
                isCrouching = value;
                animator.SetBool(AnimCrouch, value);
            }
        }

        private bool isJumping;
        public bool IsJumping
        {
            get => isJumping;
            set
            {
                isJumping = value;
                animator.SetBool(AnimIsJumping, isJumping);
            }
        }

        private bool isFalling;
        public bool IsFalling
        {
            get => isFalling;
            set
            {
                isFalling = value;
                animator.SetBool(AnimIsFalling, value);                
            }
        }

        private float horizontal;
        public float Hoizontal
        {
            get => horizontal;
            set
            {
                horizontal = value;
                animator.SetFloat(AnimHorizontal, horizontal);
            }
        }

        private float vertical;
        public float Vertical
        {
            get => vertical;
            set
            {
                vertical = value;
                animator.SetFloat(AnimVertical, vertical);
            }
        }

        private float turn;

        public float Turn
        {
            get => turn;
            set
            {
                turn = value;
                animator.SetFloat(AnimTurn, turn);
            }
        }

        private int attackHorizontal;

        public int AttackHorizontal
        {
            get => attackHorizontal;
            set
            {
                attackHorizontal = value;
                animator.SetInteger(AnimCombatDirectionHorizontal, value);
            }
        }
        
        private int attackVertical;

        public int AttackVertical
        {
            get => attackVertical;
            set
            {
                attackVertical = value;
                animator.SetInteger(AnimCombatDirectionVertical, value);
            }
        }

        private void OnEnable()
        {
            activeController = Instantiate(baseLocomotionController);
            CharacterReady();
        }

        private void OnDisable()
        {
            isReady = false;
        }

        private void Update()
        {
            if (!animator)
            {
                animator = GetComponentInChildren<Animator>();
                if(animator) CharacterReady();
                return;
            }

            velocity = (transform.position - lastPosition) / Time.deltaTime;
            speed = Velocity.magnitude;

            if (calculateSpeed)
            {
                // TODO: Calculate horizontal and vertical from the velocity here for AI.
            }
            
            lastPosition = transform.position;
            lastRotation = transform.eulerAngles;
        }

        private void LateUpdate() {
            if (!animator) return;

            UpdateLayerWeight();
        }

        private void UpdateLayerWeight()
        {
            activeLayerWeight = Mathf.Lerp(activeLayerWeight,
                activeLayer == AnimLayerCombat ? 1 : 0, Time.deltaTime * combatLayerTransitionSpeed);
            animator.SetLayerWeight(AnimLayerCombat, activeLayerWeight);
        }

        public void CharacterReady()
        {
            if(!animator) animator = GetComponentInChildren<Animator>();

            if (animator)
            {
                animator.runtimeAnimatorController = activeController;
                isReady = true;
            }
        }
        
        public void StrongAttack()
        {
            animator.SetTrigger(AnimAttackStrong);
        }

        public void WeakAttack()
        {
            animator.SetTrigger(AnimAttackWeak);
        }

        public void Block()
        {
            animator.SetTrigger(AnimBlock);
        }

        public void Jump(bool shouldMove = false)
        {
            if (Speed > .01f && !shouldMove)
            {
                animator.CrossFade(StateIdleJump, .1f, activeLayer);
            }
            else
            {
                animator.SetBool(AnimIsJumping, true);
                animator.CrossFade(activeSet.Jump, .1f, activeLayer);
            }
        }

        public void LandOnFeet(bool move)
        {
            if (move)
            {
                animator.CrossFade(activeSet.LandToMove, fallTransition, activeLayer);
            }
            else
            {
                animator.CrossFade(activeSet.LandToStop, fallTransition, activeLayer);
            }
        }

        public void LandHard()
        {
            animator.CrossFade(activeSet.LandHardStop, fallTransition, activeLayer);
        }

        public void LandAndFall()
        {
            animator.CrossFade(activeSet.LandFall, fallTransition, activeLayer);
        }
        
        public void LandAndDie() {
            animator.CrossFade(activeSet.LandFallDead, fallTransition, activeLayer);
        }

        public void StartControlledFall()
        {
            animator.CrossFade(activeSet.ControlledFall, fallTransition);
        }

        public void StartUncontrolledFall()
        {
            animator.CrossFade(activeSet.UncontrolledFall, fallTransition);
        }

        public void Equip()
        {
            if (activeLayer == AnimLayerCombat)
            {
                activeLayer = AnimLayerDefault;
                animator.CrossFade(AnimUnequip, equipTransition);
                ClearOverrides();
            }
            else
            {
                activeLayer = AnimLayerCombat;
                animator.CrossFade(AnimEquip, unequipTransition);
                ApplyOverrides();
            }

            activeLayerWeight = animator.GetLayerWeight(AnimLayerCombat);
        }

        private void ApplyOverrides()
        {
            List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            equippedWeaponAnimConfig.weaponClassController.GetOverrides(overrides);
            
            foreach (var slot in overrides)
            {
                if (slot.Value && slot.Value.length > 1)
                {
                    activeController[slot.Key.name] = slot.Value;
                }
            }
            
            var values = equippedWeaponAnimConfig.overrides.values;
            for(int i = 0; i < values.Count; i++)
            {
                activeController[values[i].animationSlot] = values[i].animation;
            }
        }

        private void ClearOverrides()
        {
            List<KeyValuePair<AnimationClip,AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            baseLocomotionController.GetOverrides(overrides);
            activeController.ApplyOverrides(overrides);
        }
    }
}

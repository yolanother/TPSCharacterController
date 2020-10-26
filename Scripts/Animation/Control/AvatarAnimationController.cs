using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Animation.Slots;
using UnityEngine.Events;

namespace DoubTech.TPSCharacterController.Animation.Control
{
    public class AvatarAnimationController : MonoBehaviour
    {
        [Header("Animations")] [SerializeField]
        private AnimatorOverrideController baseLocomotionController;

        [SerializeField] private WeaponClassAnimConfig equippedWeaponAnimConfig;

        public WeaponClassAnimConfig WeaponAnimConfig
        {
            get => equippedWeaponAnimConfig;
            set => equippedWeaponAnimConfig = value;
        }

        [Header("Controller Config")] 
        [SerializeField] private bool calculateSpeed;

        [Header("Equip/Unequip")] 
        [SerializeField] private float equipTransition = .1f;

        [SerializeField] private float unequipTransition = .1f;
        [SerializeField] private float combatLayerTransitionSpeed = 10;
        
        [Header("Animation Events")]
        [SerializeField] private UnityEvent onEquipGrab = new UnityEvent();
        [SerializeField] private UnityEvent onUnequipRelease = new UnityEvent();
        [SerializeField] private UnityEvent onEquipEnd = new UnityEvent();
        [SerializeField] private UnityEvent onUnequipEnd = new UnityEvent();

        public UnityEvent OnEquipGrab => onEquipGrab;
        public UnityEvent OnUnequipRelease => onUnequipRelease;
        public UnityEvent OnEquipEnd => onEquipEnd;
        public UnityEvent OnUnequipEnd => onUnequipEnd;
        
        [SerializeField] private UnityEvent onStartUse = new UnityEvent();
        [SerializeField] private UnityEvent onStopUse = new UnityEvent();

        [SerializeField] private float maxCooldown = 2;

        private const string SlotAttack = "Attack";
        private const string SlotBlock = "Block";

        private readonly int AnimRun = UnityEngine.Animator.StringToHash("Run");
        private readonly int AnimCrouch = UnityEngine.Animator.StringToHash("Crouch");
        private readonly int AnimFallDistance = UnityEngine.Animator.StringToHash("FallDistance");
        private readonly int AnimIsJumping = UnityEngine.Animator.StringToHash("IsJumping");
        private readonly int AnimIsFalling = UnityEngine.Animator.StringToHash("IsFalling");
        private readonly int AnimHorizontal = UnityEngine.Animator.StringToHash("Horizontal");
        private readonly int AnimVertical = UnityEngine.Animator.StringToHash("Vertical");
        private readonly int AnimSpeed = UnityEngine.Animator.StringToHash("Speed");
        private readonly int AnimTurn = UnityEngine.Animator.StringToHash("Turn");

        private readonly int StateIdleJump = UnityEngine.Animator.StringToHash("Idle Jump");

        private AnimStateSet StatesWalking = new AnimStateSet("Walking");
        private AnimStateSet StatesRunning = new AnimStateSet("Running");

        private readonly int AnimAttackStrong = Animator.StringToHash("Strong Attacks");
        private readonly int AnimAttackWeak = Animator.StringToHash("Weak Attacks");
        private readonly int AnimBlock = Animator.StringToHash("Blocks");
        private readonly int AnimCombatDirectionVertical = Animator.StringToHash("CombatDirectionVertical");
        private readonly int AnimCombatDirectionHorizontal = Animator.StringToHash("CombatDirectionHorizontal");

        private AnimStateSet activeSet;
        
        private static Dictionary<string, AnimationClip> processedClips = new Dictionary<string, AnimationClip>();

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
        private AnimationEventReceiver eventReceiver;
        private bool isAttacking;
        private bool isBlocking;
        private bool isUsing;
        private float cooldownStart;

        public bool IsAttacking => isAttacking;
        public bool IsBlocking => isBlocking;
        public bool IsUsing => isUsing;

        public bool IsBusy => isAttacking || isBlocking || isUsing;

        public int AttackVertical
        {
            get => attackVertical;
            set
            {
                attackVertical = value;
                animator.SetInteger(AnimCombatDirectionVertical, value);
            }
        }

        private void Awake()
        {
            activeController = Instantiate(baseLocomotionController);

            var overrides =
                new List<KeyValuePair<AnimationClip, AnimationClip>>();
            activeController.GetOverrides(overrides);            
            foreach (var pair in overrides)
            {
                if (pair.Value)
                {
                    if (!pair.Value.isLooping)
                    {
                        activeController[pair.Key.name] = PrepareClip(pair.Key.name, pair.Value);
                    }
                }
                
                AnimationEventReceiver.AddStartAnimationEvent(pair.Key, pair.Key.name);
                AnimationEventReceiver.AddStopAnimationEvent(pair.Key, pair.Key.name);
            }
            activeController.ApplyOverrides(overrides);
        }

        private void OnEnable()
        {
            CharacterReady();
        }

        private void OnDisable()
        {
            eventReceiver.OnNamedAnimationEvent.RemoveListener(OnAnimationEvent);
            eventReceiver.OnAnimationStart.RemoveListener(OnAnimationStartEvent);
            eventReceiver.OnAnimationEnd.RemoveListener(OnAnimationStopEvent);
            eventReceiver.OnTaggedAnimationEvent.RemoveListener(OnAnimationTagEvent);
            isReady = false;
        }

        private void Update()
        {
            if (!isReady)
            {
                CharacterReady();
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

        private void LateUpdate()
        {
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
            if (!animator) animator = GetComponentInChildren<Animator>();

            if (animator)
            {
                animator.runtimeAnimatorController = activeController;
                eventReceiver = animator.GetComponent<AnimationEventReceiver>();
                if (!eventReceiver)
                {
                    eventReceiver = animator.gameObject.AddComponent<AnimationEventReceiver>();
                }

                eventReceiver.OnNamedAnimationEvent.AddListener(OnAnimationEvent);
                eventReceiver.OnAnimationStart.AddListener(OnAnimationStartEvent);
                eventReceiver.OnAnimationEnd.AddListener(OnAnimationStopEvent);
                eventReceiver.OnTaggedAnimationEvent.AddListener(OnAnimationTagEvent);

                isReady = true;
            }
        }

        private void OnAnimationTagEvent(AnimationTagType tagType, string slot, string tag)
        {
            switch (tagType)
            {
                case AnimationTagType.Recover:
                    isAttacking = false;
                    Debug.Log("Stopped attacking - in recovery with " + slot);
                    break;
                case AnimationTagType.EquipGrab:
                    onEquipGrab.Invoke();
                    break;
                case AnimationTagType.UnequipRelease:
                    onUnequipRelease.Invoke();
                    break;
            }
        }

        private void OnAnimationStopEvent(string slot)
        {
            if (slot.Contains(SlotAttack))
            {
                isAttacking = false;
                Debug.Log("Stopped attacking with " + slot);
            }
            else if (slot.Contains(SlotBlock))
            {
                isBlocking = false;
            }
            else if (slot == AnimSlotDefinitions.USE.slotName)
            {
                isUsing = false;
                onStopUse.Invoke();
            }
            else if (slot == AnimSlotDefinitions.EQUIP.slotName)
            {
                onEquipEnd.Invoke();
            }
            else if (slot == AnimSlotDefinitions.UNEQUIP.slotName)
            {
                ClearOverrides();
                onUnequipEnd.Invoke();
            }
        }

        private void OnAnimationStartEvent(string slot)
        {
            if (slot.Contains(SlotAttack))
            {
                Debug.Log("Started attacking with slot " + slot);
            }
            else if (slot.Contains(SlotBlock))
            {
                
            }
            else if (slot == AnimSlotDefinitions.USE.slotName)
            {
                isUsing = true;
            }
        }

        private void OnAnimationEvent(string eventName)
        {

        }

        public void Use(AnimationClip clip)
        {
            if (!IsBusy)
            {
                onStartUse.Invoke();
                activeController[AnimSlotDefinitions.USE.slotName] =
                    PrepareClip(AnimSlotDefinitions.USE.slotName, clip);
                animator.CrossFade(AnimSlotDefinitions.USE.animStateHash, .1f);
            }
        }

        public void Use(AnimationConfig config)
        {
            if (!IsBusy)
            {
                activeController[AnimSlotDefinitions.USE.slotName] =
                    PrepareClip(AnimSlotDefinitions.USE.slotName, config.animation, config);
                animator.CrossFade(AnimSlotDefinitions.USE.animStateHash, .1f);
            }
        }

        public void StrongAttack()
        {
            if (!IsBusy || cooldownStart + maxCooldown > Time.realtimeSinceStartup)
            {
                cooldownStart = Time.realtimeSinceStartup;
                isAttacking = true;
                Debug.Log("Triggering strong attack...");
                animator.CrossFade(AnimAttackStrong, .1f);
            }
        }

        public void WeakAttack()
        {
            if (!IsBusy || cooldownStart + maxCooldown > Time.realtimeSinceStartup)
            {
                cooldownStart = Time.realtimeSinceStartup;
                isAttacking = true;
                Debug.Log("Triggering weak attack...");
                animator.CrossFade(AnimAttackWeak, .1f);
            }
        }

        public void Block()
        {
            if (!IsBusy || cooldownStart + maxCooldown > Time.realtimeSinceStartup)
            {
                cooldownStart = Time.realtimeSinceStartup;
                isBlocking = true;
                animator.CrossFade(AnimBlock, .1f);
            }
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

        public void LandAndDie()
        {
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

        private void PlaySlot(AnimationSlotDefinition slot, int layer)
        {
            var clip = activeController[slot.slotName];
            if (clip && clip.length > 0 && clip.name != slot.slotName)
            {
                animator.CrossFade(slot.animStateHash, .1f, layer);
            }
            else
            {
                OnAnimationStartEvent(slot.slotName);
                OnAnimationStopEvent(slot.slotName);
            }
        }

        public bool Equip()
        {
            if(activeLayer != AnimLayerCombat)
            {
                activeLayer = AnimLayerCombat;
                ApplyOverrides();

                PlaySlot(AnimSlotDefinitions.EQUIP, 2);
                activeLayerWeight = animator.GetLayerWeight(AnimLayerCombat);
                return true;
            }

            return false;
        }

        public bool Unequip()
        {
            if (activeLayer == AnimLayerCombat)
            {
                activeLayer = AnimLayerDefault;
                PlaySlot(AnimSlotDefinitions.UNEQUIP, 2);
                activeLayerWeight = animator.GetLayerWeight(AnimLayerCombat);
                return true;
            }

            return false;
        }

        private void ApplyOverrides()
        {
            List<KeyValuePair<AnimationClip, AnimationClip>> overrides =
                new List<KeyValuePair<AnimationClip, AnimationClip>>();

            if (equippedWeaponAnimConfig)
            {
                overrides =
                    new List<KeyValuePair<AnimationClip, AnimationClip>>();
                if(equippedWeaponAnimConfig) equippedWeaponAnimConfig.weaponClassController.GetOverrides(overrides);

                foreach (var slot in overrides)
                {
                    if (slot.Value)
                    {
                        activeController[slot.Key.name] = PrepareClip(slot.Key.name, slot.Value);
                    }
                }
                var values = equippedWeaponAnimConfig.overrides.values;
                for (int i = 0; i < values.Count; i++)
                {
                    var slot = values[i];
                    activeController[slot.slot] = PrepareClip(slot.slot, slot.config.animation, slot.config);
                }
            }
        }

        private AnimationClip PrepareClip(string slotName, AnimationClip clip, AnimationConfig config = null)
        {
            string key = slotName + "::" + clip.name;
            if (config) key += "::" + config.name;
            AnimationClip preppedClip;
            if (processedClips.TryGetValue(key, out preppedClip))
            {
                return preppedClip;
            }
            
            preppedClip = Instantiate(clip);
            processedClips[key] = preppedClip;
            
            if (!preppedClip.isLooping)
            {
                AnimationEventReceiver.AddStartAnimationEvent(preppedClip, slotName);
                AnimationEventReceiver.AddStopAnimationEvent(preppedClip, slotName);
            }

            if (config)
            {
                foreach (var tag in config.animationTags)
                {
                    AnimationEventReceiver.AddTaggedEvent(preppedClip, slotName, tag);
                }
            }

            return preppedClip;
        }

        private void ClearOverrides()
        {
            List<KeyValuePair<AnimationClip, AnimationClip>> overrides =
                new List<KeyValuePair<AnimationClip, AnimationClip>>();
            baseLocomotionController.GetOverrides(overrides);
            activeController.ApplyOverrides(overrides);
        }
    }
}

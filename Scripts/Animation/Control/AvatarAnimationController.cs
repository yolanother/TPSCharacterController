using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DoubTech.TPSCharacterController.Animation.Slots;
using DoubTech.TPSCharacterController.Scripts.Animation;
using Sirenix.OdinInspector;
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

        [Header("Animation Layers")] 
        [SerializeField] private int upperBodyLayer = 2;
        [SerializeField] private int lowerBodyLayer = 3;
        [SerializeField] private int fullBodyLayer = 4;
        
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
        [SerializeField] private UnityEvent onAvatarReady = new UnityEvent();
        [SerializeField] private UnityEvent onDeathFinished = new UnityEvent();
        
        [Header("Attack Events")]
        [SerializeField] private UnityEvent onAttackStarted = new UnityEvent();
        [SerializeField] private UnityEvent onAttackStopped = new UnityEvent();
        [SerializeField] private UnityEvent onAttackInterrupted = new UnityEvent();
        public UnityEvent OnAttackStarted => onAttackStarted;
        public UnityEvent OnAttackStopped => onAttackStopped;
        
        [Header("Block Events")]
        [SerializeField] private UnityEvent onBlockStarted = new UnityEvent();
        [SerializeField] private UnityEvent onBlockStopped = new UnityEvent();

        [Header("General Events")]
        [SerializeField] private OnTagEvent onTaggedEvent = new OnTagEvent();
        [SerializeField] private OnKnownTagEvent onKnownTagEvent = new OnKnownTagEvent();
        
        public UnityEvent OnBlockStarted => onBlockStarted;
        public UnityEvent OnBlockStopped => onBlockStopped;
        
        public UnityEvent OnAvatarReady => onAvatarReady;

        private float maxCooldown = 2;

        [SerializeField] private AudioSource audioSource;

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

        private readonly int StateWeakAttacks = Animator.StringToHash("Weak Attacks");
        private readonly int StateStrongAttacks = Animator.StringToHash("Strong Attacks");
        private readonly int StateBlocks = Animator.StringToHash("Blocks");

        private readonly int TriggerInterrupt = UnityEngine.Animator.StringToHash("InterruptAction");

        private const string DeathCompleteEvent = "OnDeathComplete";

        private AnimStateSet activeSet;
        
        private static Dictionary<string, AnimationClip> processedClips = new Dictionary<string, AnimationClip>();
        private Dictionary<string, AnimationSoundTag> soundTags = new Dictionary<string, AnimationSoundTag>();

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
        private bool isPlayingAction;
        private bool isHit;
        private bool hasStartTag;

        public bool IsAttacking => isAttacking;
        public bool IsBlocking => isBlocking;
        public bool IsUsing => isUsing;

        public bool IsBusy => isPlayingAction || isUsing || IsDead;

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
            }
            activeController.ApplyOverrides(overrides);
        }

        private void OnDisable()
        {
            eventReceiver.OnNamedAnimationEvent.RemoveListener(OnAnimationEvent);
            eventReceiver.OnTaggedAnimationEvent.RemoveListener(OnAnimationTagEvent);
            eventReceiver.OnPlaySound.RemoveListener(OnPlaySound);
            eventReceiver.OnEnterState.RemoveListener(OnAnimationStartEvent);
            eventReceiver.OnExitState.RemoveListener(OnAnimationStopEvent);
            isReady = false;
        }

        private void OnPlaySound(string clipName)
        {
            if (audioSource && soundTags.ContainsKey(clipName))
            {
                var clip = soundTags[clipName];
                audioSource.PlayOneShot(clip.sound);
            }
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
                eventReceiver.OnTaggedAnimationEvent.AddListener(OnAnimationTagEvent);
                eventReceiver.OnPlaySound.AddListener(OnPlaySound);
                eventReceiver.OnExitState.AddListener(OnAnimationStopEvent);
                eventReceiver.OnEnterState.AddListener(OnAnimationStartEvent);

                isReady = true;
                onAvatarReady.Invoke();
            }
        }

        private void OnAnimationTagEvent(AnimationTagType tagType, string slot, string tag)
        {
            switch (tagType)
            {
                case AnimationTagType.Recover:
                    isAttacking = false;
                    isPlayingAction = false;
                    Debug.Log("Stopped attacking - in recovery with " + slot);
                    break;
                case AnimationTagType.AttackEnd:
                    if (isAttacking)
                    {
                        onAttackStopped.Invoke();
                    }
                    break;
                case AnimationTagType.AttackStart:
                    onAttackStarted.Invoke();
                    break;
                case AnimationTagType.EquipGrab:
                    onEquipGrab.Invoke();
                    break;
                case AnimationTagType.UnequipRelease:
                    onUnequipRelease.Invoke();
                    break;
                case AnimationTagType.Custom:
                    onTaggedEvent.Invoke(slot, tag);
                    break;
            }

            if (tagType != AnimationTagType.Custom)
            {
                onKnownTagEvent.Invoke(slot, tagType);
            }
        }

        private void OnAnimationStartEvent(string tag)
        {
            if (tag == "action")
            {
                if (isAttacking && !hasStartTag)
                {
                    onAttackStarted.Invoke();
                }

                if (isBlocking)
                {
                    onBlockStarted.Invoke();
                }
            } else if (tag == AnimSlotDefinitions.HIT.tag)
            {
                if(isAttacking)
                {
                    isAttacking = false;
                    onAttackStopped.Invoke();
                    onAttackInterrupted.Invoke();
                }
            }
        }

        private void OnAnimationStopEvent(string tag)
        {
            if (tag == "action")
            {
                isPlayingAction = false;

                if (isAttacking)
                {
                    isAttacking = false;
                    onAttackStopped.Invoke();
                }

                if (isBlocking)
                {
                    isBlocking = false;
                    onBlockStopped.Invoke();
                }
            }
            else if (tag == AnimSlotDefinitions.HIT.tag)
            {
                isPlayingAction = false;
                isHit = false;
            } else if (tag == AnimSlotDefinitions.USE.tag)
            {
                isUsing = false;
                onStopUse.Invoke();
            }
            else if (tag == AnimSlotDefinitions.EQUIP.tag)
            {
                onEquipEnd.Invoke();
            }
            else if (tag == AnimSlotDefinitions.UNEQUIP.tag)
            {
                ClearOverrides();
                onUnequipEnd.Invoke();
            }
        }

        private void OnAnimationEvent(string eventName)
        {
            if (eventName == DeathCompleteEvent)
            {
                onDeathFinished.Invoke();
            }
        }

        public void Use(AnimationClip clip)
        {
            if (!IsBusy)
            {
                isUsing = true;
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
                isUsing = true;
                activeController[AnimSlotDefinitions.USE.slotName] =
                    PrepareClip(AnimSlotDefinitions.USE.slotName, config.animation, config);
                animator.CrossFade(AnimSlotDefinitions.USE.animStateHash, .1f);
            }
        }

        private bool Play(AnimationConfig config, string slotName, int stateHash, int layer = -1, bool interrupt = false, float transitionDuration = .1f)
        {
            if ((interrupt || !IsBusy) && null != config && config.animation)
            {
                isPlayingAction = true;
                PlayConfig(config, slotName, stateHash, layer, transitionDuration: transitionDuration);
                return true;
            }

            return false;
        }

        private void PlayConfig(AnimationConfig config, string slotName, int stateHash, int layer = -1, float transitionDuration = .1f)
        {
            var clip = PrepareClip(config.animationSlot, config.animation, config);
            animator.SetBool("Mirror" + slotName, config.mirror);
            animator.SetFloat("Speed" + slotName, config.speed);
            activeController[slotName] = clip;

            if (layer == -1)
            {
                UpdateWeights(config);
                animator.CrossFade(stateHash, transitionDuration);
                if (config.upperBody.layerWeight > 0)
                {
                    animator.CrossFade(stateHash, transitionDuration, upperBodyLayer);
                }

                if (config.lowerBody.layerWeight > 0)
                {
                    animator.CrossFade(stateHash, transitionDuration, lowerBodyLayer);
                }

                if (config.fullBody.layerWeight > 0)
                {
                    animator.CrossFade(stateHash, transitionDuration, fullBodyLayer);
                }
            }
            else
            {
                animator.CrossFade(stateHash, transitionDuration, layer);
            }
        }

        public bool PlayAction(AnimationConfig config, float transitionDuration = .1f)
        {
            return Play(config, AnimSlotDefinitions.ACTION.slotName, AnimSlotDefinitions.ACTION.animStateHash, transitionDuration: transitionDuration);
        }

        private void UpdateWeights(AnimationConfig config)
        {
            animator.SetLayerWeight(upperBodyLayer, config.upperBody.layerWeight);
            animator.SetLayerWeight(lowerBodyLayer, config.lowerBody.layerWeight);
            animator.SetLayerWeight(fullBodyLayer, config.fullBody.layerWeight);

            foreach (var layerOverride in config.layerOverrides)
            {
                animator.SetLayerWeight(layerOverride.layerId, layerOverride.layerWeight);
            }
        }

        private void Attack(AnimationConfig attack)
        {
            Debug.Log("Attack: isBusy? " + IsBusy);
            var attackStart = attack.GetTags(AnimationTagType.AttackStart);

            if (attackStart.Count > 0)
            {
                PlayAction(attack, transitionDuration: attackStart.First().time);
            }
            else
            {
                if (PlayAction(attack))
                {
                    isAttacking = true;
                }
            }
        }

        public void SecondaryAttack()
        {
            Attack(equippedWeaponAnimConfig.GetSecondaryAttack(AttackHorizontal, AttackVertical));
        }

        public void PrimaryAttack()
        {
            Attack(equippedWeaponAnimConfig.GetPrimaryAttack(AttackHorizontal, AttackVertical));
        }

        public void Block()
        {
            if (PlayAction(equippedWeaponAnimConfig.GetBlock(AttackHorizontal, AttackVertical)))
            {
                isBlocking = true;
            }
        }

        [Button]
        public void Hit()
        {
            if (!isHit && !isBlocking && !IsDead)
            {
                if (IsBusy)
                {
                    animator.SetTrigger(TriggerInterrupt);
                }
                
                isHit = true;
                Play(equippedWeaponAnimConfig.GetHit(0, 0), AnimSlotDefinitions.HIT.slotName,
                    AnimSlotDefinitions.HIT.animStateHash, activeLayer, true);
            }
        }
        
        public void Hit(Vector3 hitPoint)
        {
            if (!isHit && !isBlocking && !IsDead)
            {
                if (IsBusy)
                {
                    animator.SetTrigger(TriggerInterrupt);
                }
                
                int horizontal = 0;
                int vertical = 0;
                Vector3 direction = Vector3.zero;
                if (hitPoint != Vector3.zero)
                {
                    direction = transform.position - hitPoint;

                    var normalized = direction.normalized;
                    if (direction.magnitude > .25f)
                    {
                        horizontal = Nearest(normalized.x);
                        vertical = Nearest(Nearest(normalized.z));
                    }
                }

                Play(equippedWeaponAnimConfig.GetHit(horizontal, vertical), AnimSlotDefinitions.HIT.slotName,
                    AnimSlotDefinitions.HIT.animStateHash, activeLayer, true);
            }
        }

        private int Nearest(float value)
        {
            return (int) (Mathf.Sign(value) * Mathf.Ceil(Mathf.Abs(value)));
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

        private void PlaySlot(AnimationSlotDefinition slot, int layer, AnimationConfig config = null)
        {
            if (null != config)
            {
                if (config.animation)
                {
                    PlayConfig(config, slot.slotName, slot.animStateHash);
                }
                else
                {
                    OnAnimationStartEvent(slot.tag);
                    OnAnimationStopEvent(slot.tag);
                }
            }
            else
            {
                var clip = activeController[slot.slotName];
                if (clip && clip.length > 0 && clip.name != slot.slotName)
                {
                    animator.CrossFade(slot.animStateHash, .1f, layer);
                }
                else
                {
                    OnAnimationStartEvent(slot.tag);
                    OnAnimationStopEvent(slot.tag);
                }
            }
        }

        /// <summary>
        /// Play a custom animation clip
        /// </summary>
        /// <param name="clip"></param>
        public void PlayAnimation(AnimationClip clip)
        {
            var slot = AnimSlotDefinitions.MULTIPURPOSE.slotName;
            activeController[slot] = PrepareClip(slot, clip);
            animator.CrossFade(AnimSlotDefinitions.MULTIPURPOSE.animStateHash, .1f, activeLayer);
        }
        

        /// <summary>
        /// Plays the default death animation or a animation clip provided. When the animation finishes it will not
        /// go back to the locomotion states.
        /// </summary>
        /// <param name="clip"></param>
        [Button]
        public void Die(AnimationClip clip = null)
        {
            IsDead = true;
            if (clip)
            {
                var slot = AnimSlotDefinitions.DEATH.slotName;
                var preppedClip = PrepareClip(slot, clip);
                AnimationEventReceiver.AddNamedEvent(preppedClip, DeathCompleteEvent, Mathf.Max(0, preppedClip.length - .01f));
                activeController[slot] = preppedClip;
            } 
            PlaySlot(AnimSlotDefinitions.DEATH, activeLayer);
        }

        public bool IsDead { get; set; }

        public bool Equip()
        {
            if(activeLayer != AnimLayerCombat)
            {
                activeLayer = AnimLayerCombat;
                ApplyOverrides();

                PlaySlot(AnimSlotDefinitions.EQUIP, 2, equippedWeaponAnimConfig.equip.Config);
                activeLayerWeight = animator.GetLayerWeight(AnimLayerCombat);
                return true;
            }

            return false;
        }

        public bool Unequip(bool instant = false)
        {
            if (activeLayer == AnimLayerCombat)
            {
                activeLayer = AnimLayerDefault;
                if(!instant) PlaySlot(AnimSlotDefinitions.UNEQUIP, 2, equippedWeaponAnimConfig.unequip.Config);
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
                    activeController[slot.slot] = PrepareClip(slot.slot, slot.Config.animation, slot.Config);
                }
            }
        }

        private AnimationClip PrepareClip(string slotName, AnimationClip clip, AnimationConfig config = null)
        {
            string key = slotName + "::" + clip.name;
            if (null != config) key += "::" + config.name;
            AnimationClip preppedClip;
            if (processedClips.TryGetValue(key, out preppedClip))
            {
                return preppedClip;
            }
            
            preppedClip = Instantiate(clip);
            processedClips[key] = preppedClip;

            if (null != config)
            {
                foreach (var tag in config.animationTags)
                {
                    AnimationEventReceiver.AddTaggedEvent(preppedClip, slotName, tag);
                }

                foreach (var soundTag in config.soundTags)
                {
                    soundTags[soundTag.sound.name] = soundTag;
                    AnimationEventReceiver.AddAudioEvent(preppedClip, soundTag);
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

        public void Throw()
        {
            if (equippedWeaponAnimConfig)
            {
                PlayAction(equippedWeaponAnimConfig.throwAnimation.Config);
            }
        }
    }

    [Serializable]
    public class OnTagEvent : UnityEvent<string, string>{}

    [Serializable]
    public class OnKnownTagEvent : UnityEvent<string, AnimationTagType>{}
}

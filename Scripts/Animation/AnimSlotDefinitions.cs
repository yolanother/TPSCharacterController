using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DoubTech.TPSCharacterController.Animation.Slots
{
    public class AnimSlotDefinitions
    {

        #region Strong Attack Slots

        public static readonly ActionSlotDefinition ATTACK_STRONG_LT_UP = new ActionSlotDefinition()
        {
            actionName = "Strong Attack", positionName = "Upper Left", slotName = "Attack - Strong - Lt Up",
            animStateHash = Animator.StringToHash("Strong Attacks")
        };

        public static readonly ActionSlotDefinition ATTACK_STRONG_MID_UP = new ActionSlotDefinition()
        {
            actionName = "Strong Attack", positionName = "Upper Middle", slotName = "Attack - Strong - Mid Up",
            animStateHash = Animator.StringToHash("Strong Attacks")
        };

        public static readonly ActionSlotDefinition ATTACK_STRONG_RT_UP = new ActionSlotDefinition()
        {
            actionName = "Strong Attack", positionName = "Upper Right", slotName = "Attack - Strong - Rt Up",
            animStateHash = Animator.StringToHash("Strong Attacks")
        };

        public static readonly ActionSlotDefinition ATTACK_STRONG_LT_MID = new ActionSlotDefinition()
        {
            actionName = "Strong Attack", positionName = "Middle Left", slotName = "Attack - Strong - Lt Mid",
            animStateHash = Animator.StringToHash("Strong Attacks")
        };

        public static readonly ActionSlotDefinition ATTACK_STRONG_MID_MID = new ActionSlotDefinition()
        {
            actionName = "Strong Attack", positionName = "Middle Middle", slotName = "Attack - Strong - Mid Mid",
            animStateHash = Animator.StringToHash("Strong Attacks")
        };

        public static readonly ActionSlotDefinition ATTACK_STRONG_RT_MID = new ActionSlotDefinition()
        {
            actionName = "Strong Attack", positionName = "Middle Right", slotName = "Attack - Strong - Rt Mid",
            animStateHash = Animator.StringToHash("Strong Attacks")
        };

        public static readonly ActionSlotDefinition ATTACK_STRONG_LT_DN = new ActionSlotDefinition()
        {
            actionName = "Strong Attack", positionName = "Lower Left", slotName = "Attack - Strong - Lt Dn",
            animStateHash = Animator.StringToHash("Strong Attacks")
        };

        public static readonly ActionSlotDefinition ATTACK_STRONG_MID_DN = new ActionSlotDefinition()
        {
            actionName = "Strong Attack", positionName = "Lower Middle", slotName = "Attack - Strong - Mid Dn",
            animStateHash = Animator.StringToHash("Strong Attacks")
        };

        public static readonly ActionSlotDefinition ATTACK_STRONG_RT_DN = new ActionSlotDefinition()
        {
            actionName = "Strong Attack", positionName = "Lower Right", slotName = "Attack - Strong - Rt Dn",
            animStateHash = Animator.StringToHash("Strong Attacks")
        };

        public static readonly ActionSlotDefinition[] ATTACK_STRONG_SLOTS = new[]
        {
            ATTACK_STRONG_LT_UP, ATTACK_STRONG_MID_UP, ATTACK_STRONG_RT_UP,
            ATTACK_STRONG_LT_MID, ATTACK_STRONG_MID_MID, ATTACK_STRONG_RT_MID,
            ATTACK_STRONG_LT_DN, ATTACK_STRONG_MID_DN, ATTACK_STRONG_RT_DN
        };

        #endregion

        #region Weak Attack Slots

        public static readonly ActionSlotDefinition ATTACK_WEAK_LT_UP = new ActionSlotDefinition()
        {
            actionName = "Weak Attack", positionName = "Upper Left", slotName = "Attack - Weak - Lt Up",
            animStateHash = Animator.StringToHash("Weak Attacks")
        };

        public static readonly ActionSlotDefinition ATTACK_WEAK_MID_UP = new ActionSlotDefinition()
        {
            actionName = "Weak Attack", positionName = "Upper Middle", slotName = "Attack - Weak - Mid Up",
            animStateHash = Animator.StringToHash("Weak Attacks")
        };

        public static readonly ActionSlotDefinition ATTACK_WEAK_RT_UP = new ActionSlotDefinition()
        {
            actionName = "Weak Attack", positionName = "Upper Right", slotName = "Attack - Weak - Rt Up",
            animStateHash = Animator.StringToHash("Weak Attacks")
        };

        public static readonly ActionSlotDefinition ATTACK_WEAK_LT_MID = new ActionSlotDefinition()
        {
            actionName = "Weak Attack", positionName = "Middle Left", slotName = "Attack - Weak - Lt Mid",
            animStateHash = Animator.StringToHash("Weak Attacks")
        };

        public static readonly ActionSlotDefinition ATTACK_WEAK_MID_MID = new ActionSlotDefinition()
        {
            actionName = "Weak Attack", positionName = "Middle Middle", slotName = "Attack - Weak - Mid Mid",
            animStateHash = Animator.StringToHash("Weak Attacks")
        };

        public static readonly ActionSlotDefinition ATTACK_WEAK_RT_MID = new ActionSlotDefinition()
        {
            actionName = "Weak Attack", positionName = "Middle Right", slotName = "Attack - Weak - Rt Mid",
            animStateHash = Animator.StringToHash("Weak Attacks")
        };

        public static readonly ActionSlotDefinition ATTACK_WEAK_LT_DN = new ActionSlotDefinition()
        {
            actionName = "Weak Attack", positionName = "Lower Left", slotName = "Attack - Weak - Lt Dn",
            animStateHash = Animator.StringToHash("Weak Attacks")
        };

        public static readonly ActionSlotDefinition ATTACK_WEAK_MID_DN = new ActionSlotDefinition()
        {
            actionName = "Weak Attack", positionName = "Lower Middle", slotName = "Attack - Weak - Mid Dn",
            animStateHash = Animator.StringToHash("Weak Attacks")
        };

        public static readonly ActionSlotDefinition ATTACK_WEAK_RT_DN = new ActionSlotDefinition()
        {
            actionName = "Weak Attack", positionName = "Lower Right", slotName = "Attack - Weak - Rt Dn",
            animStateHash = Animator.StringToHash("Weak Attacks")
        };

        public static readonly ActionSlotDefinition[] ATTACK_WEAK_SLOTS = new[]
        {
            ATTACK_WEAK_LT_UP, ATTACK_WEAK_MID_UP, ATTACK_WEAK_RT_UP,
            ATTACK_WEAK_LT_MID, ATTACK_WEAK_MID_MID, ATTACK_WEAK_RT_MID,
            ATTACK_WEAK_LT_DN, ATTACK_WEAK_MID_DN, ATTACK_WEAK_RT_DN
        };

        #endregion

        #region Block Slots

        public static readonly ActionSlotDefinition BLOCK_LT_UP = new ActionSlotDefinition()
        {
            actionName = "Block Attack", positionName = "Upper Left", slotName = "Block - Lt Up",
            animStateHash = Animator.StringToHash("Block")
        };

        public static readonly ActionSlotDefinition BLOCK_MID_UP = new ActionSlotDefinition()
        {
            actionName = "Block Attack", positionName = "Upper Middle", slotName = "Block - Mid Up",
            animStateHash = Animator.StringToHash("Block")
        };

        public static readonly ActionSlotDefinition BLOCK_RT_UP = new ActionSlotDefinition()
        {
            actionName = "Block Attack", positionName = "Upper Right", slotName = "Block - Rt Up",
            animStateHash = Animator.StringToHash("Block")
        };

        public static readonly ActionSlotDefinition BLOCK_LT_MID = new ActionSlotDefinition()
        {
            actionName = "Block Attack", positionName = "Middle Left", slotName = "Block - Lt Mid",
            animStateHash = Animator.StringToHash("Block")
        };

        public static readonly ActionSlotDefinition BLOCK_MID_MID = new ActionSlotDefinition()
        {
            actionName = "Block Attack", positionName = "Middle Middle", slotName = "Block - Mid Mid",
            animStateHash = Animator.StringToHash("Block")
        };

        public static readonly ActionSlotDefinition BLOCK_RT_MID = new ActionSlotDefinition()
        {
            actionName = "Block Attack", positionName = "Middle Right", slotName = "Block - Rt Mid",
            animStateHash = Animator.StringToHash("Block")
        };

        public static readonly ActionSlotDefinition BLOCK_LT_DN = new ActionSlotDefinition()
        {
            actionName = "Block Attack", positionName = "Lower Left", slotName = "Block - Lt Dn",
            animStateHash = Animator.StringToHash("Block")
        };

        public static readonly ActionSlotDefinition BLOCK_MID_DN = new ActionSlotDefinition()
        {
            actionName = "Block Attack", positionName = "Lower Middle", slotName = "Block - Mid Dn",
            animStateHash = Animator.StringToHash("Block")
        };

        public static readonly ActionSlotDefinition BLOCK_RT_DN = new ActionSlotDefinition()
        {
            actionName = "Block Attack", positionName = "Lower Right", slotName = "Block - Rt Dn",
            animStateHash = Animator.StringToHash("Block")
        };

        public static readonly ActionSlotDefinition[] BLOCK_SLOTS = new[]
        {
            BLOCK_LT_UP, BLOCK_MID_UP, BLOCK_RT_UP,
            BLOCK_LT_MID, BLOCK_MID_MID, BLOCK_RT_MID,
            BLOCK_LT_DN, BLOCK_MID_DN, BLOCK_RT_DN
        };

        #endregion

        #region Actions

        public static readonly AnimationSlotDefinition MULTIPURPOSE = new AnimationSlotDefinition()
            {actionName = "Multipurpose", slotName = "Actions - Multipurpose", animStateHash = Animator.StringToHash("Actions - Multipurpose")};

        public static readonly AnimationSlotDefinition DEATH = new AnimationSlotDefinition()
            {actionName = "Death", slotName = "Actions - Death", animStateHash = Animator.StringToHash("Death")};

        public static readonly AnimationSlotDefinition USE = new AnimationSlotDefinition()
            {actionName = "Use", slotName = "Actions - Use", animStateHash = Animator.StringToHash("Use")};

        public static readonly AnimationSlotDefinition EQUIP = new AnimationSlotDefinition()
            {actionName = "Equip", slotName = "Actions - Equip", animStateHash = Animator.StringToHash("Equip")};

        public static readonly AnimationSlotDefinition UNEQUIP = new AnimationSlotDefinition()
            {actionName = "Unequip", slotName = "Actions - Unequip", animStateHash = Animator.StringToHash("Unequip")};

        #endregion
    }

    public class AnimationSlotDefinition
    {
        public string actionName;
        public string slotName;
        public int animStateHash;
    }

    public class ActionSlotDefinition : AnimationSlotDefinition
    {
        public string positionName;
    }
}

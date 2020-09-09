using System;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Inputs;
using UnityEngine;
using UnityEngine.UI;

public class AttackProcessor : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;
    private Animator animator;
    private CharacterController controller;
    
    private readonly int AnimAttackStrong = Animator.StringToHash("AttackStrong");
    private readonly int AnimAttackWeak = Animator.StringToHash("AttackWeak");
    private readonly int AnimBlock = Animator.StringToHash("Block");
    private readonly int AnimCombatDirectionVertical = Animator.StringToHash("CombatDirectionVertical");
    private readonly int AnimCombatDirectionHorizontal = Animator.StringToHash("CombatDirectionHorizontal");
    
    private void Awake()
    {
        if(!playerInput) playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        // This is more of an example implementation.
        // These may be replaced or just become public abstractions for
        // projects to process how they want.
        playerInput.AttackStrong.OnPressed.AddListener(AttackStrong);
        playerInput.AttackWeak.OnPressed.AddListener(AttackWeak);
        playerInput.Block.OnPressed.AddListener(Block);
        playerInput.CombatDirection.OnValueChanged.AddListener(CombatDirectionChanged);
    }

    private void OnDisable()
    {
        playerInput.AttackStrong.OnPressed.RemoveListener(AttackStrong);
        playerInput.AttackWeak.OnPressed.RemoveListener(AttackWeak);
        playerInput.Block.OnPressed.RemoveListener(Block);
        playerInput.CombatDirection.OnValueChanged.RemoveListener(CombatDirectionChanged);
    }

    private void CombatDirectionChanged(Vector2 direction)
    {
        animator.SetInteger(AnimCombatDirectionHorizontal, (int) direction.y);
        animator.SetInteger(AnimCombatDirectionVertical, (int) direction.x);
    }

    private void Block()
    {
        animator.SetTrigger(AnimBlock);
    }

    private void AttackWeak()
    {
        animator.SetTrigger(AnimAttackWeak);
    }

    private void AttackStrong()
    {
        animator.SetTrigger(AnimAttackStrong);
    }
}

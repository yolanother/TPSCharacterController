using System;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Animation.Control;
using DoubTech.TPSCharacterController.Inputs;
using UnityEngine;
using UnityEngine.UI;

namespace DoubTech.TPSCharacterController.Demo
{
    public class AttackProcessor : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;
        private Animator animator;
        private AvatarAnimationController animController;

        private void Awake()
        {
            if (!playerInput) playerInput = GetComponent<PlayerInput>();
            animController = GetComponent<AvatarAnimationController>();
            
            playerInput.AttackStrong.OnPressed.AddListener(SecondaryAttack);
            playerInput.AttackWeak.OnPressed.AddListener(PrimaryAttack);
            playerInput.Block.OnPressed.AddListener(Block);
            playerInput.CombatDirection.OnValueChanged.AddListener(CombatDirectionChanged);
        }

        private void OnDisable()
        {
            playerInput.AttackStrong.OnPressed.RemoveListener(SecondaryAttack);
            playerInput.AttackWeak.OnPressed.RemoveListener(PrimaryAttack);
            playerInput.Block.OnPressed.RemoveListener(Block);
            playerInput.CombatDirection.OnValueChanged.RemoveListener(CombatDirectionChanged);
        }

        private int Nearest(float value)
        {
            return (int) (Mathf.Sign(value) * Mathf.Ceil(Mathf.Abs(value)));
        }

        private void CombatDirectionChanged(Vector2 direction)
        {
            var normalized = direction.normalized;
            if (direction.magnitude > .25f)
            {
                animController.AttackHorizontal = Nearest(normalized.y);
                animController.AttackVertical = Nearest(Nearest(normalized.x));
            }
        }

        private void Block()
        {
            animController.Block();
        }

        private void PrimaryAttack()
        {
            animController.PrimaryAttack();
        }

        private void SecondaryAttack()
        {
            animController.SecondaryAttack();
        }
    }
}

using DoubTech.TPSCharacterController.Animation.Control;
using DoubTech.TPSCharacterController.Inputs;
using UnityEngine;

namespace DoubTech.TPSCharacterController.Demo
{
    public class AttackProcessor : MonoBehaviour
    {
        [SerializeField] public bool useCombatDirection = true;
        
        [SerializeField] private PlayerInput playerInput;
        private Animator animator;
        private AvatarAnimationController animController;

        private Vector2[] directions = new[]
        {
            new Vector2(0, 1),
            new Vector2(0, -1),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(1, -1),
            new Vector2(-1, 0),
            new Vector2(-1, 1),
            new Vector2(-1, -1)
        };

        private void Awake()
        {
            if (!playerInput) playerInput = GetComponent<PlayerInput>();
            animController = GetComponent<AvatarAnimationController>();

            if (playerInput)
            {
                playerInput.AttackStrong.OnPressed.AddListener(SecondaryAttack);
                playerInput.AttackWeak.OnPressed.AddListener(PrimaryAttack);
                playerInput.Block.OnPressed.AddListener(Block);
                playerInput.CombatDirection.OnValueChanged.AddListener(CombatDirectionChanged);
            }
        }

        private void OnDisable()
        {
            if (playerInput)
            {
                playerInput.AttackStrong.OnPressed.RemoveListener(SecondaryAttack);
                playerInput.AttackWeak.OnPressed.RemoveListener(PrimaryAttack);
                playerInput.Block.OnPressed.RemoveListener(Block);
                playerInput.CombatDirection.OnValueChanged.RemoveListener(CombatDirectionChanged);
            }
        }

        private int Nearest(float value)
        {
            return (int) (Mathf.Sign(value) * Mathf.Ceil(Mathf.Abs(value)));
        }

        public void CombatDirectionChanged(Vector2 direction)
        {
            var normalized = direction.normalized;
            if (direction.magnitude > .25f)
            {
                animController.AttackHorizontal = Nearest(normalized.y);
                animController.AttackVertical = Nearest(Nearest(normalized.x));
            }
        }

        public void Block()
        {
            UpdateAttackDirection();
            animController.Block();
        }

        public void PrimaryAttack()
        {
            UpdateAttackDirection();
            animController.PrimaryAttack();
        }

        public void SecondaryAttack()
        {
            UpdateAttackDirection();
            animController.SecondaryAttack();
        }

        private void UpdateAttackDirection()
        {
            if (!useCombatDirection)
            {
                // TODO: Make attack direction smarter.
                var direction = directions[Random.Range(0, directions.Length - 1)];
                CombatDirectionChanged(direction);
            }
        }
    }
}

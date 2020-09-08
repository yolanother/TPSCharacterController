#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;

namespace DoubTech.TPSCharacterController.Inputs.InputMethods
{
    public class InputSystem : PlayerInput
    {
        [SerializeField] private bool useMouseForCombatDirection = true;
        
        public InputActions inputActions;
        private Vector2 combatDirection;

        private void OnEnable()
        {
            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        protected void Awake()
        {
            inputActions = new InputActions();
            
            InitializeButton(inputActions.Player.Jump, Jump);
            InitializeButton(inputActions.Player.Crouch, Crouch);
            InitializeButton(inputActions.Player.Run, Run);
            InitializeButton(inputActions.Player.AttackStrong, AttackStrong);
            InitializeButton(inputActions.Player.AttackWeak, AttackWeak);
            InitializeButton(inputActions.Player.Block, Block);
            InitializeButton(inputActions.Player.Equip, Equip);
            InitializeButton(inputActions.Player.Use, Use);

            inputActions.Player.CombatDirection.performed += ctx =>
            {
                var direction = ctx.ReadValue<Vector2>();
                if (direction.magnitude > .001f)
                {
                    combatDirection = RoundUpOrDown(direction);
                    CombatDirectionChanged.Invoke(combatDirection);
                }
            };
            inputActions.Player.AxisCombatDirection.performed += ctx =>
            {
                var direction = ctx.ReadValue<Vector2>();
                if (direction.magnitude > .001f)
                {
                    combatDirection = RoundUpOrDown(direction);
                    CombatDirectionChanged.Invoke(combatDirection);
                }
            };
        }

        private void InitializeButton(InputAction action, ButtonHandler button)
        {
            action.started += ctx => button.Invoke(ButtonEventTypes.Down);
            action.performed += ctx => button.Invoke(ButtonEventTypes.Held);
            action.canceled += ctx => button.Invoke(ButtonEventTypes.Up);
        }

        public override float Horizontal => inputActions.Player.Movement.ReadValue<Vector2>().x;
        public override float Vertical => inputActions.Player.Movement.ReadValue<Vector2>().y;
        public override float Turn => inputActions.Player.Look.ReadValue<Vector2>().x;
        public override float Look => inputActions.Player.Look.ReadValue<Vector2>().y;

        public override Vector2 CombatDirection => combatDirection;

        private Vector2 RoundUpOrDown(Vector2 vector)
        {
            return new Vector2(
                RoundUpOrDown(vector.x),
                RoundUpOrDown(vector.y)
            );
        }

        private float RoundUpOrDown(float direction)
        {
            if (direction > .5f) return 1;
            if (direction < -.5f) return -1;
            return 0;
        }
    }
}
#endif
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

            InitializeValue(inputActions.Player.Movement, Horizontal, Vertical);
            InitializeValue(inputActions.Player.Look, Turn, Look);
            InitializeValue(inputActions.Player.CombatDirection, CombatDirection);
            InitializeValue(inputActions.Player.AxisCombatDirection, CombatDirection);
        }

        private void InitializeValue(InputAction action, ValueHandler<float> x, ValueHandler<float> y)
        {
            action.performed += ctx =>
            {
                Vector2 value = ctx.ReadValue<Vector2>();
                x.Value = value.x;
                y.Value = value.y;
            };
        }

        private void InitializeValue<T>(InputAction action, ValueHandler<T> value) where T : struct
        {
            action.performed += ctx =>
            {
                value.Value = ctx.ReadValue<T>();
            };
        }

        private void InitializeButton(InputAction action, ButtonHandler button)
        {
            action.started += ctx => button.Invoke(ButtonEventTypes.Down);
            action.performed += ctx => button.Invoke(ButtonEventTypes.Held);
            action.canceled += ctx => button.Invoke(ButtonEventTypes.Up);
        }

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
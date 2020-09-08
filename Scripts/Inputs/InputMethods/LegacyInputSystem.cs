using UnityEngine;
using DoubTech.TPSCharacterController.Configuration;

namespace DoubTech.TPSCharacterController.Inputs.InputMethods
{
    public class LegacyInputSystem : PlayerInput
    {
        [SerializeField]
        private LegacyInputConfiguration config = null;

        private float horizontal;
        private float vertical;
        private float turn;
        private float look;
        
        public override float Horizontal => horizontal;
        public override float Vertical => vertical;
        public override float Turn => turn;
        public override float Look => look;
        public override Vector2 CombatDirection => Vector2.zero;

        private void Update()
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
            turn = Input.GetAxis("Mouse X");
            look = Input.GetAxis("Mouse Y");

            SendEvent(Jump, config.jump);
            SendEvent(Crouch, config.crouch);
            SendEvent(Run, config.run);
            SendEvent(Equip, config.equip);

            SendMouseEvent(AttackWeak, 0);
            SendMouseEvent(AttackStrong, 1);
            SendMouseEvent(Block, 2);
        }

        private void SendMouseEvent(ButtonHandler buttonEvent, int button)
        {
            if (Input.GetMouseButtonDown(button))
            {
                buttonEvent.Invoke(ButtonEventTypes.Down);
            }
            if (Input.GetMouseButton(button))
            {
                buttonEvent.Invoke(ButtonEventTypes.Held);
            }
            if (Input.GetMouseButtonUp(button))
            {
                buttonEvent.Invoke(ButtonEventTypes.Up);
            }
        }

        private void SendEvent(ButtonHandler buttonEvent, KeyCode key)
        {
            if (Input.GetKeyDown(key))
            {
                buttonEvent.Invoke(ButtonEventTypes.Down);
            }
            if (Input.GetKey(key))
            {
                buttonEvent.Invoke(ButtonEventTypes.Held);
            }
            if (Input.GetKeyUp(key))
            {
                buttonEvent.Invoke(ButtonEventTypes.Up);
            }
        }
    }
}

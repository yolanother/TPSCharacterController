using UnityEngine;
using DoubTech.TPSCharacterController.Configuration;

namespace DoubTech.TPSCharacterController.Inputs.InputMethods
{
    public class LegacyInputSystem : PlayerInput
    {
        [SerializeField]
        private LegacyInputConfiguration config = null;

        private void Update()
        {
            Horizontal.Value = Input.GetAxis("Horizontal");
            Vertical.Value = Input.GetAxis("Vertical");
            Turn.Value = Input.GetAxis("Mouse X");
            Look.Value = Input.GetAxis("Mouse Y");

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

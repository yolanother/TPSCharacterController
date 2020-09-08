using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Inputs;

namespace DoubTech.TPSCharacterController.Inputs.InputMethods.Mirror
{
    public class AuthoritativeInput : PlayerInput
    {
        [SerializeField] private PlayerInput localInput;

        public PlayerInput LocalInput => localInput;
    }
}

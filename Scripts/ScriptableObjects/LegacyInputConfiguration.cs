using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DoubTech.TPSCharacterController.Configuration
{
    [CreateAssetMenu(menuName = "TPS Character Controller/Input Configuration")]
    public class LegacyInputConfiguration : ScriptableObject
    {
        [SerializeField]
        public KeyCode jump = KeyCode.Space;
        [SerializeField]
        public KeyCode run = KeyCode.LeftShift;
        [SerializeField]
        public KeyCode crouch = KeyCode.LeftControl;
        [SerializeField]
        public KeyCode equip = KeyCode.Q;
        [SerializeField]
        public KeyCode use = KeyCode.E;
    }
}

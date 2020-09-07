using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DoubTech.TPSCharacterController.Configuration
{
    [CreateAssetMenu(menuName = "TPS Character Controler/Input Configuration")]
    public class LegacyInputConfiguration : ScriptableObject
    {
        [SerializeField]
        public KeyCode jump;
        [SerializeField]
        public KeyCode run;
        [SerializeField]
        public KeyCode crouch;
        [SerializeField]
        public KeyCode attack;
        [SerializeField]
        public KeyCode block;
    }
}

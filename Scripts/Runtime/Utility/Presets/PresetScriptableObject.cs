using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DoubTech.TPSCharacterController.Utilities
{
    public class PresetScriptableObject<T> : ScriptableObject
    {
        [SerializeField] private T data;
        public T Data => data;
    }
}

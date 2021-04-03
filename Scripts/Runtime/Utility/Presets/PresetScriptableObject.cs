using UnityEngine;

namespace DoubTech.TPSCharacterController.Utilities
{
    public class PresetScriptableObject<T> : ScriptableObject
    {
        [SerializedClassField]
        [SerializeField] private T data;
        public T Data => data;
    }
}

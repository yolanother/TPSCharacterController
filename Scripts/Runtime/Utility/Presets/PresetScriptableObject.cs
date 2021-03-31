using UnityEngine;

namespace DoubTech.TPSCharacterController.Utilities
{
    public class PresetScriptableObject<T> : ScriptableObject
    {
        [FlattenSerializedClass]
        [SerializeField] private T data;
        public T Data => data;
    }
}

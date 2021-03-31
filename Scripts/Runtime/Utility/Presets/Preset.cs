using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DoubTech.TPSCharacterController.Utilities
{
    [Serializable]
    public class Preset<TData, TPreset> : MonoBehaviour where TPreset : PresetScriptableObject<TData>
    {
        [SerializeField] protected TPreset preset;
        [SerializeField] protected TData data;

        public TData Data => null != preset ? preset.Data : data;
    }
    
    
    
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FlattenSerializedClass : PropertyAttribute
    {
        
    }
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class Preset : PropertyAttribute
    {
        
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DoubTech.TPSCharacterController.Utilities
{
    public class PresetMonoBehaviour<TData, TPreset> : MonoBehaviour where TPreset : PresetWithData<TData>
    {
        [SerializeField] private TPreset preset;
        [SerializeField] private TData data;

        public TData Data => null != preset ? preset.Data : data;
    }
}

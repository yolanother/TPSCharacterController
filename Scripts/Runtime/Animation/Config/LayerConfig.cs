using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DoubTech.TPSCharacterController.Animation
{
    [Serializable]
    public class LayerConfig
    {
        [Range(0, 1)]
        public float layerWeight = 1;
    }

    [Serializable]
    public class LayerConfigOverride : LayerConfig
    {
        public int layerId;
    }
}

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DoubTech.TPSCharacterController.Animation
{
    [Serializable]
    public class LayerConfig
    {
        public int layerId;
        [Range(0, 1)]
        public float layerWeight = 1;

        public LayerConfig(int id)
        {
            layerId = id;
        }
    }
}

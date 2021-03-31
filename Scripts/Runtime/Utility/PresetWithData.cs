using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DoubTech.TPSCharacterController.Utilities
{
    public interface PresetWithData<out T>
    {
        T Data { get; }
    }
}

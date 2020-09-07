using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DoubTech.TPSCharacterController.Inputs
{
    public abstract class PlayerInput : MonoBehaviour
    {
        [SerializeField] public readonly ButtonEvent OnJump = new ButtonEvent();
        [SerializeField] public readonly ButtonEvent OnCrouch = new ButtonEvent();
        [SerializeField] public readonly ButtonEvent OnRun = new ButtonEvent();
        
        public abstract float Horizontal { get;  }
        public abstract float Vertical { get;  }
        public abstract float Turn { get; }
        public abstract float Look { get; }

        public float MovementMagnitude => Mathf.Sqrt(Mathf.Pow(Horizontal, 2) + Mathf.Pow(Vertical, 2));
    }

    public enum ButtonEventTypes
    {
        Down,
        Held,
        Up
    }

    [Serializable]
    public class ButtonEvent : UnityEvent<ButtonEventTypes> { }
}

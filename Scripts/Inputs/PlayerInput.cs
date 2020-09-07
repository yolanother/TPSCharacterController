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
        [SerializeField] public readonly ButtonEvent OnAttack = new ButtonEvent();
        [SerializeField] public readonly ButtonEvent OnBlock = new ButtonEvent();
        [SerializeField] public readonly ButtonEvent OnEquip = new ButtonEvent();
        
        public abstract float Horizontal { get;  }
        public abstract float Vertical { get;  }
        public abstract float Turn { get; }
        public abstract float Look { get; }
        
        public bool Jump { get; private set; }
        public bool Crouch { get; private set; }
        public bool Run { get; private set; }
        
        public bool Attack { get; private set; }
        
        public bool Block { get; private set; }
        
        public bool Equip { get; private set; }

        public float MovementMagnitude => Mathf.Sqrt(Mathf.Pow(Horizontal, 2) + Mathf.Pow(Vertical, 2));

        protected virtual void Awake()
        {
            OnCrouch.AddListener(e => Crouch = e == ButtonEventTypes.Held);
            OnJump.AddListener(e => Jump = e == ButtonEventTypes.Held);
            OnRun.AddListener(e => Run = e == ButtonEventTypes.Held);
            OnAttack.AddListener(e => Attack = e == ButtonEventTypes.Held);
            OnBlock.AddListener(e => Block = e == ButtonEventTypes.Held);
            OnEquip.AddListener(e => Equip = e == ButtonEventTypes.Held);
        }
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

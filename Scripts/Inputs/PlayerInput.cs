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
        [Header("Movement")]
        [SerializeField] public readonly ButtonHandler Run = new ButtonHandler();
        [SerializeField] public readonly ButtonHandler Jump = new ButtonHandler();
        [SerializeField] public readonly ButtonHandler Crouch = new ButtonHandler();
        
        [Header("Combat")]
        [SerializeField] public readonly ButtonHandler AttackStrong = new ButtonHandler();
        [SerializeField] public readonly ButtonHandler AttackWeak = new ButtonHandler();
        [SerializeField] public readonly ButtonHandler Block = new ButtonHandler();
        [SerializeField] public readonly CombatDirectionChangedHandler CombatDirectionChanged = new CombatDirectionChangedHandler();
        
        [Header("Interaction")]
        [SerializeField] public readonly ButtonHandler Equip = new ButtonHandler();
        [SerializeField] public readonly ButtonHandler Use = new ButtonHandler();
        
        public abstract float Horizontal { get;  }
        public abstract float Vertical { get;  }
        public abstract float Turn { get; }
        public abstract float Look { get; }
        
        public abstract Vector2 CombatDirection { get; }

        public float MovementMagnitude => Mathf.Sqrt(Mathf.Pow(Horizontal, 2) + Mathf.Pow(Vertical, 2));
    }

    public enum ButtonEventTypes
    {
        Down,
        Held,
        Up
    }

    [Serializable]
    public class ButtonHandler
    {
        private bool isHeld;
        
        [SerializeField]
        public readonly ButtonPressedEvent OnPressed = new ButtonPressedEvent();
        
        [SerializeField]
        public readonly ButtonPressedEvent OnReleased = new ButtonPressedEvent();

        [SerializeField]
        public readonly ButtonEvent OnButtonEvent = new ButtonEvent();
        
        public void Invoke(ButtonEventTypes type)
        {
            isHeld = type == ButtonEventTypes.Held;
            
            OnButtonEvent.Invoke(type);
            
            if(type == ButtonEventTypes.Down) OnPressed.Invoke();
            else if(type == ButtonEventTypes.Up) OnReleased.Invoke();
        }
    }

    [Serializable]
    public class ButtonPressedEvent : UnityEvent { }

    [Serializable]
    public class ButtonEvent : UnityEvent<ButtonEventTypes> { }

    [Serializable]
    public class CombatDirectionChangedHandler : UnityEvent<Vector2> { }
}

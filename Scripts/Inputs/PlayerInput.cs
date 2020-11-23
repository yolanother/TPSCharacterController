using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DoubTech.TPSCharacterController.Inputs
{
    public class PlayerInput : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] public readonly FloatHandler Horizontal = new FloatHandler();
        [SerializeField] public readonly FloatHandler Vertical = new FloatHandler();
        [SerializeField] public readonly FloatHandler Turn = new FloatHandler();
        [SerializeField] public readonly FloatHandler Look = new FloatHandler();
        [SerializeField] public readonly ButtonHandler Run = new ButtonHandler();
        [SerializeField] public readonly ButtonHandler Jump = new ButtonHandler();
        [SerializeField] public readonly ButtonHandler Crouch = new ButtonHandler();
        
        [Header("Combat")]
        [SerializeField] public readonly ButtonHandler AttackStrong = new ButtonHandler();
        [SerializeField] public readonly ButtonHandler AttackWeak = new ButtonHandler();
        [SerializeField] public readonly ButtonHandler Block = new ButtonHandler();
        [SerializeField] public readonly Vector2Handler CombatDirection = new Vector2Handler();
        
        [Header("Interaction")]
        [SerializeField] public readonly ButtonHandler Equip = new ButtonHandler();
        [SerializeField] public readonly ButtonHandler Use = new ButtonHandler();
        [SerializeField] public readonly ButtonHandler Throw = new ButtonHandler();

        public float MovementMagnitude => Mathf.Sqrt(Mathf.Pow(Horizontal.Value, 2) + Mathf.Pow(Vertical.Value, 2));
    }

    public enum ButtonEventTypes
    {
        Down,
        Held,
        Up
    }

    [Serializable]
    public class FloatHandler : ValueHandler<float>
    {
        [SerializeField]
        private OnFloatChanged onValueChanged = new OnFloatChanged();

        public override UnityEvent<float> OnValueChanged => onValueChanged;
    }
    
    [Serializable]
    public class Vector2Handler : ValueHandler<Vector2>
    {
        [SerializeField]
        private OnVector2Changed onValueChanged = new OnVector2Changed();

        public override UnityEvent<Vector2> OnValueChanged => onValueChanged;
    }

    [Serializable]
    public abstract class ValueHandler<T>
    {
        public abstract UnityEvent<T> OnValueChanged { get;  }
        
        private T value;

        public T Value
        {
            get => value;
            set
            {
                if (null == value && null != this.value || null != value && !value.Equals(this.value))
                {
                    this.value = value;
                    OnValueChanged?.Invoke(value);
                }
            }
        }
        
    }

    [Serializable]
    public class OnFloatChanged : UnityEvent<float> { }

    [Serializable]
    public class OnVector2Changed : UnityEvent<Vector2> { }

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

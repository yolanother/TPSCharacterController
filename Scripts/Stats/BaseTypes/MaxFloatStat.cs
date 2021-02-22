using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine.Events;

namespace DoubTech.TPSCharacterController.Stats
{
    public class MaxFloatStat : SerializableStat
    {
        [SerializeField] private float max = 100;
        [SerializeField] private float current;
        [SerializeField] private float drainRate = 0;
        [SerializeField] private float restoreRate = 0;

        public StatChangedEvent OnStatEmptyEvent = new StatChangedEvent();
        public StatChangedEvent OnStatFullEvent = new StatChangedEvent();
        public StatChangedEvent OnStatChanged = new StatChangedEvent();
        public StatPercentChangedEvent OnStatPercentChanged = new StatPercentChangedEvent();
        
        public float Max => max;

        public float Current
        {
            get => current;
            set
            {
                var last = current;
                current = Mathf.Max(0, Mathf.Min(max, value));
                if (Math.Abs(last - current) > .001f)
                {
                    OnStatChanged.Invoke(this);
                    OnStatPercentChanged.Invoke(Percent);
                    
                    if (current <= 0.001f)
                    {
                        OnStatEmptyEvent.Invoke(this);
                    }

                    if (Math.Abs(current - max) < .001f)
                    {
                        OnStatFullEvent.Invoke(this);
                    }
                }
            }
        }

        public float Percent
        {
            get => current / max;
            set => Current = max * value;
        }

        public void RestoreToMax()
        {
            Current = Max;
        }

        protected virtual void Update()
        {
            if (drainRate > 0)
            {
                Current -= drainRate * Time.deltaTime;
            }

            if (restoreRate > 0)
            {
                Current += restoreRate * Time.deltaTime;
            }
        }

        protected override void OnSave(XmlWriter writer)
        {
            WriteAttribute("current", current);
            
        }

        protected override void OnLoad(XmlReader reader)
        {
            current = ReadFloatAttribute("current", current);
            max = ReadFloatAttribute("max", max);
        }
        
        public static implicit operator float(MaxFloatStat stat) => stat.Current;

    }
    
    [Serializable] public class StatChangedEvent : UnityEvent<MaxFloatStat> {}
    [Serializable] public class StatPercentChangedEvent : UnityEvent<float> {}
}

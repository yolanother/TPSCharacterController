using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using DoubTech.TPSCharacterController.Stats.BaseTypes;
using UnityEngine.Events;

namespace DoubTech.TPSCharacterController.Stats
{
    public class Experience : FloatStat
    {
        [SerializeField] private OnExperienceChanged onExperienceChanged = new OnExperienceChanged();

        public OnExperienceChanged OnExperienceChanged => onExperienceChanged;

        public float XP
        {
            get => base.value;
            set
            {
                if (base.value != value)
                {
                    base.value = value;
                    onExperienceChanged.Invoke(base.value);
                }
            }
        }
    }

    [Serializable]
    public class OnExperienceChanged : UnityEvent<float>
    {
    }
}

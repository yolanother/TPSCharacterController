using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using DoubTech.TPSCharacterController.Stats.BaseTypes;
using UnityEngine.Events;

namespace DoubTech.TPSCharacterController.Stats
{
    public class Level : IntStat
    {
        [SerializeField] private Experience experience;
        [SerializeField] private OnLevelChanged onLevelChanged = new OnLevelChanged();

        public int CurrentLevel
        {
            get
            {
                return base.value;
            }
            protected set
            {
                if (value != base.value)
                {
                    base.value = value;
                    onLevelChanged.Invoke(base.value);
                }
            }
        }

        private int CalculateLevel(float experience)
        {
            return (int) Mathf.Ceil(Mathf.Log10(Mathf.Max(2f, experience)));
        }

        private void Awake()
        {
            if (experience)
            {
                experience.OnExperienceChanged.AddListener((e) =>
                {
                    CurrentLevel = CalculateLevel(experience.XP);
                });

                CurrentLevel = CalculateLevel(experience);
            }
            
            onLevelChanged.Invoke(CurrentLevel);
        }
    }
    
    [Serializable] public class OnLevelChanged : UnityEvent<int> {}
}

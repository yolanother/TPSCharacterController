using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Com.Doubtech.PackageManager;
using UnityEngine.Events;

namespace DoubTech.TPSCharacterController.Stats
{
    public class Health : BaseStat
    {
        [SerializeField] private UnityEvent onDied = new UnityEvent();
        
        public bool IsAlive => Current > 0;

        private void OnEnable()
        {
            OnStatEmptyEvent.AddListener(OnDied);
        }

        private void OnDisable()
        {
            OnStatEmptyEvent.RemoveListener(OnDied);
        }

        private void OnDied(BaseStat stat)
        {
            onDied.Invoke();
        }

        public void OnDamaged(float amount)
        {
            Current -= amount;
        }
    }
}

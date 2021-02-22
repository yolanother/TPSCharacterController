using UnityEngine;
using UnityEngine.Events;

namespace DoubTech.TPSCharacterController.Stats
{
    public class Health : MaxFloatStat
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

        private void OnDied(MaxFloatStat stat)
        {
            onDied.Invoke();
        }

        public void OnDamaged(float amount)
        {
            Current -= amount;
        }
    }
}

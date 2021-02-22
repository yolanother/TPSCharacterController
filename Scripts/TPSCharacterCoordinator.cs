using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Animation.Control;
using DoubTech.TPSCharacterController.Stats;
using UnityEngine.Events;

namespace DoubTech.TPSCharacterController
{
    public class TPSCharacterCoordinator : MonoBehaviour
    {
        [SerializeField] CoordinatorEffectedOtherCoordinatorEvent onKilledEnemy = new CoordinatorEffectedOtherCoordinatorEvent();
        [SerializeField] CoordinatorEffectedOtherCoordinatorEvent onDamagedEnemy = new CoordinatorEffectedOtherCoordinatorEvent();

        public CoordinatorEffectedOtherCoordinatorEvent OnDamagedEnemy => onDamagedEnemy;

        public CoordinatorEffectedOtherCoordinatorEvent OnKilledEnemy => onKilledEnemy;

        private AvatarAnimationController avatar;
        private Health health;
        private Stamina stamina;

        public Health Health
        {
            get
            {
                if (!health) health = GetComponentInChildren<Health>();
                return health;
            }
        }

        public Stamina Stamina
        {
            get
            {
                if (!stamina) stamina = GetComponentInChildren<Stamina>();
                return stamina;
            }
        }

        private Experience experience;
        public Experience Experience
        {
            get
            {
                if (!experience) experience = GetComponentInChildren<Experience>();
                return experience;
            }
        }

        private Level level;
        public Level Level
        {
            get
            {
                if (!level) level = GetComponentInChildren<Level>();
                return level;
            }
        }

        public AvatarAnimationController AvatarAnimator
        {
            get
            {
                if (!avatar) avatar = GetComponentInChildren<AvatarAnimationController>();
                return avatar;
            }
        }

        public void OnKilled(TPSCharacterCoordinator other)
        {
            onKilledEnemy.Invoke(this, other);
        }

        public void OnDamaged(TPSCharacterCoordinator other, float damage)
        {
            onDamagedEnemy.Invoke(this, other);
        }
    }
    
    [Serializable] public class CoordinatorEvent : UnityEvent<TPSCharacterCoordinator> {}
    [Serializable] public class CoordinatorEffectedOtherCoordinatorEvent : UnityEvent<TPSCharacterCoordinator, TPSCharacterCoordinator> {}
}

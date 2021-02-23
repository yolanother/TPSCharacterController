using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Demo;
using DoubTech.TPSCharacterController.Stats;
using SGoap;

namespace DoubTech.TPSCharacterController.SGoap.Actions
{
    public class AttackAction : BasicAction
    {
        [SerializeField] private AttackProcessor attackProcessor;
        public override float CooldownTime => Random.Range(0, 1.0f);

        public override bool AlwaysIncludeInPlan => true;

        private Vector2[] directions = new[]
        {
            new Vector2(0, 1),
            new Vector2(0, -1),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(1, -1),
            new Vector2(-1, 0),
            new Vector2(-1, 1),
            new Vector2(-1, -1)
        };

        private Transform target;
        private Health health;

        public override EActionStatus Perform()
        {
            if (target != AgentData.Target)
            {
                target = AgentData.Target;
                health = target.GetComponentInChildren<Health>();
            }

            if (health && !health.IsAlive)
            {
                AgentData.Target = null;
                return EActionStatus.Failed;
            }
            
            // TODO: Make this smarter. Maybe stamina based
            if (Random.Range(0, 1) > .25f)
            {
                attackProcessor.PrimaryAttack();
            }
            else
            {
                attackProcessor.SecondaryAttack();
            }

            return EActionStatus.Success;
        }
    }
}

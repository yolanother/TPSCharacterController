using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Demo;
using SGoap;

namespace DoubTech.TPSCharacterController.SGoap.Actions
{
    public class AttackAction : BasicAction
    {
        [SerializeField] private AttackProcessor attackProcessor;
        public override float CooldownTime => .25f;

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

        public override EActionStatus Perform()
        {
            // TODO: Make attack direction smarter.
            var direction = directions[Random.Range(0, directions.Length - 1)];
            
            attackProcessor.CombatDirectionChanged(direction);
            
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

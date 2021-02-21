using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SGoap;
using UnityEngine.AI;

namespace DoubTech.TPSCharacterController.SGoap.Actions
{
    public class FollowTarget : BasicAction
    {
        [SerializeField] private float targetRange;
        [SerializeField] private NavMeshAgentMovementController agent;
        [SerializeField] private bool run = true;


        public override bool PostPerform()
        {
            agent.IsRunning = false;
            return base.PostPerform();
        }

        public override EActionStatus Perform()
        {
            if (Vector3.Distance(AgentData.Target.position, transform.position) < targetRange)
            {
                agent.Stop();
                agent.IsRunning = false;
                return EActionStatus.Success;
            }
            else
            {
                agent.MoveTo(AgentData.Target.position, run);
                return EActionStatus.Running;
            }
        }
    }
}

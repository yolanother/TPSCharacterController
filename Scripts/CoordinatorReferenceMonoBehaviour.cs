using UnityEngine;

namespace DoubTech.TPSCharacterController
{
    public class CoordinatorReferenceMonoBehaviour : MonoBehaviour
    {
        private TPSCharacterCoordinator coordinator;

        public TPSCharacterCoordinator Coordinator
        {
            get
            {
                if (!coordinator) coordinator = GetComponentInParent<TPSCharacterCoordinator>();
                return coordinator;
            }
            protected set => coordinator = value;
        }
    }
}
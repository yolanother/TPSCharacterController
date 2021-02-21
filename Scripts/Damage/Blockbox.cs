using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DoubTech.TPSCharacterController.Damage
{
    public class Blockbox : MonoBehaviour
    {
        [SerializeField] private float damageReductionMultiplier = 1.0f;
        [SerializeField] private bool causesRecoil;

        public float DamageReductionMultiplier => damageReductionMultiplier;
        public bool CausesRecoil => causesRecoil;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoubTech.TPSCharacterController.Weapons
{
    [CreateAssetMenu(menuName = "TPS Character Controler/Weapon Rig Configuration")]
    public class WeaponRigConfiguration : ScriptableObject
    {
        [Header("Weapon")]
        [SerializeField] private GameObject prefab;
        [SerializeField] private AnimatorOverrideController weaponController;
        
        [Header("Equipped Placement")]
        [SerializeField] private HumanBodyBones boneAttachedToWhileEquipped = HumanBodyBones.RightHand;
        [SerializeField] private Vector3 positionOnBone;
        [SerializeField] private Vector3 rotationOnBone;

        [Header("Unequipped Placement")]
        [SerializeField] private bool visibleWhenUnequipped = true;
        [SerializeField] private HumanBodyBones boneAttachedToWhileUnequipped;
        [SerializeField] private Vector3 positionOnBoneUnequipped;
        [SerializeField] private Vector3 rotationOnBoneUnequipped;
        
        [Header("IK")]
        [SerializeField] private bool twoHanded;
    }
}

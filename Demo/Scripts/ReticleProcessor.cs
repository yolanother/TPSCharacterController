using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Inputs;
using UnityEngine;
using UnityEngine.UI;

public class ReticleProcessor : MonoBehaviour
{
    [Header("Input")]
    [SerializeField]
    private PlayerInput playerInput;
    
    [Header("UI")]
    [SerializeField] private RawImage reticle;
    [SerializeField] private Texture2D directionReticle;
    [SerializeField] private Texture2D centerReticle;

    private void Awake()
    {
        if (!playerInput) playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        playerInput.CombatDirection.OnValueChanged.AddListener(CombatDirectionChanged);
    }

    private void OnDisable()
    {
        playerInput.CombatDirection.OnValueChanged.RemoveListener(CombatDirectionChanged);
    }
    
    private void CombatDirectionChanged(Vector2 direction)
    {
        if (direction.magnitude > .001f)
        {
            reticle.texture = directionReticle;
            float angle = Mathf.Atan2(direction.y, direction.x) * 180 / Mathf.PI - 90;
            reticle.rectTransform.localEulerAngles = Vector3.forward * angle;
        }
    }
}

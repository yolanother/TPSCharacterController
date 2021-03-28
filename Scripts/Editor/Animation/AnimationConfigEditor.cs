using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace DoubTech.TPSCharacterController.Animation
{
    [CustomEditor(typeof(AnimationConfigPreset))]
    public class AnimationConfigEditor : Editor
    {
        private AnimationConfigPreset configPreset;
        private AnimationOverrideControllerSelector overrideController;
        private AnimationSlotSelector slotSelector;

        private void OnEnable()
        {
            configPreset = target as AnimationConfigPreset;
            overrideController = new AnimationOverrideControllerSelector();
            slotSelector = new AnimationSlotSelector();
        }

        public override void OnInspectorGUI()
        {
            overrideController.Draw();
            slotSelector.SelectedController = overrideController.SelectedController;
            if (null != slotSelector.SelectedSlot)
            {
                configPreset.config.animationSlot = slotSelector.SelectedSlot;
            }
            slotSelector.Draw();
            base.OnInspectorGUI();
        }
    }
}

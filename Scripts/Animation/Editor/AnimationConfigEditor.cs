using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace DoubTech.TPSCharacterController.Animation
{
    [CustomEditor(typeof(AnimationConfig))]
    public class AnimationConfigEditor : Editor
    {
        private AnimationConfig config;
        private AnimationOverrideControllerSelector overrideController;
        private AnimationSlotSelector slotSelector;

        private void OnEnable()
        {
            config = target as AnimationConfig;
            overrideController = new AnimationOverrideControllerSelector();
            slotSelector = new AnimationSlotSelector();
        }

        public override void OnInspectorGUI()
        {
            overrideController.Draw();
            slotSelector.SelectedController = overrideController.SelectedController;
            if (null != slotSelector.SelectedSlot)
            {
                config.animationSlot = slotSelector.SelectedSlot;
            }
            slotSelector.Draw();
            base.OnInspectorGUI();
        }
    }
}

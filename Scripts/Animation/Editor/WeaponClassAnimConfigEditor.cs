using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace DoubTech.TPSCharacterController.Animation
{
    [CustomEditor(typeof(WeaponClassAnimConfig))]
    public class WeaponClassAnimConfigEditor : Editor
    {
        private ActionSetEditor actionSetEditor;
        private WeaponClassAnimConfig config;
        private Vector2 scrollPosition;

        private void OnEnable()
        {
            config = target as WeaponClassAnimConfig;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (null != config.weaponClassController)
            {
                if (actionSetEditor == null || actionSetEditor.controller != config.weaponClassController)
                {
                    actionSetEditor = new ActionSetEditor(config.weaponClassController, config.overrides);
                }

                scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                actionSetEditor.Draw();

                GUILayout.EndScrollView();
            }
        }
    }
}

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

        public WeaponClassAnimConfig Config
        {
            get => config;
            set
            {
                if (value != config)
                {
                    config = value;
                    actionSetEditor = new ActionSetEditor(config);
                }
            }
        }
        
        public bool DisableScroll { get; set; }

        private void OnEnable()
        {
            if (target)
            {
                Config = target as WeaponClassAnimConfig;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (config.weaponClassController)
            {
                if(!DisableScroll) scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                actionSetEditor.Draw();
                
                GUILayout.BeginVertical(GUILayout.Height(30));
                DrawAnimationDropbox();
                GUILayout.EndVertical();

                foreach (var overridesKey in config.overrides.keys)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(overridesKey);
                    GUILayout.ExpandWidth(true);
                    if (UIUtil.ButtonIconBordered("TreeEditor.Trash"))
                    {
                        config.overrides.Remove(overridesKey);
                        EditorUtility.SetDirty(config);
                        Repaint();
                    }
                    GUILayout.EndHorizontal();
                }

                if(!DisableScroll) GUILayout.EndScrollView();
            }
        }
        
        void DrawAnimationDropbox()
        {
            Rect myRect = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUI.Box(myRect, "Active Overrides\n(Drop Animation Config here to add override)", UIUtil.BoxStyle);
            if (myRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.DragUpdated)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    Event.current.Use();
                }
                else if (Event.current.type == EventType.DragPerform)
                {
                    if (DragAndDrop.objectReferences.Length > 0)
                    {
                        object reference = DragAndDrop.objectReferences[0];
                        if (reference is AnimationConfigPreset)
                        {
                            AnimationConfigPreset configPresetRef = reference as AnimationConfigPreset;
                            config.overrides[configPresetRef.config.animationSlot] = new AnimationConfigOverride()
                                { preset = configPresetRef };
                            EditorUtility.SetDirty(config);
                        }
                    }

                    Event.current.Use();
                }
                else if (Event.current.type == EventType.MouseDown)
                {
                    Event.current.Use();
                }
            }
        }
    }
}

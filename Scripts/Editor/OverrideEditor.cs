using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DoubTech.TPSCharacterController.Utils;
using UnityEditor;

namespace DoubTech.TPSCharacterController
{
    public class OverrideEditor : EditorWindow
    {
        private AnimatorOverrideController currentController;
        private FoldoutHierarchy<KeyValuePair<AnimationClip, AnimationClip>> overrides;
        private Vector2 scroll;
        private GUIStyle boxStyle;

        public AnimatorOverrideController CurrentController
        {
            get => currentController;
            set
            {
                currentController = value;
                overrides = new FoldoutHierarchy<KeyValuePair<AnimationClip, AnimationClip>>(isFoldout: false);
                UpdateOverrides();
            }
        }

        private void UpdateOverrides()
        {
            string[] sep = {" - "};
            int idx = 0;
            var list = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            currentController.GetOverrides(list);
            foreach (var o in list)
            {
                var clip = o.Key;
                var path = clip.name.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                var c = new FoldoutHierarchy<KeyValuePair<AnimationClip, AnimationClip>>.Item(path[path.Length - 1], o);
                c.onDraw = DrawOverride;
                overrides.Add(c, path);
            }
        }

        [MenuItem("Tools/TPS Controller/Override Editor")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            OverrideEditor window = (OverrideEditor) EditorWindow.GetWindow(typeof(OverrideEditor));
            window.titleContent = new GUIContent("Override Editor");
            window.Show();
        }
        
        private void OnSelectionChange()
        {
            if (Selection.activeObject is AnimatorOverrideController)
            {
                CurrentController = Selection.activeObject as AnimatorOverrideController;
            }
        }

        private void OnGUI()
        {
            if (currentController)
            {
                var all = new List<KeyValuePair<AnimationClip, AnimationClip>>(); 
                currentController.GetOverrides(all);
                
                EditorGUILayout.LabelField("Override: " + currentController.name, EditorStyles.boldLabel);
                EditorGUILayout.LabelField(all.Count + " animations.");
                scroll = EditorGUILayout.BeginScrollView(scroll);
                if (null == overrides) CurrentController = currentController;

                overrides.Draw();

                GUILayout.Space(16);
                GUILayout.Label("Missing Animations");
                bool hasMissing = false;
                foreach (var o in all)
                {
                    if (o.Value == null)
                    {
                        hasMissing = true;
                        EditorGUILayout.LabelField(o.Key.name);
                        DrawClipField(o);
                    }
                }
                if(!hasMissing) GUILayout.Label("None.");
                EditorGUILayout.EndScrollView();
            }
            else
            {
                DropControllerGUI();
            }

            if (EditorUtility.IsDirty(currentController))
            {
                UpdateOverrides();
            }
        }

        private void DrawOverride(FoldoutHierarchy<KeyValuePair<AnimationClip, AnimationClip>>.Item overrideClip)
        {
            EditorGUILayout.LabelField(overrideClip.name);
            DrawClipField(overrideClip.data);
        }

        private void DrawClipField(KeyValuePair<AnimationClip,AnimationClip> overrideClip)
        {
            var clip = (AnimationClip) EditorGUILayout.ObjectField(overrideClip.Value, typeof(AnimationClip), false);
            if (clip != overrideClip.Value)
            {
                currentController[overrideClip.Key.name] = clip;
                EditorUtility.SetDirty(currentController);
                
            }
        }

        void DropControllerGUI()
        {
            if (null == boxStyle)
            {
                boxStyle = new GUIStyle(GUI.skin.box);
                boxStyle.alignment = TextAnchor.MiddleCenter;
                boxStyle.fontStyle = FontStyle.Italic;
                boxStyle.fontSize = 12;
                GUI.skin.box = boxStyle;
            }

            Rect myRect = GUILayoutUtility.GetRect(0, 0,GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUI.Box(myRect, "Drop an AnimatorOverrideController here to edit it.", boxStyle);
            if (myRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.DragUpdated)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    Debug.Log("Drag Updated!");
                    Event.current.Use ();
                }   
                else if (Event.current.type == EventType.DragPerform)
                {
                    if (DragAndDrop.objectReferences.Length > 0 &&
                        DragAndDrop.objectReferences[0] is AnimatorOverrideController)
                    {
                        CurrentController = (AnimatorOverrideController) DragAndDrop.objectReferences[0];
                    }

                    Event.current.Use ();
                }
            }
        }
    }
}

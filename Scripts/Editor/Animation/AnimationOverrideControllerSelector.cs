using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DoubTech.TPSCharacterController.Animation
{
    public class AnimationOverrideControllerSelector
    {
        private int selected;
        private string[] controllers;
        private string[] controllerNames;
        private string[] controllerPaths;

        private AnimatorOverrideController controllerInstance;
        public AnimatorOverrideController SelectedController
        {
            get
            {
                if (!controllerInstance)
                {
                    controllerInstance = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(controllerPaths[selected]);
                }

                return controllerInstance;
            }
            set
            {
                string path = AssetDatabase.GetAssetPath(value);
                for (int i = 0; i < controllerPaths.Length; i++)
                {
                    if (controllerPaths[i] == path)
                    {
                        selected = i;
                    }
                }
            }
        }

        public AnimationOverrideControllerSelector()
        {
            Refresh();
        }

        public void Refresh()
        {
            controllers = AssetDatabase.FindAssets("t:AnimatorOverrideController");
            controllerNames = new string[controllers.Length];
            controllerPaths = new string[controllers.Length];
            for (int i = 0; i < controllers.Length; i++)
            {
                var path = controllerPaths[i] = AssetDatabase.GUIDToAssetPath(controllers[i]);
                int startIndex = path.LastIndexOf("/") + 1; 
                controllerNames[i] = path.Substring(
                    startIndex, path.Length - startIndex - ".overrideController".Length);
            }
        }
        
        public void Draw()
        {
            var newlySelected = EditorGUILayout.Popup(selected, controllerNames);
            if (selected != newlySelected)
            {
                controllerInstance = null;
                selected = newlySelected;
            }
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DoubTech.TPSCharacterController.Animation
{
    public class AnimationSlotSelector
    {
        private int selected;
        private string[] slotNames = new string[0];

        private AnimatorOverrideController controllerInstance;
        public AnimatorOverrideController SelectedController
        {
            get => controllerInstance;
            set {
                if (controllerInstance != value)
                {
                    var list = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                    value.GetOverrides(list);
                    slotNames = new string[list.Count + 1];
                    for (int i = 0; i < list.Count; i++)
                    {
                        slotNames[i + 1] = list[i].Key.name;
                    }

                    controllerInstance = value;
                }
            }
        }

        public string SelectedSlot => null != slotNames && slotNames.Length > 1 ? slotNames[selected] : null;
        
        public void Draw()
        {
            var newlySelected = EditorGUILayout.Popup(selected, slotNames);
            if (selected != newlySelected)
            {
                selected = newlySelected;
            }
        }
    }
}

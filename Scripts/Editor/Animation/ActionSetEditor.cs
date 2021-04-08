using System;
using UnityEngine;
using DoubTech.TPSCharacterController.Scripts.Animation;
using UnityEditor;

namespace DoubTech.TPSCharacterController.Animation
{
    public class ActionSetEditor
    {
        private string[] actionSetNames = new[] {"Primary Attacks", "Secondary Attacks", "Blocks", "Hits"};
        private int actionSetIndex;
        private WeaponClassAnimConfig config;
        private AnimationConfigOverride selectedSlot;

        public System.Action<AnimationConfigOverride> onSelectedSlot;
        public System.Action<AnimationConfigOverride> onUnselectedSlot;

        public ActionSetEditor(WeaponClassAnimConfig config)
        {
            this.config = config;
        }

        private void DrawWeaponSlot(AnimationConfigOverride slot, int width, int height)
        {
            GUILayout.BeginVertical(UIUtil.BackgroundColor(0.3f, 0.3f, 0.3f), GUILayout.Width(width),
                GUILayout.Height(height));
            DrawAnimationDropbox(slot);
            GUILayout.EndVertical();
        }

        private void DrawRow(AnimationConfigOverride[] slots, int start, int count, int size)
        {
            GUILayout.BeginHorizontal();
            GUILayout.ExpandWidth(true);
            for (int i = 0; i < count; i++)
            {
                DrawWeaponSlot(slots[start + i], size, size);
                if(i + 1 < slots.Length) GUILayout.Space(8);
            }

            GUILayout.ExpandWidth(true);
            GUILayout.EndHorizontal();
        }

        public void Draw(float width = -1)
        {
            if (width < 0) width = EditorGUIUtility.currentViewWidth;
            GUILayout.Space(8);
            int size = 84;
            GUILayout.BeginHorizontal();
            GUILayout.Space((width - 3 * size) / 2.0f - 32);
            GUILayout.ExpandWidth(true);
            GUILayout.BeginVertical();
            actionSetIndex = EditorGUILayout.Popup(actionSetIndex, actionSetNames, GUILayout.Width(3 * size + 10));
            AnimationConfigOverride[] slots = null;
            switch (actionSetIndex)
            {
                case 0:
                    slots = config.primaryAttacks;
                    break;
                case 1:
                    slots = config.secondaryAttacks;
                    break;
                case 2:
                    slots = config.blocks;
                    break;
                case 3:
                    slots = config.hits;
                    break;
            }

            if (null != slots)
            {
                DrawRow(slots, 6, 3, size);
                GUILayout.Space(8);
                DrawRow(slots, 0, 3, size);
                GUILayout.Space(8);
                DrawRow(slots, 3, 3, size);
            }

            GUILayout.EndVertical();
            GUILayout.ExpandWidth(true);
            GUILayout.EndHorizontal();
            GUILayout.Space(8);

            if (DrawAnimaitonConfigEditor(selectedSlot))
            {
                EditorUtility.SetDirty(config);
            }
            GUILayout.Space(16);
        }

        private static bool Slider(string label, ref float value, float leftValue, float rightValue)
        {
            var changed = EditorGUILayout.Slider(
                label,
                value,
                0f, 1f);

            if (Math.Abs(changed - value) > .001f)
            {
                value = changed;
                return true;
            }

            return false;
        }
        
        public static bool DrawAnimaitonConfigEditor(AnimationConfigOverride selectedSlot)
        {
            bool isDirty = false;
            if (null != selectedSlot)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                if (selectedSlot.preset)
                {
                    GUILayout.Label("Using Preset: " + selectedSlot.preset.name);
                }

                selectedSlot.Config.name = EditorGUILayout.TextField("Name", selectedSlot.Config.name);
                selectedSlot.Config.animation = (AnimationClip) EditorGUILayout.ObjectField("Animation", selectedSlot.Config.animation,
                    typeof(AnimationClip), true);
                
                GUILayout.Label("Weights");
                isDirty |= Slider(
                    "Full Body",
                    ref selectedSlot.Config.fullBody.layerWeight,
                    0f, 1f);
     
                isDirty |= Slider(
                    "Upper Body",
                    ref selectedSlot.Config.upperBody.layerWeight,
                    0f, 1f);
                
                isDirty |= Slider(
                    "Lower Body",
                    ref selectedSlot.Config.lowerBody.layerWeight,
                    0f, 1f);
                
                isDirty |= Slider(
                    "Speed",
                    ref selectedSlot.Config.speed,
                    .1f, 2f);
                
                var value = EditorGUILayout.Toggle(
                    "Mirror",
                    selectedSlot.Config.mirror);
                
                if (value != selectedSlot.Config.mirror)
                {
                    selectedSlot.Config.mirror = value;
                    isDirty = true;
                }
                EditorGUILayout.EndVertical();
            }

            
            return isDirty;
        }

        bool DrawAnimationDropbox(AnimationConfigOverride slot)
        {
            bool hasSlot = false;

            Rect myRect = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUIContent content = new GUIContent(slot.Config.animation ? slot.Config.animation.name : "Unassigned");
            

            GUI.Box(myRect, content, UIUtil.BoxStyle);
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
                        if (reference is AnimationClip)
                        {
                            slot.preset = null;
                            slot.Config.animation = reference as AnimationClip;
                            EditorUtility.SetDirty(config);
                        }
                        else if (reference is AnimationConfigPreset)
                        {
                            slot.preset = reference as AnimationConfigPreset;
                            EditorUtility.SetDirty(config);
                        }
                        else if (reference is AudioClip)
                        {
                            var audio = reference as AudioClip;
                            bool hasFile = false;
                            var tags = slot.Config.soundTags;
                            AnimationSoundTag[] newTags = new AnimationSoundTag[tags.Length + 1];
                            for (int i = 0; i < tags.Length; i++)
                            {
                                if (tags[i].sound.name == audio.name) hasFile = true;
                                newTags[i] = tags[i];
                            }

                            if (!hasFile)
                            {
                                newTags[tags.Length] = new AnimationSoundTag()
                                {
                                    sound = audio
                                };
                                slot.Config.soundTags = newTags;
                                EditorUtility.SetDirty(config);
                            }
                        }
                        
                    }

                    Event.current.Use();
                }
                else if (Event.current.type == EventType.MouseDown)
                {
                    Event.current.Use();
                    
                    if (slot == selectedSlot) selectedSlot = null;
                    else
                    {
                        if (null != selectedSlot)
                        {
                            onUnselectedSlot?.Invoke(slot);
                        }
                        selectedSlot = slot;
                        onSelectedSlot?.Invoke(slot);
                    }
                    
                    return true;
                }
            }

            return false;
        }
    }
}

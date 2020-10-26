using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Animation.Slots;
using UnityEditor;

namespace DoubTech.TPSCharacterController.Animation
{
    public class ActionSetEditor
    {
        private string[] actionSetNames = new[] {"Strong Attacks", "Weak Attacks", "Blocks"};
        private ActionSlotDefinition[][] actionSlots = new ActionSlotDefinition[][]
        {
            AnimSlotDefinitions.ATTACK_STRONG_SLOTS,
            AnimSlotDefinitions.ATTACK_WEAK_SLOTS,
            AnimSlotDefinitions.BLOCK_SLOTS
        };
        private int actionSetIndex;
        private WeaponClassAnimConfig config;

        public ActionSetEditor(WeaponClassAnimConfig config)
        {
            this.config = config;
        }

        private void DrawWeaponSlot(ActionSlotDefinition slot, int width, int height)
        {
            GUILayout.BeginVertical(UIUtil.BackgroundColor(0.3f, 0.3f, 0.3f), GUILayout.Width(width),
                GUILayout.Height(height));
            DrawAnimationDropbox(slot);
            GUILayout.EndVertical();
        }

        private void DrawRow(ActionSlotDefinition[] slots, int start, int count, int size)
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

        public void Draw()
        {
            GUILayout.Space(8);
            int size = 84;
            GUILayout.BeginHorizontal();
            GUILayout.Space((EditorGUIUtility.currentViewWidth - 3 * size) / 2.0f - 32);
            GUILayout.ExpandWidth(true);
            GUILayout.BeginVertical();
            actionSetIndex = EditorGUILayout.Popup(actionSetIndex, actionSetNames, GUILayout.Width(3 * size + 10));
            DrawRow(actionSlots[actionSetIndex], 0, 3, size);
            GUILayout.Space(8);
            DrawRow(actionSlots[actionSetIndex], 3, 3, size);
            GUILayout.Space(8);
            DrawRow(actionSlots[actionSetIndex], 6, 3, size);
            GUILayout.EndVertical();
            GUILayout.ExpandWidth(true);
            GUILayout.EndHorizontal();
            GUILayout.Space(8);
        }

        void DrawAnimationDropbox(ActionSlotDefinition slot)
        {
            bool hasSlot = false;
            string animName = "No animation";
            if (config.weaponClassController[slot.slotName] && config.weaponClassController[slot.slotName].length > 1)
            {
                hasSlot = true;
                animName = config.weaponClassController[slot.slotName].name;
            }

            if (config.overrides.ContainsKey(slot.slotName) && config.overrides[slot.slotName].config.animation)
            {
                hasSlot = true;
                var c = config.overrides[slot.slotName].config;
                animName = c.name + " - " + c.animation.name;
            }

            Rect myRect = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUIContent content;
            if (hasSlot)
            {
                content = new GUIContent(slot.positionName.Replace(" ", "\n"), animName);
            }
            else
            {
                content = new GUIContent(slot.positionName.Replace(" ", "\n") + "\n(Unassigned)", animName);
            }

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
                            var clip = reference as AnimationClip;
                            if (config.overrides.ContainsKey(slot.slotName))
                            {
                                config.overrides.Remove(slot.slotName);
                                Debug.Log("Assigned override " + clip.name + " to " + slot.slotName);
                            }
                            
                            config.weaponClassController[slot.slotName] = reference as AnimationClip;
                            Debug.Log("Assigned " + clip.name + " to " + slot.slotName);

                            EditorUtility.SetDirty(config);
                        }
                        else if (reference is AnimationConfig)
                        {
                            var refConfig = reference as AnimationConfig;
                            config.overrides[slot.slotName] = new AnimationConfigOverride()
                                { slot = slot.slotName, config = refConfig};
                            Debug.Log("Setting " + slot.slotName + " to " + refConfig.name);
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

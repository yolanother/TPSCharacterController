using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController.Inventory.Weapons;
using UnityEditor;

namespace DoubTech.TPSCharacterController.Utilities
{
    [CustomPropertyDrawer(typeof(SerializedClassField))]
    public class SerializedClassFieldDrawer : PropertyDrawer
    {   
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = 0;

            var e = property.GetEnumerator();
            while (e.MoveNext())
            {
                var prop = e.Current as SerializedProperty;
                height += EditorGUI.GetPropertyHeight(prop);
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var currentRect = new Rect(position);
            var e = property.GetEnumerator();
            while (e.MoveNext())
            {
                var prop = e.Current as SerializedProperty;
                currentRect.height = EditorGUI.GetPropertyHeight(prop);
                EditorGUI.PropertyField(currentRect, prop);
                currentRect.y += EditorGUI.GetPropertyHeight(prop);
            }
            
            EditorGUI.EndProperty();
        }
    }
}

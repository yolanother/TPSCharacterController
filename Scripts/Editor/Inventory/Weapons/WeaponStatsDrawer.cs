using UnityEngine;
using UnityEditor;

namespace DoubTech.TPSCharacterController.Inventory.Weapons
{
    [CustomPropertyDrawer( typeof( WeaponStats ) )]
    public class WeaponStatsDrawer : PropertyDrawer
    {
        private SerializedProperty preset;

        private SerializedProperty PresetProperty(SerializedProperty property)
        {
            if (null == preset)
            {
                preset = property.FindPropertyRelative("preset");
            }

            return preset;
        }
        
        private SerializedProperty stats;
        private float statsHeight;
        private float presetHeight;

        private SerializedProperty StatsProperty(SerializedProperty property)
        {
            if (null == preset)
            {
                stats = property.FindPropertyRelative("data");
            }

            return stats;
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = presetHeight = EditorGUIUtility.singleLineHeight;

            if (PresetProperty(property).objectReferenceValue)
            {
                height += EditorGUIUtility.singleLineHeight;
            }
            
            var e = property.FindPropertyRelative("data").GetEnumerator();
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
            var presetRect = new Rect( position.x, position.y, position.width, EditorGUIUtility.singleLineHeight );

            EditorGUI.PropertyField(presetRect, PresetProperty(property));
            
            var statsRect = new Rect(presetRect);
            statsRect.y += presetRect.height;
            if (PresetProperty(property).objectReferenceValue)
            {
                EditorGUI.LabelField(statsRect, "Using preset for data. Any changes here will update the preset.", EditorStyles.miniLabel);
                statsRect.y += EditorGUIUtility.singleLineHeight;

                var value = PresetProperty(property).objectReferenceValue;
                SerializedObject obj = new SerializedObject(value);
                SerializedProperty dataProp = obj.GetIterator();
                while (dataProp.NextVisible(true))
                {
                    if (dataProp.name == "data")
                    {
                        EditorGUI.BeginProperty(statsRect, GUIContent.none, dataProp);
                        EditorGUI.PropertyField(statsRect, dataProp);
                        obj.ApplyModifiedProperties();
                        EditorGUI.EndProperty();
                        break;
                    }
                }
            }
            else
            {
                EditorGUI.PropertyField(statsRect, property.FindPropertyRelative("data"));
            }
            
            EditorGUI.EndProperty();
        }
    }
}

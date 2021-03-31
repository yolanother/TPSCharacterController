using UnityEngine;
using UnityEditor;

namespace DoubTech.TPSCharacterController.Utilities
{
    public class PresetDrawer : PropertyDrawer
    {
        private SerializedProperty preset;

        private SerializedProperty PresetProperty(SerializedProperty property)
        {
            if (null == preset)
            {
                preset = property.FindPropertyRelative("preset");
                if (null == preset)
                {
                    Debug.LogError(property.name + " must have a field named preset to be used with a PresetPropertyDrawer.");
                }
            }

            return preset;
        }
        
        private SerializedProperty stats;
        private float statsHeight;
        private float presetHeight;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUIUtility.singleLineHeight;

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
            Debug.Log("Drawing preset.");
            EditorGUI.BeginProperty(position, label, property);
            var presetRect = new Rect( position.x, position.y, position.width, EditorGUIUtility.singleLineHeight );

            EditorGUI.PropertyField(presetRect, PresetProperty(property));
            
            var statsRect = new Rect(presetRect);
            statsRect.y += presetRect.height;
            if (PresetProperty(property).objectReferenceValue)
            {
                EditorGUI.LabelField(statsRect, "Using preset for data. Any changes here will update the preset.", EditorStyles.miniLabel);
                statsRect.y += EditorGUIUtility.singleLineHeight;

                bool dataFound = false;
                var value = PresetProperty(property).objectReferenceValue;
                SerializedObject obj = new SerializedObject(value);
                SerializedProperty dataProp = obj.GetIterator();
                while (dataProp.NextVisible(true))
                {
                    if (dataProp.name == "data")
                    {
                        dataFound = true;
                        EditorGUI.BeginProperty(statsRect, GUIContent.none, dataProp);
                        EditorGUI.PropertyField(statsRect, dataProp);
                        obj.ApplyModifiedProperties();
                        EditorGUI.EndProperty();
                        break;
                    }
                }

                if (!dataFound)
                {
                    Debug.LogError("Your preset's scriptable object must have a data field defined.");
                }
            }
            else
            {
                var data = property.FindPropertyRelative("data");
                if (null != data)
                {
                    EditorGUI.PropertyField(statsRect, data);
                }
                else
                {
                    Debug.LogError(property.name + " must have a data field defined.");
                }
            }
            
            EditorGUI.EndProperty();
        }
    }
}

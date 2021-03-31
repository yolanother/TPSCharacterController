using UnityEngine;
using UnityEditor;

namespace DoubTech.TPSCharacterController.Inventory.Weapons
{
    [CustomPropertyDrawer( typeof( WeaponStatsData ) )]
    public class WeaponStatsDataDrawer : PropertyDrawer
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

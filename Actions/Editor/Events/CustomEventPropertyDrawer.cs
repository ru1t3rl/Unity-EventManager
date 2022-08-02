using UnityEngine;
using UnityEditor;
using Ru1t3rl.Events;

namespace Ru1t3rl.Editors.Events
{
    [CustomPropertyDrawer(typeof(CustomEvent))]
    public class CustomEventPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty actionFields = property.FindPropertyRelative("actionFields");
            EditorGUI.PropertyField(position, actionFields, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("actionFields"), label, true);
        }
    }
}
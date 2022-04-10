using Knot.Localization.Attributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Knot.Localization.Editor
{
    [CustomPropertyDrawer(typeof(KnotNotNullStringAttribute))]
    public class KnotNotNullStringDrawer : PropertyDrawer
    {
        void DoFallback(SerializedProperty property, string previousValue)
        {
            if (!string.IsNullOrEmpty(previousValue))
                property.stringValue = previousValue;
            else
            {
                KnotNotNullStringAttribute attr = this.attribute as KnotNotNullStringAttribute;
                property.stringValue = attr == null ? "Empty" : attr.FallbackString;
            }

            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }


        public override VisualElement CreatePropertyGUI(SerializedProperty property)
            => property.GetFallbackPropertyGUI();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property, label, property.hasChildren);
                return;
            }

            string stringValue = property.stringValue;
            if (string.IsNullOrEmpty(stringValue))
                DoFallback(property, stringValue);

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label, property.hasChildren);
            if (EditorGUI.EndChangeCheck() && string.IsNullOrEmpty(property.stringValue))
                DoFallback(property, stringValue);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, property.isExpanded);
        }
    }
}
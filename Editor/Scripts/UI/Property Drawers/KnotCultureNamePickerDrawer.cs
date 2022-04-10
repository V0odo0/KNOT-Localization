using System.Globalization;
using Knot.Localization.Attributes;
using Knot.Localization.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Knot.Localization.Editor
{
    [CustomPropertyDrawer(typeof(KnotCultureNamePickerAttribute), true)]
    public class KnotCultureNamePickerDrawer : PropertyDrawer
    {
        void ShowPicker(Rect atRect, SerializedProperty property)
        {
            KnotCulturePickerPopup.Show(atRect, cultureName =>
            {
                property.stringValue = cultureName;
                property.serializedObject.ApplyModifiedProperties();
            }, property.stringValue);
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
            => property.GetFallbackPropertyGUI();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property, label, property.isExpanded);
                return;
            }

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            if (property.propertyType != SerializedPropertyType.String)
                EditorGUI.LabelField(position, "Invalid type");
            else
            {
                if (EditorGUI.DropdownButton(position, new GUIContent(CultureInfo.GetCultureInfo(property.stringValue).GetDisplayName()), FocusType.Keyboard))
                    ShowPicker(position, property);
            }
        }
    }
}
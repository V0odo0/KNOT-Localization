using System;
using System.Linq;
using Knot.Localization.Attributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Knot.Localization.Editor
{
    [CustomPropertyDrawer(typeof(KnotTypePickerAttribute))]
    public class KnotTypePickerDrawer : PropertyDrawer
    {
        public Type BaseType => (attribute as KnotTypePickerAttribute)?.BaseType;


        bool IsValidProperty(SerializedProperty property)
        {
            return BaseType != null && 
                   property.propertyType == SerializedPropertyType.ManagedReference &&
                   BaseType.GetDerivedTypesInfo().Any();
        }


        public override VisualElement CreatePropertyGUI(SerializedProperty property)
            => property.GetFallbackPropertyGUI();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!IsValidProperty(property))
            {
                base.OnGUI(position, property, label);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            Type currentType = property.GetManagedReferenceType();
            var types = BaseType.GetDerivedTypesInfo();

            Rect popupPos = position;
            popupPos.height = EditorGUIUtility.singleLineHeight;
            
            EditorGUI.BeginChangeCheck();

            int selectedTypeInfoId = types.Select((info, i) => new {typeInfo = info, Index = i})
                .FirstOrDefault(t => t.typeInfo.Type == currentType)?.Index ?? -1;
            selectedTypeInfoId = EditorGUI.Popup(popupPos, label, selectedTypeInfoId, types.Select(ti => ti.Content).ToArray());

            if (EditorGUI.EndChangeCheck() && types[selectedTypeInfoId].Type != property.GetManagedReferenceType())
            {
                property.managedReferenceValue = types[selectedTypeInfoId].GetInstance();
                property.serializedObject.ApplyModifiedProperties();
            }

            position.y += popupPos.height + EditorGUIUtility.standardVerticalSpacing;
            position.height -= popupPos.height;

            if (property.hasVisibleChildren)
            {
                EditorGUI.indentLevel++;
                property.DrawChildProperties(position);

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!IsValidProperty(property))
                return base.GetPropertyHeight(property, label);

            return EditorGUIUtility.singleLineHeight + property.GetChildPropertiesHeight();
        }
    }
}

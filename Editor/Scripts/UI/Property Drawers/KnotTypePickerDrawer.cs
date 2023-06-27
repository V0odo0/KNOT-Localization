using System;
using System.Collections.Generic;
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
        public KnotTypePickerAttribute Attribute => attribute as KnotTypePickerAttribute;
        public Type BaseType => Attribute?.BaseType;


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

            Rect popupPos = new Rect(position.x + EditorGUIUtility.labelWidth, position.y,
                position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

            Type selectedType = property.GetManagedReferenceType();

            if (selectedType == null)
                EditorGUI.PrefixLabel(position, label);

            bool isDropdownClicked;
            if (selectedType == null || Attribute.BaseType == null)
                isDropdownClicked = EditorGUI.DropdownButton(popupPos, EditorGUIUtility.TrTextContent("[Select Type]"), FocusType.Keyboard);
            else
            {
                var typeInfo = Attribute.BaseType.GetDerivedTypesInfo().FirstOrDefault(t => t.Type == selectedType);
                isDropdownClicked = EditorGUI.DropdownButton(popupPos, typeInfo == null ?
                    EditorGUIUtility.TrTextContent(selectedType.Name) :
                    typeInfo.Content, FocusType.Keyboard);
            }

            if (isDropdownClicked)
            {
                var types = BaseType.GetDerivedTypesInfo();
                GenericMenu menu = new GenericMenu();

                HashSet<Type> typeConstraints = null;
                if (!Attribute.AllowSameTypeInArray)
                {
                    SerializedProperty parentProperty = property.FindParentProperty();
                    if (parentProperty != null && parentProperty.isArray)
                    {
                        typeConstraints = new HashSet<Type>();
                        for (int i = 0; i < parentProperty.arraySize; i++)
                        {
                            typeConstraints.Add(parentProperty.GetArrayElementAtIndex(i).GetManagedReferenceType());
                        }
                    }
                }

                foreach (var t in types)
                {
                    bool canSelect = typeConstraints == null || !typeConstraints.Contains(t.Type);
                    bool isSelected = t.Type == selectedType;
                    if (canSelect)
                    {
                        menu.AddItem(EditorGUIUtility.TrTextContent(t.Info.MenuName), isSelected, () =>
                        {
                            try
                            {
                                property.managedReferenceValue = t.GetInstance();
                                property.serializedObject.ApplyModifiedProperties();
                                selectedType = property.GetManagedReferenceType();
                            }
                            catch
                            {
                                //
                            }
                        });
                    }
                    else menu.AddDisabledItem(EditorGUIUtility.TrTextContent(t.Info.MenuName), isSelected);
                }

                menu.DropDown(popupPos);
            }

            if (selectedType != null)
                EditorGUI.PropertyField(position, property, label, true);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!IsValidProperty(property))
                return base.GetPropertyHeight(property, label);

            return EditorGUI.GetPropertyHeight(property, true) + 3;
        }
    }
}

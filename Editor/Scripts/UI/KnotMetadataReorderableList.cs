using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Knot.Localization.Attributes;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Knot.Localization.Editor
{
    public class KnotMetadataReorderableList : KnotReorderableList
    {
        public event Action<int> SelectionChanged;

        public readonly KnotMetadataInfoAttribute.MetadataScope Scope;
        public readonly bool IsEditorOnly;

        private List<KnotEditorExtensions.TypeInfo> _availableMetadataTypes = new List<KnotEditorExtensions.TypeInfo>();
        private SerializedProperty _lastSerializedProperty;


        public KnotMetadataReorderableList(KnotMetadataInfoAttribute.MetadataScope scope, bool isEditorOnly = false, SerializedProperty metadataListProperty = null) :
            base(metadataListProperty?.serializedObject, metadataListProperty, true, true, true, true)
        {
            Scope = scope;
            IsEditorOnly = isEditorOnly;

            drawHeaderCallback = DrawHeaderCallback;
            drawElementCallback = DrawElement;
            elementHeightCallback = GetElementHeight;
            onCanAddCallback = OnCanAddCallback;
            onAddDropdownCallback = AddDropdown;
            onSelectCallback = reorderableList =>
            {
                SelectionChanged?.Invoke(index);
            };
            onRemoveCallback = reorderableList =>
            {
                serializedProperty.DeleteArrayElementAtIndex(index);
                if (index > 0)
                    index--;
                serializedProperty.serializedObject.ApplyModifiedProperties();
                UpdateAvailableMetadataTypes();
            };

            UpdateAvailableMetadataTypes();
        }


        void DrawHeaderCallback(Rect rect) => EditorGUI.LabelField(rect, IsEditorOnly ? "Editor-only" : "Runtime");

        void DrawElement(Rect rect, int elementIndex, bool isactive, bool isfocused)
        {
            rect.y += EditorGUIUtility.standardVerticalSpacing;
            var property = serializedProperty.GetArrayElementAtIndex(elementIndex);

            EditorGUI.PropertyField(rect, property, new GUIContent(property.GetManagedReferenceTypeName()), property.isExpanded);
        }

        float GetElementHeight(int elementIndex) =>
            EditorGUI.GetPropertyHeight(serializedProperty.GetArrayElementAtIndex(elementIndex)) + EditorGUIUtility.standardVerticalSpacing * 2;


        void UpdateAvailableMetadataTypes()
        {
            _availableMetadataTypes.Clear();
            foreach (var metadataType in KnotEditorUtils.MetadataTypes[Scope])
            {
                var metadataInfo = metadataType.Type.GetCustomAttribute<KnotMetadataInfoAttribute>();
                if (metadataInfo != null)
                {
                    if (metadataInfo.IsEditorOnly && !IsEditorOnly)
                        continue;

                    if (!metadataInfo.AllowMultipleInstances && serializedProperty.HasInstanceOfTypeInArray(metadataType.Type))
                        continue;
                }

                _availableMetadataTypes.Add(metadataType);
            }
        }

        bool OnCanAddCallback(ReorderableList reorderableList) => _availableMetadataTypes.Any();

        void AddDropdown(Rect rect, ReorderableList elementsList)
        {
            GenericMenu menu = new GenericMenu();
            foreach (var metadataType in _availableMetadataTypes)
            {
                menu.AddItem(metadataType.Content, false, () =>
                {
                    var instance = metadataType.GetInstance();
                    if (instance == null)
                        return;

                    serializedProperty.InsertArrayElementAtIndex(serializedProperty.arraySize);
                    serializedProperty.GetArrayElementAtIndex(serializedProperty.arraySize - 1).managedReferenceValue =
                        instance;

                    index = serializedProperty.arraySize - 1;
                    serializedProperty.serializedObject.ApplyModifiedProperties();

                    UpdateAvailableMetadataTypes();
                });
            }

            menu.DropDown(rect);
        }

        
        public new void DoLayoutList()
        {
            if (serializedProperty == null || serializedProperty.serializedObject.targetObject == null)
                return;

            if (_lastSerializedProperty != serializedProperty)
            {
                _lastSerializedProperty = serializedProperty;
                UpdateAvailableMetadataTypes();
            }

            serializedProperty.serializedObject.UpdateIfRequiredOrScript();
            base.DoLayoutList();
            serializedProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}


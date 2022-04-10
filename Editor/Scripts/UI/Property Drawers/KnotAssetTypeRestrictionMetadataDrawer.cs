using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Knot.Localization.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Knot.Localization.Editor
{
    [CustomPropertyDrawer(typeof(KnotAssetTypeRestrictionMetadata))]
    public class KnotAssetTypeRestrictionMetadataDrawer : PropertyDrawer
    {
        protected GUIContent DropdownButtonContent => _dropdownButtonContent ?? (_dropdownButtonContent = new GUIContent());
        private GUIContent _dropdownButtonContent;

        protected Dictionary<SerializedObject, Type> ModifiedTypeObjects => _modifiedTypeObjects ?? (_modifiedTypeObjects = new Dictionary<SerializedObject, Type>());
        private Dictionary<SerializedObject, Type> _modifiedTypeObjects = new Dictionary<SerializedObject, Type>();


        protected virtual void HandleKeyDragAndDrop(Rect dropRect, SerializedProperty typeNameProp)
        {
            if (!dropRect.Contains(Event.current.mousePosition))
                return;

            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    var draggedObjType = DragAndDrop.objectReferences?.FirstOrDefault()?.GetType();
                    if (draggedObjType != null && KnotAssetTypePickerPopup.AllAssetTypes.Contains(draggedObjType))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                        if (Event.current.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();

                            SetType(typeNameProp, draggedObjType);

                            Event.current.Use();
                        }
                    }
                    break;
            }
        }

        string GetAssetTypeName(string assemblyQualifiedTypeName)
        {
            if (string.IsNullOrEmpty(assemblyQualifiedTypeName))
                return "Object";

            var type = Type.GetType(assemblyQualifiedTypeName);
            if (type == null || !type.IsSubclassOf(typeof(UnityEngine.Object)))
                return "Object";

            return type.Name;
        }

        void SetType(SerializedProperty typeNameProp, Type type)
        {
            //Since PopupWindow does not block the main UI update and continues drawing all visible properties with single PropertyDrawer instance,
            //we should remember the changes for current property and apply them later to get EditorGUI.Changed working properly
            typeNameProp.stringValue = type.AssemblyQualifiedName;
            typeNameProp.serializedObject.ApplyModifiedProperties();
            
            if (!ModifiedTypeObjects.ContainsKey(typeNameProp.serializedObject))
                ModifiedTypeObjects.Add(typeNameProp.serializedObject, type);
            else ModifiedTypeObjects[typeNameProp.serializedObject] = type;
        }


        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            IMGUIContainer container = new IMGUIContainer(() => EditorGUILayout.PropertyField(property));

            return container;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var typeNameProp = property.FindPropertyRelative("_assemblyQualifiedTypeName");

            Rect typePropertyPos = position;
            typePropertyPos.height = EditorGUIUtility.singleLineHeight;
            typePropertyPos = EditorGUI.PrefixLabel(typePropertyPos, label);

            HandleKeyDragAndDrop(typePropertyPos, typeNameProp);

            DropdownButtonContent.text = GetAssetTypeName(typeNameProp.stringValue);

            if (EditorGUI.DropdownButton(typePropertyPos, DropdownButtonContent, FocusType.Keyboard))
                KnotAssetTypePickerPopup.Show(position, type => SetType(typeNameProp, type));

            if (ModifiedTypeObjects.ContainsKey(property.serializedObject))
            {
                GUI.changed = true;
                ModifiedTypeObjects.Remove(property.serializedObject);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
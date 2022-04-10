using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Knot.Localization.Editor
{
    public abstract class KnotKeyReferenceDrawer<TValueType> : PropertyDrawer
    {
        protected GUIContent DropdownButtonContent => _dropdownButtonContent ?? (_dropdownButtonContent = new GUIContent());
        private GUIContent _dropdownButtonContent;

        protected virtual Texture GetIcon(SerializedProperty parentProperty, SerializedProperty keyProperty)
        {
            return null;
        }

        
        protected abstract string GetKeyDragAndDropDataName();

        protected abstract void ShowKeyPicker(Rect atRect, SerializedProperty parentProperty, Action<string> keyPicked);


        protected virtual void SetKey(SerializedProperty parentProperty, SerializedProperty keyProperty, string key)
        {
            keyProperty.stringValue = key;
            keyProperty.serializedObject.ApplyModifiedProperties();

            if (Application.IsPlaying(parentProperty.serializedObject.targetObject))
                if (parentProperty.serializedObject.targetObject is MonoBehaviour go)
                    go.SendMessage("ForceUpdateValue", SendMessageOptions.DontRequireReceiver);
        }

        protected virtual SerializedProperty GetKeyProperty(SerializedProperty parentProperty)
        {
            if (parentProperty.propertyType == SerializedPropertyType.String)
                return parentProperty;

            var keyProperty = parentProperty.FindPropertyRelative("_key");
            if (keyProperty != null && keyProperty.propertyType == SerializedPropertyType.String)
                return keyProperty;

            return null;
        }

        protected virtual void HandleKeyDragAndDrop(Rect dropRect, SerializedProperty parentProperty, SerializedProperty keyProperty)
        {
            if (!dropRect.Contains(Event.current.mousePosition))
                return;

            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (DragAndDrop.GetGenericData(GetKeyDragAndDropDataName()) is string[] keys && keys.Length != 0)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                        if (Event.current.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();
                            SetKey(parentProperty, keyProperty, keys.FirstOrDefault());

                            Event.current.Use();
                        }
                    }
                    break;
            }
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
            => property.GetFallbackPropertyGUI();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var keyProperty = GetKeyProperty(property);

            if (keyProperty == null)
            {
                base.OnGUI(position, property, label);
                return;
            }

            EditorGUI.BeginProperty(position, label, keyProperty);
            
            Rect keyPropertyPos = position;
            keyPropertyPos.height = EditorGUIUtility.singleLineHeight;
            keyPropertyPos = EditorGUI.PrefixLabel(keyPropertyPos, label);

            HandleKeyDragAndDrop(keyPropertyPos, property, keyProperty);

            var lastIconSize = EditorGUIUtility.GetIconSize();
            EditorGUIUtility.SetIconSize(new Vector2(16, 16));

            DropdownButtonContent.text = string.IsNullOrEmpty(keyProperty.stringValue) ? "None" : keyProperty.hasMultipleDifferentValues ? "-" : keyProperty.stringValue;
            DropdownButtonContent.image = GetIcon(property, keyProperty);

            if (EditorGUI.DropdownButton(keyPropertyPos, DropdownButtonContent, FocusType.Keyboard))
            {
                if (KnotLocalization.ProjectSettings.DefaultDatabase == null)
                {
                    if (EditorUtility.DisplayDialog("Default Database not assigned", 
                        "Default Database should be assigned in order to preview available keys", "Open Project Settings",
                        "Cancel"))
                        KnotProjectSettingsEditor.Open();
                }
                else ShowKeyPicker(keyPropertyPos, property, key =>
                {
                    SetKey(property, keyProperty, key);
                });
            }

            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUIUtility.SetIconSize(lastIconSize);

            if (property.propertyType != SerializedPropertyType.String)
                property.DrawChildProperties(position, keyProperty.propertyPath);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var keyProperty = GetKeyProperty(property);

            if (keyProperty == null)
                return base.GetPropertyHeight(property, label);

            return EditorGUI.GetPropertyHeight(keyProperty) + (property.propertyType == SerializedPropertyType.String ? 0 : property.GetChildPropertiesHeight(keyProperty.propertyPath));
        }
    }
}
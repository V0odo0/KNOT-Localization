using System;
using Knot.Localization.Attributes;
using UnityEditor;
using UnityEngine;

namespace Knot.Localization.Editor
{
    [CustomPropertyDrawer(typeof(KnotTextKeyReference), true)]
    [CustomPropertyDrawer(typeof(KnotTextKeyPickerAttribute), true)]
    public class KnotTextKeyReferenceDrawer : KnotKeyReferenceDrawer<string>
    {
        protected override Texture GetIcon(SerializedProperty parentProperty, SerializedProperty keyProperty) 
            => string.IsNullOrEmpty(keyProperty.stringValue) ? null : KnotEditorUtils.GetIcon(KnotTextKeysTabPanel.KeyViewIconName);

        protected  override void ShowKeyPicker(Rect atRect, SerializedProperty parentProperty, Action<string> keyPicked)
        {
            KnotTextKeyPickerPopup.Show(atRect, keyPicked, GetKeyProperty(parentProperty)?.stringValue);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var keyProperty = GetKeyProperty(property);
            if (keyProperty == null)
            {
                base.OnGUI(position, property, label);
                return;
            }

            Rect keyPropertyPos = position;
            
            var formattersProperty = property.FindPropertyRelative("_formatters");
            if (formattersProperty != null)
            {
                if (formattersProperty.arraySize == 0)
                {
                    keyPropertyPos.xMax -= 25;
                    var formatterPropertyPos = new Rect(keyPropertyPos.x + keyPropertyPos.width, keyPropertyPos.y, 25,
                        keyPropertyPos.height);
                    if (GUI.Button(formatterPropertyPos, EditorGUIUtility.TrTextContent("F")))
                        formattersProperty.InsertArrayElementAtIndex(formattersProperty.arraySize);
                }
            }

            EditorGUI.BeginProperty(position, label, keyProperty);

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

            if (formattersProperty != null && formattersProperty.arraySize > 0)
            {
                EditorGUI.indentLevel = 1;
                var formatterPropertyPos = position;
                formatterPropertyPos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(formatterPropertyPos, formattersProperty);
            }

            EditorGUIUtility.SetIconSize(lastIconSize);

            EditorGUI.EndProperty();
        }

        protected override string GetKeyDragAndDropDataName() => KnotTextKeysTreeView.DragAndDropKeyName;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var formattersProperty = property.FindPropertyRelative("_formatters");
            var h = EditorGUIUtility.singleLineHeight;
            if (formattersProperty != null && formattersProperty.arraySize > 0)
            {
                h += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                if (formattersProperty.isExpanded)
                    h += formattersProperty.GetChildPropertiesHeight() + EditorGUIUtility.singleLineHeight * 2;
            }

            return h;
        }
    }
}

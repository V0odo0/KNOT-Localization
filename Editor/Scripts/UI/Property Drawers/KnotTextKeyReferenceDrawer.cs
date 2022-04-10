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
            KnotTextKeyPickerPopup.Show(atRect, keyPicked);
        }

        protected override string GetKeyDragAndDropDataName() => KnotTextKeysTreeView.DragAndDropKeyName;
    }
}

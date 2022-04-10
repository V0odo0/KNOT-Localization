using System;
using System.Linq;
using Knot.Localization.Attributes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Knot.Localization.Editor
{
    [CustomPropertyDrawer(typeof(KnotAssetKeyReference), true)]
    [CustomPropertyDrawer(typeof(KnotAssetKeyPickerAttribute), true)]
    public class KnotAssetKeyReferenceDrawer : KnotKeyReferenceDrawer<Object>
    {
        public KnotAssetKeyPickerAttribute Attribute => attribute as KnotAssetKeyPickerAttribute;

        protected override Texture GetIcon(SerializedProperty parentProperty, SerializedProperty keyProperty)
        {
            if (string.IsNullOrEmpty(keyProperty.stringValue))
                return null;

            var type = GetTargetType(parentProperty);
            
            return type == null || type == typeof(Object) ? KnotEditorUtils.GetIcon(KnotAssetsKeysTabPanel.KeyViewIconName) : AssetPreview.GetMiniTypeThumbnail(type);
        }

        protected override void ShowKeyPicker(Rect atRect, SerializedProperty parentProperty, Action<string> keyPicked)
        {
            KnotAssetKeyPickerPopup.Show(atRect, GetTargetType(parentProperty), keyPicked);
        }

        protected override string GetKeyDragAndDropDataName() => KnotAssetKeysTreeView.DragAndDropKeyName;

        protected virtual Type GetTargetType(SerializedProperty parentProperty)
        {
            if (Attribute != null)
                return Attribute.BaseType;

            var typePickerAttribute = parentProperty.serializedObject.targetObject.GetType().
                GetCustomAttributes(typeof(KnotTypePickerAttribute), true).
                FirstOrDefault() as KnotTypePickerAttribute;

            return typePickerAttribute?.BaseType ?? typeof(Object);
        }
    }
}
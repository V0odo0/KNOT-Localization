using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Knot.Localization.Data;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Knot.Localization.Editor
{
    public class KnotAssetItemViewEditor  : KnotItemViewEditor<KnotAssetKeyView, KnotAssetItemView, KnotAssetData>
    {
        public readonly ObjectField ObjectField;


        public KnotAssetItemViewEditor(Action valueChanged = null) : base(nameof(KnotAssetItemViewEditor), valueChanged)
        {
            ObjectField = Root.Q<ObjectField>(nameof(ObjectField));

            ObjectField.allowSceneObjects = false;
            ObjectField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == evt.previousValue)
                    return;

                KnotEditorUtils.RegisterCompleteObjects("Asset Changed",
                    () =>
                    {
                        ItemView.ItemData.Asset = evt.newValue;
                    }, ItemView.SourceAsset);
                
                OnValueChanged();
            });
        }


        void UpdateObjectFieldType()
        {
            var assetType = typeof(Object);

            if (KeyView != null && ItemView != null && KeyView.KeyData != null)
            {
                var typeRestrictionMetadata = KeyView.KeyData.Metadata
                    .OfType<KnotAssetTypeRestrictionMetadata>().FirstOrDefault();

                if (typeRestrictionMetadata != null)
                    assetType = typeRestrictionMetadata.AssetType;
            }

            ObjectField.objectType = assetType;
        }

        protected override void OnBind()
        {
            UpdateObjectFieldType();
            ObjectField.SetValueWithoutNotify(ItemView.ItemData.Asset);
            
            ObjectField.SetEnabled(!IsReadOnly);
        }

        public override void ReloadLayout()
        {
            base.ReloadLayout();

            UpdateObjectFieldType();
        }
    }
}
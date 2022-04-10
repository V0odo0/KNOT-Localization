using System.Collections;
using System.Collections.Generic;
using Knot.Localization.Attributes;
using Knot.Localization.Data;
using UnityEngine;

namespace Knot.Localization.Editor
{
    public class KnotAssetKeyViewEditorPanel : KnotKeyViewEditorPanel<KnotAssetKeyView, KnotAssetItemView, KnotAssetData>
    {
        protected override KnotMetadataInfoAttribute.MetadataScope MetadataScope =>
            KnotMetadataInfoAttribute.MetadataScope.Asset;

        protected override List<KnotKeyCollection> KeyCollections => Database.AssetKeyCollections;


        protected override KnotAssetData GetNewItem(KnotAssetKeyView keyView) => new KnotAssetData(keyView.Key);

        protected override KnotItemViewEditor<KnotAssetKeyView, KnotAssetItemView, KnotAssetData> GetNewItemViewEditor(KnotAssetKeyView keyView, KnotAssetItemView itemView)
        {
            KnotAssetItemViewEditor editor = new KnotAssetItemViewEditor();
            
            return editor;
        }
    }
}

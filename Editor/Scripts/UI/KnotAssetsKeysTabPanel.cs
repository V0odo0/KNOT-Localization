using System;
using System.Collections.Generic;
using System.Linq;
using Knot.Localization.Attributes;
using Knot.Localization.Data;
using UnityEngine;

namespace Knot.Localization.Editor
{

    [KnotTypeInfo("Assets", 200, KeyViewIconName)]
    public class KnotAssetsKeysTabPanel : KnotKeysTabPanel<KnotAssetKeyView, KnotAssetKeysTreeView, KnotAssetItemView, KnotAssetData>
    {
        public const string KeyViewIconName = "GameObject Icon";

        protected override List<KnotKeyCollection> KeyCollections => Database.AssetKeyCollections;

        protected override Type SelectedSearchFilterType
        {
            get => KnotEditorUtils.UserSettings.AssetKeySearchFilterType.Type;
            set => KnotEditorUtils.UserSettings.AssetKeySearchFilterType.Type = value;
        }


        protected override KnotAssetKeyView GetNewKeyView(string key, KnotKeyCollection sourceCollection = null, KnotKeyData keyData = null)
            => new KnotAssetKeyView(key, sourceCollection, keyData);

        protected override KnotAssetItemView GetNewItemView(KnotAssetData itemData, IKnotItemCollection<KnotAssetData> collection)
            => new KnotAssetItemView(itemData, collection);

        protected override KnotAssetKeysTreeView GetKeysTreeView() => new KnotAssetKeysTreeView();

        protected override KnotKeyViewEditorPanel<KnotAssetKeyView, KnotAssetItemView, KnotAssetData> GetKeyViewEditor()
            => new KnotAssetKeyViewEditorPanel();

        protected override KnotAssetData GetItemCopy(string key, KnotAssetData other) => new KnotAssetData(key, other);

    }

    public class KnotAssetKeyView : KnotKeyView<KnotAssetItemView, KnotAssetData>
    {
        public override Texture2D icon => KnotEditorUtils.GetIcon(KnotAssetsKeysTabPanel.KeyViewIconName) as Texture2D;


        public KnotAssetKeyView(string key, KnotKeyCollection sourceCollection, KnotKeyData keyData = null) : base(key, sourceCollection, keyData)
        {

        }

        public override bool IsMatchSearch(string searchString)
        {
            if (Key.ContainsCaseInsensitive(searchString))
                return true;

            return LanguageItems.Select(p => p.Value).SelectMany(d => d).Select(view => view.ItemData.Asset)
                .Where(asset => asset != null).Any(asset => asset.name.ContainsCaseInsensitive(searchString));
        }
    }

    public class KnotAssetItemView : KnotItemView<KnotAssetData>
    {
        public KnotAssetItemView(KnotAssetData itemData, IKnotItemCollection<KnotAssetData> sourceCollection) : base(itemData, sourceCollection)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Knot.Localization.Attributes;
using Knot.Localization.Data;
using UnityEngine;

namespace Knot.Localization.Editor
{
    [KnotTypeInfo("Texts", 100, KeyViewIconName)]
    public class KnotTextKeysTabPanel : KnotKeysTabPanel<KnotTextKeyView, KnotTextKeysTreeView, KnotTextItemView, KnotTextData>
    {
        public const string KeyViewIconName = "UnityEditor.ConsoleWindow.png";

        protected override List<KnotKeyCollection> KeyCollections => Database.TextKeyCollections;

        protected override Type SelectedSearchFilterType
        {
            get => KnotEditorUtils.UserSettings.TextKeySearchFilterType.Type;
            set => KnotEditorUtils.UserSettings.TextKeySearchFilterType.Type = value;
        }


        protected override KnotTextKeyView GetNewKeyView(string key, KnotKeyCollection sourceCollection = null, KnotKeyData keyData = null)
            => new KnotTextKeyView(key, sourceCollection, keyData);

        protected override KnotTextItemView GetNewItemView(KnotTextData itemData, IKnotItemCollection<KnotTextData> collection)
            => new KnotTextItemView(itemData, collection);

        protected override KnotTextKeysTreeView GetKeysTreeView() => new KnotTextKeysTreeView();

        protected override KnotKeyViewEditorPanel<KnotTextKeyView, KnotTextItemView, KnotTextData> GetKeyViewEditor()
        {
            return new KnotTextKeyViewEditorPanel();
        }

        protected override KnotTextData GetItemCopy(string key, KnotTextData other) => new KnotTextData(key, other);

    }

    public class KnotTextKeyView : KnotKeyView<KnotTextItemView, KnotTextData>
    {
        public override Texture2D icon => KnotEditorUtils.GetIcon(KnotTextKeysTabPanel.KeyViewIconName) as Texture2D;
        

        public KnotTextKeyView(string key, KnotKeyCollection sourceCollection = null, KnotKeyData keyData = null) : base(key, sourceCollection, keyData)
        {

        }


        public override bool IsMatchSearch(string searchString)
        {
            return Key.ContainsCaseInsensitive(searchString) || LanguageItems.Any(p => p.Value.Any(view => view.ItemData.RawText.ContainsCaseInsensitive(searchString)));
        }
    }

    public class KnotTextItemView : KnotItemView<KnotTextData>
    {
        public KnotTextItemView(KnotTextData itemData, IKnotItemCollection<KnotTextData> sourceCollection) : base(itemData, sourceCollection)
        {

        }
    }
}
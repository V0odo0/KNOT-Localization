using System;
using System.Collections.Generic;
using System.Linq;
using Knot.Localization.Data;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Knot.Localization.Editor
{
    public class KnotTextKeyPickerPopup : KnotItemPickerPopup<string>
    {
        protected KnotTextKeyPickerPopup(List<PickerTreeViewItem> items, TreeViewState state = null) : base(items, state)
        {

        }


        static string[] GetAllKeys()
        {
            if (KnotDatabaseUtils.ActiveDatabase == null)
                return new string[0];

            var keysHashSet = new HashSet<string>();
            foreach (var textKey in KnotLocalization.ProjectSettings.DefaultDatabase.TextKeyCollections.Where(c => c != null).SelectMany(c => c))
                keysHashSet.Add(textKey.Key);

            foreach (var lang in KnotLocalization.ProjectSettings.DefaultDatabase.Languages)
            {
                var collections = lang.CollectionProviders.OfType<IKnotPersistentItemCollectionProvider>().Select(p => p.Collection).
                    Distinct().OfType<IKnotItemCollection<KnotTextData>>();

                foreach (var collection in collections)
                foreach (var textItem in collection.AsParallel())
                    keysHashSet.Add(textItem.Key);
            }

            return keysHashSet.OrderBy(text => text).ToArray();
        }

        public static void Show(Rect rect, Action<string> keyPicked, string selectedKey = "")
        {
            var allKeys = GetAllKeys();

            var treeViewItems = new List<PickerTreeViewItem>();
            for (int i = 0; i < allKeys.Length; i++)
                treeViewItems.Add(new PickerTreeViewItem(allKeys[i], i + 1, allKeys[i], KnotEditorUtils.GetIcon(KnotTextKeysTabPanel.KeyViewIconName)));

            treeViewItems.Insert(0, new PickerTreeViewItem(string.Empty, 0, "None", null));
            
            var popup = new KnotTextKeyPickerPopup(treeViewItems);
            popup.ItemPicked += keyPicked;
            
            ShowAndRememberLastFocus(rect, popup);
        }
    }
}
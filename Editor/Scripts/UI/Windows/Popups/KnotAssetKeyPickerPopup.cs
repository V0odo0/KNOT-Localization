using System;
using System.Collections.Generic;
using System.Linq;
using Knot.Localization.Data;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Knot.Localization.Editor
{
    public class KnotAssetKeyPickerPopup : KnotItemPickerPopup<string>
    {
        protected KnotAssetKeyPickerPopup(List<PickerTreeViewItem> items, TreeViewState state = null) : base(items, state)
        {

        }


        static string[] GetAllKeys(Type assetType = null)
        {
            if (KnotDatabaseUtils.ActiveDatabase == null)
                return new string[0];

            var keysHashSet = new HashSet<string>();
            foreach (var assetKey in KnotLocalization.ProjectSettings.DefaultDatabase.AssetKeyCollections.SelectMany(c => c))
            {
                if (assetType != null && assetType != typeof(Object))
                {
                    var restrictedType = assetKey.Metadata.OfType<KnotAssetTypeRestrictionMetadata>().FirstOrDefault()?.AssetType;
                    if (restrictedType == null || restrictedType != assetType)
                        continue;
                }
                
                keysHashSet.Add(assetKey.Key);
            }
                

            foreach (var lang in KnotLocalization.ProjectSettings.DefaultDatabase.Languages)
            {
                var collections = lang.CollectionProviders.OfType<IKnotPersistentItemCollectionProvider>().
                    Select(p => p.Collection).Where(c => c != null).
                    OfType<IKnotItemCollection<KnotAssetData>>().Distinct();

                foreach (var collection in collections)
                {
                    foreach (var assetItem in collection.AsParallel())
                    {
                        if (assetType != null && assetType != typeof(Object) && assetType != assetItem.Asset?.GetType())
                            continue;

                        keysHashSet.Add(assetItem.Key);
                    }
                        
                }
            }

            return keysHashSet.OrderBy(key => key).ToArray();
        }

        
        public static void Show(Rect rect, Type restrictedAssetType, Action<string> keyPicked, string selectedKey = "")
        {
            var allKeys = GetAllKeys(restrictedAssetType);

            Texture2D icon = restrictedAssetType == null
                ? null
                : AssetPreview.GetMiniTypeThumbnail(restrictedAssetType);

            var treeViewItems = new List<PickerTreeViewItem>();

            for (int i = 0; i < allKeys.Length; i++)
                treeViewItems.Add(new PickerTreeViewItem(allKeys[i], i + 1, allKeys[i], icon));

            treeViewItems.Insert(0, new PickerTreeViewItem(string.Empty, 0, "None", null));

            var popup = new KnotAssetKeyPickerPopup(treeViewItems);
            popup.ItemPicked += keyPicked;

            ShowAndRememberLastFocus(rect, popup);
        }
    }
}
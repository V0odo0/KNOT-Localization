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
    public class KnotAssetKeyPickerPopup : KnotItemKeyPickerPopup
    {
        protected KnotAssetKeyPickerPopup(List<PickerTreeViewItem> items, TreeViewState state = null) 
            : base(items, state)
        {
            
        }


        static string[] GetAllKeys(KnotDatabase db, Type assetType = null)
        {
            if (db == null)
                return Array.Empty<string>();

            bool hasAssetTypeRestriction = assetType != null && assetType != typeof(Object);
            var keys = new HashSet<string>();
            foreach (var assetKey in db.AssetKeyCollections.Where(c => c != null).SelectMany(c => c))
            {
                if (hasAssetTypeRestriction)
                {
                    var restrictedAssetType = assetKey.Metadata.OfType<KnotAssetTypeRestrictionMetadata>().FirstOrDefault()?.AssetType;
                    if (restrictedAssetType != null && restrictedAssetType != assetType)
                        continue;
                }
                
                keys.Add(assetKey.Key);
            }

            return keys.OrderBy(key => key).ToArray();
        }

        
        public static void Show(Rect rect, Type restrictedAssetType, Action<string> keyPicked, string lastSelectedKey = "")
        {
            Texture2D icon = restrictedAssetType == null
                ? null
                : AssetPreview.GetMiniTypeThumbnail(restrictedAssetType);

            var treeViewItems = new List<PickerTreeViewItem> { new PickerTreeViewItem(string.Empty, 0, "None", null) };
            var allKeys = GetAllKeys(KnotLocalization.ProjectSettings.DefaultDatabase, restrictedAssetType);
            for (int i = 0; i < allKeys.Length; i++)
                treeViewItems.Add(new PickerTreeViewItem(allKeys[i], i + 1, allKeys[i], icon));
            
            var popup = new KnotAssetKeyPickerPopup(treeViewItems);
            popup.ItemPicked += keyPicked;
            
            if (!string.IsNullOrEmpty(lastSelectedKey))
                popup.SelectItem(lastSelectedKey);

            ShowAndRememberLastFocus(rect, popup);
        }
    }
}
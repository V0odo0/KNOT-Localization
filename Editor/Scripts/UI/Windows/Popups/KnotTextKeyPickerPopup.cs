using System;
using System.Collections.Generic;
using System.Linq;
using Knot.Core.Editor;
using Knot.Localization.Data;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Knot.Localization.Editor
{
    public class KnotTextKeyPickerPopup : KnotItemKeyPickerPopup
    {
        protected KnotTextKeyPickerPopup(List<PickerTreeViewItem> items, TreeViewState state = null) 
            : base(items, state)
        {

        }

        static string[] GetAllKeys(KnotDatabase db)
        {
            if (db == null)
                return Array.Empty<string>();

            var keys = new HashSet<string>();
            foreach (var textKey in db.TextKeyCollections.Where(c => c != null).SelectMany(c => c))
                keys.Add(textKey.Key);

            return keys.OrderBy(key => key).ToArray();
        }


        public static void Show(Rect rect, Action<string> keyPicked, string lastSelectedKey = "")
        {
            var allKeys = GetAllKeys(KnotLocalization.ProjectSettings.DefaultDatabase);

            var treeViewItems = new List<PickerTreeViewItem> { new PickerTreeViewItem(string.Empty, 0, "None", null) };
            for (int i = 0; i < allKeys.Length; i++)
                treeViewItems.Add(new PickerTreeViewItem(allKeys[i], i + 1, allKeys[i], Core.Editor.EditorUtils.GetIcon(KnotTextKeysTabPanel.KeyViewIconName)));
            
            var popup = new KnotTextKeyPickerPopup(treeViewItems);
            popup.ItemPicked += keyPicked;
            
            if (!string.IsNullOrEmpty(lastSelectedKey))
                popup.SelectItem(lastSelectedKey);

            ShowAndRememberLastFocus(rect, popup);
        }
    }
}
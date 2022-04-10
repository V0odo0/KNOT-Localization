using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Knot.Localization.Data;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Knot.Localization.Editor
{
    public class KnotCulturePickerPopup : KnotItemPickerPopup<CultureInfo>
    {
        protected KnotCulturePickerPopup(List<PickerTreeViewItem> items, TreeViewState state = null) : base(items, state)
        {

        }

        protected override bool DoesItemMatchSearch(string search, PickerTreeViewItem treeViewItem)
        {
            return treeViewItem.Item.EnglishName.ContainsCaseInsensitive(search) ||
                   treeViewItem.Item.Name.ContainsCaseInsensitive(search);
        }


        public static void Show(Rect rect, Action<string> cultureNamePicked, string selectedCultureName = "", params string[] cultureNames)
        {
            var items = (cultureNames.Length == 0 
                ? CultureInfo.GetCultures(CultureTypes.AllCultures) 
                : cultureNames.Select(CultureInfo.GetCultureInfo)).OrderBy(c => c.EnglishName).ToArray();

            var treeViewItems = new List<PickerTreeViewItem>();
            for (int i = 0; i < items.Length; i++)
                treeViewItems.Add(new PickerTreeViewItem(items[i], i, items[i].GetDisplayName(), 0));
            
            var popup = new KnotCulturePickerPopup(treeViewItems);
            popup.ItemPicked += info => cultureNamePicked?.Invoke(info.Name);

            if (!string.IsNullOrEmpty(selectedCultureName))
                popup.SelectItem(items.FirstOrDefault(c => c.Name == selectedCultureName));

            ShowAndRememberLastFocus(rect, popup);
        }
    }
}
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine.UIElements;

namespace Knot.Localization.Editor
{
    public class KnotItemKeyPickerPopup : KnotItemPickerPopup<string>
    {
        protected KnotItemKeyPickerPopup(List<PickerTreeViewItem> items, TreeViewState state = null) : base(items, state)
        {
            if (!KnotLocalization.ProjectSettings.DefaultDatabase.IsPersistent())
            {
                var noDatabaseLabel = new Label("No Default Database assigned.");
                noDatabaseLabel.style.SetMargins(3f);

                var noDatabaseButton = new Button(KnotProjectSettingsEditor.Open)
                {
                    text = "Open Project Settings"
                };

                Panel.Root.Insert(0, noDatabaseButton);
                Panel.Root.Insert(0, noDatabaseLabel);
            }
        }
    }
}

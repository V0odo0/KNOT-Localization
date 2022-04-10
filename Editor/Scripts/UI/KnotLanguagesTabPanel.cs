using System.Globalization;
using System.Linq;
using Knot.Localization.Attributes;
using Knot.Localization.Data;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Knot.Localization.Editor
{
    [KnotTypeInfo("Languages", 0, "Favorite.png")]
    public class KnotLanguagesTabPanel : KnotLayoutPanel, IKnotDatabaseEditorTab
    {
        public const string LanguageIconName = "Favorite.png";

        public VisualElement RootVisualElement => Root;
        
        public readonly KnotLanguageDataEditorPanel LanguageDataEditorPanel;

        public readonly IMGUIContainer TreeViewContainer;
        public readonly KnotLanguagesTreeView TreeView;
        public readonly ToolbarButton AddLanguageButton;

        protected override VisualElement ItemEditorPanel => LanguageDataEditorPanel;


        public KnotLanguagesTabPanel() : base(nameof(KnotLanguagesTabPanel))
        {
            TreeView = new KnotLanguagesTreeView(Database, KnotEditorUtils.UserSettings.LanguagesTreeViewState);
            TreeView.Selected += SelectLanguage;
            TreeView.RequestRemove += RemoveLanguage;
            TreeView.RequestMove += MoveLanguage;

            TreeViewContainer = Root.Q<IMGUIContainer>(nameof(TreeViewContainer));
            TreeViewContainer.onGUIHandler += () => TreeView.OnGUI(TreeViewContainer.contentRect);

            AddLanguageButton = Root.Q<ToolbarButton>(nameof(AddLanguageButton));
            AddLanguageButton.Add(new Image { image = KnotEditorUtils.GetIcon("Toolbar Plus More") });
            AddLanguageButton.clickable.clicked += () =>
            {
                var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures).Select(c => c.Name)
                    .Except(Database.Languages.Select(d => d.CultureName)).ToArray();
                KnotCulturePickerPopup.Show(AddLanguageButton.worldBound, AddLanguage, string.Empty, cultures);
            };

            LanguageDataEditorPanel = new KnotLanguageDataEditorPanel();

            SelectLanguage(TreeView.SelectedId);
        }


        void SelectLanguage(int id)
        {
            bool isValidId = id >= 0 && id <= Database.Languages.Count - 1;
            
            if (isValidId)
            {
                if (!TabContentRoot.Contains(LanguageDataEditorPanel))
                    TabContentRoot.Add(LanguageDataEditorPanel);

                LanguageDataEditorPanel.Bind(Database.Languages[id]);
            }
            else if (TabContentRoot.Contains(LanguageDataEditorPanel))
                TabContentRoot.Remove(LanguageDataEditorPanel);
        }

        void RemoveLanguage(int id)
        {
            if (Database.Languages.Count <= id)
                return;

            KnotEditorUtils.RecordObjects("Remove Language", () =>
            {
                Database.Languages.RemoveAt(id);
            }, Database);

            ReloadLayout();
        }
        
        void MoveLanguage(int id, int targetId)
        {
            KnotLanguageData srcData = Database.Languages[id];
            KnotEditorUtils.RecordObjects("Move Language", () =>
            {
                Database.Languages.RemoveAt(id);
                if (targetId > id)
                    targetId--;
                Database.Languages.Insert(Mathf.Clamp(targetId, 0, Database.Languages.Count), srcData);
            }, Database);

            TreeView.SelectedId = Database.Languages.IndexOf(srcData);
            ReloadLayout();
        }

        void AddLanguage(string cultureName)
        {
            if (Database.Languages.Any(d => d.CultureName == cultureName))
                return;

            KnotEditorUtils.RecordObjects("Add language", () =>
            {
                Database.Languages.Add(new KnotLanguageData(cultureName));
            }, Database);

            ReloadLayout();
            TreeView.SelectLast(true);
        }


        public override void ReloadLayout()
        {
            TreeView.Reload();
            SelectLanguage(TreeView.SelectedId);
        }
    }
}


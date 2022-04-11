using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Knot.Localization.Editor
{
    public class KnotEditorToolbarPanel : KnotEditorPanel
    {
        public EditorTab SelectedTab
        {
            get => Tabs[Toolbar.SelectedTabId];
            set
            {
                if (!_tabs.Contains(value))
                    return;

                TabContentContainer.Clear();
                TabContentContainer.Add(value.Instance.RootVisualElement);

                Toolbar.SelectedTabId = _tabs.IndexOf(value);
            }
        }

        public IReadOnlyList<EditorTab> Tabs => _tabs;
        private readonly List<EditorTab> _tabs = new List<EditorTab>();

        internal readonly KnotToolbar Toolbar;
        internal readonly VisualElement TabContentContainer;


        public KnotEditorToolbarPanel() : base(nameof(KnotEditorToolbarPanel))
        {
            var allDatabaseEditorTabs = typeof(IKnotDatabaseEditorTab).GetDerivedTypesInfo();
            foreach (var tabType in allDatabaseEditorTabs)
                _tabs.Add(new EditorTab(tabType.Type));

            Toolbar = new KnotToolbar(Root.Q<Toolbar>(nameof(Toolbar)), allDatabaseEditorTabs.Select(info => info.Content).ToArray());
            Toolbar.TabSelected += tabId =>
            {
                SelectedTab = _tabs[tabId];
                KnotEditorUtils.UserSettings.EditorTabType.Type = SelectedTab.TabType;
            };

            TabContentContainer = Root.Q<VisualElement>(nameof(TabContentContainer));

            var targetTab = Tabs.FirstOrDefault(tab => tab.TabType == KnotEditorUtils.UserSettings.EditorTabType.Type);
            SelectedTab = targetTab ?? Tabs.FirstOrDefault();
        }


        public override void ReloadLayout()
        {
            SelectedTab?.Instance.ReloadLayout();
        }


        public class EditorTab
        {
            public readonly Type TabType;

            public IKnotDatabaseEditorTab Instance
            {
                get
                {
                    return _instance ?? (_instance = typeof(IKnotDatabaseEditorTab).GetDerivedTypesInfo()
                        .FirstOrDefault(info => info.Type == TabType)?.GetInstance() as IKnotDatabaseEditorTab);
                }
            }
            private IKnotDatabaseEditorTab _instance;


            public EditorTab(Type tabType)
            {
                TabType = tabType;
            }
        }
    }
}

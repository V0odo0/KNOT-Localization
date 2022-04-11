using System;
using System.Collections.Generic;
using System.Linq;
using Knot.Localization.Data;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Knot.Localization.Editor
{
    public abstract class KnotKeysTabPanel<TKeyView, TKeysTreeView, TItemView, TItemData> : KnotLayoutPanel, IKnotDatabaseEditorTab
        where TKeyView : KnotKeyView<TItemView, TItemData>
        where TKeysTreeView : KnotKeysTreeView<TKeyView>
        where TItemView : KnotItemView<TItemData>
        where TItemData : KnotItemData
    {
        public static IKnotKeySearchFilter<TKeyView>[] KeySearchFilters =>
            _keySearchFilters ?? (_keySearchFilters = typeof(IKnotKeySearchFilter<TKeyView>).
                GetDerivedTypesInfo().Select(info => info.GetInstance() as IKnotKeySearchFilter<TKeyView>).ToArray());
        private static IKnotKeySearchFilter<TKeyView>[] _keySearchFilters;

        public VisualElement RootVisualElement => Root;

        public List<TKeyView> KeyViews => _keyViews ?? (_keyViews = new List<TKeyView>());
        private List<TKeyView> _keyViews;

        public bool HasAnyTargetCollection => KeyViews.Count > 0 || KeyCollections.Any(k => k.IsPersistent());

        public KnotKeyViewEditorPanel<TKeyView, TItemView, TItemData> KeyViewEditor
        {
            get
            {
                if (_keyViewEditor == null)
                {
                    _keyViewEditor = GetKeyViewEditor();
                    _keyViewEditor.ItemChanged += ReloadLayout;
                }
                return _keyViewEditor;
            }
        }
        private KnotKeyViewEditorPanel<TKeyView, TItemView, TItemData> _keyViewEditor;

        public readonly IMGUIContainer TreeViewContainer;
        public readonly TKeysTreeView TreeView;

        public readonly ToolbarButton AddKeyButton;
        public readonly KnotKeySearchField<TKeyView> KeySearchField;

        protected override VisualElement ItemEditorPanel => KeyViewEditor;

        protected abstract List<KnotKeyCollection> KeyCollections { get; }
        protected abstract Type SelectedSearchFilterType { get; set; }

        protected virtual IKnotKeySearchFilter<TKeyView> SelectedSearchFilter
        {
            get => KeySearchFilters.FirstOrDefault(f =>
                f.GetType() == SelectedSearchFilterType);
            set => SelectedSearchFilterType = value?.GetType();
        }


        protected KnotKeysTabPanel() : base("KnotKeysTabPanel")
        {
            AddKeyButton = Root.Q<ToolbarButton>(nameof(AddKeyButton));
            AddKeyButton.Add(new Image { image = KnotEditorUtils.GetIcon("Toolbar Plus More") });
            AddKeyButton.clicked += () =>
            {
                if (!IsKeyCollectionsPersistent())
                    return;

                string keyPrefix = string.Empty;
                var selectedKeyView = TreeView.SelectedKeyItems.FirstOrDefault();
                if (selectedKeyView != null)
                {
                    string key = selectedKeyView.Key;
                    if (selectedKeyView is KnotTreeViewKeyItemGroup)
                        keyPrefix = $"{key}.";
                    else if (key.Contains('.') && !key.EndsWith("."))
                        keyPrefix = key.Remove(key.LastIndexOf('.') + 1);
                }

                KnotCreateKeyPopup.Show(AddKeyButton.worldBound, KeyCollections, (collection, key) =>
                {
                    if (KeyViews.FirstOrDefault(v => v.Key == key)?.KeyData != null)
                        TreeView.FrameKey(key, true);
                    else AddKeys(collection, key);

                }, keyPrefix);
            };

            KeySearchField = new KnotKeySearchField<TKeyView>(Toolbar.Q<ToolbarPopupSearchField>(nameof(KeySearchField)), KeySearchFilters);
            KeySearchField.SelectedSearchFilter = SelectedSearchFilter;
            KeySearchField.Root.SetValueWithoutNotify(KnotEditorUtils.UserSettings.TextKeysTreeViewState.searchString);
            KeySearchField.ValueChanged += value =>
            {
                TreeView.searchString = value;
                UpdateSearchResultsLabel();
            };
            KeySearchField.SearchFilterSelected += filter =>
            {
                SelectedSearchFilter = filter;
                TreeView.SearchFilter = filter;
            };

            TreeView = GetKeysTreeView();
            TreeView.SearchFilter = SelectedSearchFilter;

            TreeView.RemoveKeys += RemoveKeys;
            TreeView.DuplicateKeys += DuplicateKeys;
            TreeView.KeysSelected += SelectKeys;
            
            TreeViewContainer = Root.Q<IMGUIContainer>(nameof(TreeViewContainer));
            TreeViewContainer.onGUIHandler = OnTreeViewGUI;

            KeyViewEditor.KeyChanged += (oldKey, newKey) =>
            {
                ReloadLayout();
                TreeView.FrameKey(newKey, true);
            };
            KeyViewEditor.AddToKeyCollection += (collection, view) =>
            {
                AddToKeyCollection(collection, view);
            };
        }


        protected virtual void BuildKeyViews()
        {
            KeyViews.Clear();

            Dictionary<string, TKeyView> allKeyViews = new Dictionary<string, TKeyView>();
            foreach (var keyCollection in KeyCollections.Where(c => c != null).Distinct())
            foreach (var keyData in keyCollection)
                if (!allKeyViews.ContainsKey(keyData.Key))
                    allKeyViews.Add(keyData.Key, GetNewKeyView(keyData.Key, keyCollection, keyData));
            
            foreach (var languageData in Database.Languages)
            {
                var itemCollections = languageData.CollectionProviders.OfType<IKnotPersistentItemCollectionProvider>()
                    .Select(p => p.Collection).Distinct().OfType<IKnotItemCollection<TItemData>>();

                foreach (var collection in itemCollections)
                {
                    foreach (var itemData in collection)
                    {
                        if (itemData == null)
                            continue;

                        var itemView = GetNewItemView(itemData, collection);
                        if (allKeyViews.TryGetValue(itemData.Key, out TKeyView view))
                            view.AppendItemView(languageData, itemView);
                        else
                        {
                            var newKeyView = GetNewKeyView(itemData.Key);
                            newKeyView.AppendItemView(languageData, itemView);
                            allKeyViews.Add(itemData.Key, newKeyView);
                        }
                    }
                }
            }

            KeyViews.AddRange(allKeyViews.Values);
        }

        protected abstract TKeyView GetNewKeyView(string key, KnotKeyCollection sourceCollection = null, KnotKeyData keyData = null);

        protected abstract TItemView GetNewItemView(TItemData itemData, IKnotItemCollection<TItemData> collection);

        protected abstract TKeysTreeView GetKeysTreeView();

        protected abstract KnotKeyViewEditorPanel<TKeyView, TItemView, TItemData> GetKeyViewEditor();
        
        protected abstract TItemData GetItemCopy(string key, TItemData other);


        protected virtual void AddKeys(KnotKeyCollection keyCollection, params string[] keys)
        {
            var keysToAdd = keys.Where(key => !keyCollection.ContainsKey(key)).ToArray();
            if (keysToAdd.Any())
            {
                KnotEditorUtils.RegisterCompleteObjects("Add Keys", () =>
                {
                    foreach (var key in keysToAdd)
                        keyCollection.Add(new KnotKeyData(key));
                }, keyCollection);
                ReloadLayout();
            }
            TreeView.FrameKeys(true, keys);
        }

        protected virtual void RemoveKeys(params TKeyView[] keyViews)
        {
            var itemViews = keyViews.SelectMany(view => view.LanguageItems.Values).SelectMany(list => list).ToArray();
            var collectionAssets = itemViews.Select(view => view.SourceAsset as UnityEngine.Object).
                Where(o => o != null).ToArray();

            KnotEditorUtils.RegisterCompleteObjects("Remove Keys", () =>
            {
                foreach (var keyView in keyViews.Where(v => v.KeyData != null))
                    keyView.SourceCollection.RemoveKey(keyView.Key);

                foreach (var itemView in itemViews)
                    itemView.SourceCollection.Remove(itemView.ItemData);

            }, collectionAssets.Union(keyViews.Select(v => v.SourceCollection)).ToArray());
            
            ReloadLayout();
        }

        protected virtual void DuplicateKeys(params TKeyView[] keyViews)
        {
            var itemViews = keyViews.SelectMany(view => view.LanguageItems.Values)
                .SelectMany(list => list).ToArray();
            var collectionAssets = itemViews.Select(view => view.SourceAsset as UnityEngine.Object).
                Where(o => o != null).Distinct().ToArray();

            var newKeys = new Dictionary<string, string>();
            var allKeys = new HashSet<string>(KeyViews.Select(v => v.Key));

            KnotEditorUtils.RegisterCompleteObjects("Duplicate Keys", () =>
            {
                //Duplicate keys
                foreach (var keyView in keyViews.Where(view => view.SourceCollection != null && view.SourceCollection.ContainsKey(view.Key)))
                {
                    string newKey = ObjectNames.GetUniqueName(allKeys.ToArray(), keyView.Key);
                    keyView.SourceCollection.Add(new KnotKeyData(newKey, keyView.KeyData));
                    allKeys.Add(newKey);
                    newKeys.Add(keyView.Key, newKey);
                }

                //Duplicate collection items
                foreach (var keyView in keyViews)
                {
                    foreach (var itemView in keyView.LanguageItems.SelectMany(pair => pair.Value).GroupBy(itemView => itemView.SourceCollection).Select(g => g.First()))
                    {
                        if (newKeys.ContainsKey(itemView.ItemData.Key))
                            itemView.SourceCollection.Add(GetItemCopy(newKeys[itemView.ItemData.Key], itemView.ItemData));
                        else
                        {
                            string newKey = ObjectNames.GetUniqueName(allKeys.ToArray(), itemView.ItemData.Key);
                            itemView.SourceCollection.Add(GetItemCopy(newKey, itemView.ItemData));
                            allKeys.Add(newKey);
                            newKeys.Add(itemView.ItemData.Key, newKey);
                        }
                    }
                }

            }, collectionAssets.Union(keyViews.Select(v => v.SourceCollection)).ToArray());
            
            ReloadLayout();
            TreeView.FrameKeys(true, newKeys.Values.ToArray());
        }

        protected virtual void AddToKeyCollection(KnotKeyCollection keyCollection, params TKeyView[] keyViews)
        {
            if (!IsKeyCollectionsPersistent())
                return;

            KnotEditorUtils.RegisterCompleteObjects("Add Keys", () =>
            {
                foreach (var keyView in keyViews)
                    if (!keyCollection.ContainsKey(keyView.Key))
                        keyCollection.Add(new KnotKeyData(keyView.Key));
            }, keyCollection);

            ReloadLayout();
            TreeView.FrameKeys(true, keyViews.Select(view => view.Key).ToArray());
        }

        protected virtual void RemoveFromKeyCollection(params TKeyView[] keyViews)
        {
            if (keyViews.All(v => v.SourceCollection == null))
                return;

            KnotEditorUtils.RegisterCompleteObjects("Remove from Keys Collection", () =>
            {
                foreach (var keyView in keyViews.Where(v => v.SourceCollection != null))
                    keyView.SourceCollection.RemoveKey(keyView.Key);

            }, keyViews.Select(v => v.SourceCollection).ToArray());

            ReloadLayout();
            TreeView.FrameKeys(true, keyViews.Select(view => view.Key).ToArray());
        }
        

        protected virtual bool IsKeyCollectionsPersistent()
        {
            if (KeyCollections.Any(c => c.IsPersistent()))
                return true;

            int choiceId = EditorUtility.DisplayDialogComplex("No Key Collection",
                "Key Collection not assigned. Open Settings tab to assign Key Collection or create a new one.",
                "Create", "Open Settings", "Cancel");

            switch (choiceId)
            {
                default:
                    return false;
                case 0:
                    var keyCollection = KnotEditorUtils.RequestCreateAsset<KnotKeyCollection>("Key Collection");
                    if (keyCollection != null)
                    {
                        KeyCollections.Add(keyCollection);
                        EditorUtility.SetDirty(Database);
                        return true;
                    }
                    return false;
                case 1:
                    var tabPanel = EditorWindow.GetWindow<KnotDatabaseEditorWindow>().EditorToolbarPanel;
                    if (tabPanel != null)
                        tabPanel.SelectedTab = tabPanel.Tabs.FirstOrDefault(t => t.TabType == typeof(KnotDatabaseSettingsTabPanel));
                    return false;
            }
        }

        protected virtual void SelectKeys(TKeyView[] keyViews)
        {
            if (keyViews == null || keyViews.Length == 0)
            {
                if (TabContentRoot.Contains(KeyViewEditor))
                    TabContentRoot.Remove(KeyViewEditor);

                return;
            }

            if (!TabContentRoot.Contains(KeyViewEditor))
                TabContentRoot.Add(KeyViewEditor);

            KeyViewEditor.Bind(keyViews);
        }

        protected override void BuildTabOptionsMenu(DropdownMenu menu)
        {
            base.BuildTabOptionsMenu(menu);

            foreach (KnotKeysTreeViewMode mode in Enum.GetValues(typeof(KnotKeysTreeViewMode)))
            {
                menu.AppendAction($"Tree View Mode/{ObjectNames.NicifyVariableName(mode.ToString())}", action =>
                    {
                        TreeView.TreeViewMode = mode;
                    },
                    action => KnotEditorUtils.UserSettings.KeysTreeViewMode == mode
                        ? DropdownMenuAction.Status.Checked
                        : DropdownMenuAction.Status.Normal);
            }
        }
        
        protected void UpdateSearchResultsLabel()
        {
            int totalKeysCount = TreeView.AllKeyItems.Count(item => item is TKeyView);
            int curKeysCount = TreeView.hasSearch ? TreeView.GetRows().Count(item => item is TKeyView) : totalKeysCount;
            KeySearchField.SetResultsCount(curKeysCount, totalKeysCount);
        }


        protected virtual void OnTreeViewGUI()
        {
            if (!HasAnyTargetCollection)
                return;

            TreeView.OnGUI(TreeViewContainer.contentRect);
        }
        
        public override void ReloadLayout()
        {
            BuildKeyViews();
            TreeView.Bind(KeyViews);
            SelectKeys(TreeView.SelectedKeyViews);

            UpdateSearchResultsLabel();
        }
    }

    public abstract class KnotKeyView<TItemView, TItem> : KnotTreeViewKeyItem 
        where TItemView : KnotItemView<TItem> 
        where TItem : KnotItemData
    {
        public override string Key { get; }

        public IReadOnlyDictionary<KnotLanguageData, List<TItemView>> LanguageItems => _languageItems;
        protected Dictionary<KnotLanguageData, List<TItemView>> _languageItems = new Dictionary<KnotLanguageData,List<TItemView>>();

        public override KnotKeyData KeyData { get; }
        public KnotKeyCollection SourceCollection;

        
        protected KnotKeyView(string key, KnotKeyCollection sourceCollection, KnotKeyData keyData = null)
        {
            Key = key;
            KeyData = keyData;
            SourceCollection = sourceCollection;
        }

        public void AppendItemView(KnotLanguageData languageData, TItemView itemView)
        {
            if (!LanguageItems.ContainsKey(languageData))
                _languageItems.Add(languageData, new List<TItemView>());

            _languageItems[languageData].Add(itemView);
        }
    }

    public class KnotItemView<TItemData> where TItemData : KnotItemData
    {
        public KnotItemCollection SourceAsset => SourceCollection as KnotItemCollection;

        public TItemData ItemData;
        public IKnotItemCollection<TItemData> SourceCollection;


        public KnotItemView(TItemData itemData, IKnotItemCollection<TItemData> sourceCollection)
        {
            ItemData = itemData;
            SourceCollection = sourceCollection;
        }

        public override string ToString()
        {
            return $"{ItemData?.Key}";
        }
    }
}
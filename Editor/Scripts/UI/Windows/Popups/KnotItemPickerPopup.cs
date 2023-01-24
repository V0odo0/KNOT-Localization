using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Knot.Localization.Editor
{
    public abstract class KnotItemPickerPopup<TItem> : KnotPopupWindowContent where TItem : class
    {
        internal static Vector2 DefaultWindowSize = new Vector2(350, 400);

        public event Action<TItem> ItemPicked;

        public virtual TItem SelectedItem { get; protected set; }

        protected readonly ToolbarSearchField SearchField;
        protected readonly TextField SearchTextField;
        protected readonly IMGUIContainer TreeViewContainer;
        protected readonly IMGUIContainer ItemPreviewContainer;
        protected PickerTreeView TreeView;
        

        protected KnotItemPickerPopup(List<PickerTreeViewItem> items, TreeViewState state = null) : base("KnotItemPickerPopup")
        {
            TreeView = new PickerTreeView(items ?? new List<PickerTreeViewItem>(), state);
            TreeView.ItemPicked += OnItemPicked;
            TreeView.ItemSelected += OnItemSelected;

            TreeView.SearchHandler = DoesItemMatchSearch;

            SearchField = Panel.Root.Q<ToolbarSearchField>(nameof(SearchField));
            SearchField.RegisterValueChangedCallback(evt => { TreeView.searchString = evt.newValue; });
            SearchTextField = SearchField.Q<TextField>();

            TreeViewContainer = Panel.Root.Q<IMGUIContainer>(nameof(TreeViewContainer));
            TreeViewContainer.onGUIHandler += () => TreeView.OnGUI(TreeViewContainer.contentRect);

            ItemPreviewContainer = Panel.Root.Q<IMGUIContainer>(nameof(ItemPreviewContainer));
            ItemPreviewContainer.onGUIHandler += () =>
            {
                
            };

            SetDeferredFocusTarget(SearchTextField.GetVisualInput());
        }


        protected void SelectItem(TItem item)
        {
            var targetTreeViewItem = TreeView.Items.FirstOrDefault(t => t.Item.Equals(item));
            if (targetTreeViewItem != null)
            {
                TreeView.SetSelection(new List<int> { targetTreeViewItem.id }, TreeViewSelectionOptions.FireSelectionChanged);
                TreeView.FrameItem(targetTreeViewItem.id);
                TreeView.FrameItem(targetTreeViewItem.id);
            }
        }

        protected virtual bool DoesItemMatchSearch(string search, PickerTreeViewItem treeViewItem) => treeViewItem.displayName.ContainsCaseInsensitive(search);
        
        protected virtual void OnItemPicked(TItem item)
        {
            ItemPicked?.Invoke(item);
            Close();
        }

        protected virtual void OnItemSelected(TItem item) { }

        protected virtual void OnPreviewGUI(TItem item) {}


        public override Vector2 GetWindowSize() => DefaultWindowSize;


        protected class PickerTreeView : UnityEditor.IMGUI.Controls.TreeView
        {
            public event Action<TItem> ItemSelected;
            public event Action<TItem> ItemPicked; 

            public List<PickerTreeViewItem> Items
            {
                get => _items;
                set
                {
                    if (value == null)
                        _items.Clear();
                    else _items = value;

                    Reload();
                }
            }
            private List<PickerTreeViewItem> _items = new List<PickerTreeViewItem>();

            public DoesItemMatchSearchHandler SearchHandler;


            public PickerTreeView(List<PickerTreeViewItem> items, TreeViewState state = null) : base(state ?? new TreeViewState())
            {
                _items = items ?? new List<PickerTreeViewItem>();
                Reload();
            }


            protected override TreeViewItem BuildRoot()
            {
                var root = new TreeViewItem(-1, -1, "Root")
                {
                    children = Items.Cast<TreeViewItem>().ToList()
                };

                return root;
            }


            protected override bool CanMultiSelect(TreeViewItem item) => false;

            protected override void DoubleClickedItem(int id) => ItemPicked?.Invoke(Items[id].Item);

            //protected override void SingleClickedItem(int id) => ItemPicked?.Invoke(Items[id].Item);

            protected override void KeyEvent()
            {
                if (Event.current.type == EventType.KeyDown)
                {
                    switch (Event.current.keyCode)
                    {
                        case KeyCode.Return:
                            if (HasSelection())
                                ItemPicked?.Invoke(Items[GetSelection()[0]].Item);
                            break;
                    }
                }
            }

            protected override void SelectionChanged(IList<int> selectedIds)
            {
                var selectedTreeViewItem = Items.FirstOrDefault(item => selectedIds.Contains(item.id));
                if (selectedTreeViewItem != null)
                    ItemSelected?.Invoke(selectedTreeViewItem.Item);
            }

            protected override bool DoesItemMatchSearch(TreeViewItem item, string search)
            {
                return SearchHandler?.Invoke(search, item as PickerTreeViewItem) ?? item.displayName.ContainsCaseInsensitive(search);
            }
        }

        protected class PickerTreeViewItem : TreeViewItem
        {
            public TItem Item { get; }


            public PickerTreeViewItem(TItem item, int id, string displayName, Texture itemIcon = null) : base(id, 0, displayName)
            {
                Item = item;
                icon = itemIcon as Texture2D;
            }

            public PickerTreeViewItem(TItem item, int id, string displayName, int depth = 0 ,Texture itemIcon = null) : base(id, depth, displayName)
            {
                Item = item;
                icon = itemIcon as Texture2D;
            }
        }

        protected delegate bool DoesItemMatchSearchHandler(string search, PickerTreeViewItem item);
    }
}
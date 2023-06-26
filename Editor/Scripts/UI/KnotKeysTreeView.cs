using System;
using System.Collections.Generic;
using System.Linq;
using Knot.Localization.Data;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

namespace Knot.Localization.Editor
{
    public abstract class KnotKeysTreeView<TKeyView> : TreeView where TKeyView : KnotTreeViewKeyItem
    {
        public static string DragAndDropKeyName => $"{KnotEditorUtils.CorePrefix}.{typeof(TKeyView).Name}";

        static readonly GUIContent AddToKeyCollectionContent = new GUIContent("Add to Key Collection");
        static readonly GUIContent RemoveFromKeyCollectionContent = new GUIContent("Remove from Key Collection");
        static readonly GUIContent DuplicateKeyContent = new GUIContent("Duplicate");
        static readonly GUIContent RemoveKeyContent = new GUIContent("Remove");
        

        public event Action<TKeyView[]> KeysSelected;
        public event Action<TKeyView[]> RemoveKeys;
        public event Action<TKeyView[]> DuplicateKeys;


        public IReadOnlyList<KnotTreeViewKeyItem> AllKeyItems => _allKeyItems;
        private List<KnotTreeViewKeyItem> _allKeyItems = new List<KnotTreeViewKeyItem>();

        public IEnumerable<KnotTreeViewKeyItem> SelectedKeyItems => FindRows(GetSelection()).OfType<KnotTreeViewKeyItem>();
        public TKeyView[] SelectedKeyViews => SelectedKeyItems.OfType<TKeyView>().ToArray();
        
        public KnotKeysTreeViewMode TreeViewMode
        {
            get => KnotEditorUtils.UserSettings.KeysTreeViewMode;
            set
            {
                if (value == KnotEditorUtils.UserSettings.KeysTreeViewMode)
                    return;

                KnotEditorUtils.UserSettings.KeysTreeViewMode = value;
                Reload();
            }
        }

        public IKnotKeySearchFilter<TKeyView> SearchFilter
        {
            get => _searchFilter;
            set
            {
                _searchFilter = value;
                Reload();
            }
        }
        private IKnotKeySearchFilter<TKeyView> _searchFilter;

        protected abstract IKnotKeyViewLabel<TKeyView>[] KeyViewLabels { get; }

        private List<TKeyView> _keyViews = new List<TKeyView>();


        protected KnotKeysTreeView(TreeViewState state): base(state)
        {
            
        }


        protected sealed override TreeViewItem BuildRoot()
        {
            _allKeyItems.Clear();
            _allKeyItems.AddRange(_keyViews);

            if (TreeViewMode == KnotKeysTreeViewMode.Hierarchy)
                _allKeyItems.AddRange(GetAllKeyGroups(_allKeyItems));
            _allKeyItems.Sort();
            
            TreeViewItem root = new TreeViewItem(-1, -1, "Root")
            {
                children = AllKeyItems.Cast<TreeViewItem>().ToList()
            };

            SetupParentsAndChildrenFromDepths(root, root.children);
            
            return root;
        }


        protected virtual List<KnotTreeViewKeyItemGroup> GetAllKeyGroups(List<KnotTreeViewKeyItem> items)
        {
            Dictionary<string, KnotTreeViewKeyItemGroup> groups = new Dictionary<string, KnotTreeViewKeyItemGroup>();
            
            string tempKey;
            foreach (var item in items.AsParallel().Where(item => item.Key.Contains('.')))
            {
                tempKey = item.Key;
                do
                {
                    tempKey = tempKey.Remove(tempKey.LastIndexOf('.'));
                    if (items.All(keyItem => keyItem.Key != tempKey) && !groups.ContainsKey(tempKey))
                        groups.Add(tempKey, new KnotTreeViewKeyItemGroup(tempKey));
                }
                while (tempKey.Contains('.'));
            }

            return groups.Values.ToList();
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            TKeyView keyView = args.item as TKeyView;

            EditorGUI.BeginDisabledGroup(keyView != null && keyView.KeyData == null);

            if (keyView != null)
            {
                args.rowRect.x = args.rowRect.width;
                foreach (var label in KeyViewLabels.OrderBy(l => l.Order).Where(l => l.IsAssignableTo(keyView)))
                    DrawLabelContent(ref args.rowRect, label.GetLabelContent(keyView), args.selected);
            }

            args.rowRect.x = 0;
            base.RowGUI(args);

            EditorGUI.EndDisabledGroup();
        }

        protected virtual void DrawLabelContent(ref Rect labelsRect, GUIContent label, bool rowSelected)
        {
            labelsRect.x -= GUI.skin.label.CalcSize(label).x;
            if (rowSelected && label.image != null)
                label.image = KnotEditorUtils.GetIconActiveState(label.image.name) ?? label.image;

            EditorGUI.LabelField(labelsRect, label, EditorStyles.miniLabel);
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            if (SelectedKeyItems.All(keyItem => keyItem is KnotTreeViewKeyItemGroup) &&
                item is KnotTreeViewKeyItemGroup)
                return true;

            if (SelectedKeyItems.All(keyItem => keyItem is TKeyView) && item is TKeyView)
                return true;

            return false;
        }

        protected sealed override bool CanBeParent(TreeViewItem item)
        {
            return true;
        }
        
        protected sealed override bool CanStartDrag(CanStartDragArgs args)
        {
            return !string.IsNullOrEmpty(DragAndDropKeyName);
        }

        protected sealed override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            string[] keys = FindRows(args.draggedItemIDs).OfType<KnotTreeViewKeyItem>().Select(item => item.Key)
                .ToArray();

            if (!keys.Any())
                return;

            DragAndDrop.PrepareStartDrag();

            DragAndDrop.objectReferences = null;
            DragAndDrop.paths = null;
            DragAndDrop.SetGenericData(DragAndDropKeyName, keys);
            DragAndDrop.StartDrag(DragAndDropKeyName);
        }

        protected override void ContextClickedItem(int id)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(DuplicateKeyContent, false, RequestDuplicateSelected);
            menu.AddItem(RemoveKeyContent, false, RequestRemoveSelected);

            menu.ShowAsContext();
        }

        void RequestRemoveSelected()
        {
            if (!SelectedKeyItems.Any())
                return;

            if (SelectedKeyItems.Any(item => item is KnotTreeViewKeyItemGroup))
            {
                var groupKeys = SelectedKeyItems.OfType<KnotTreeViewKeyItemGroup>().Select(g => g.Key);
                RemoveKeys?.Invoke(AllKeyItems.Where(item => groupKeys.Any(key => item.Key.StartsWith($"{key}."))).OfType<TKeyView>().ToArray());
            }
            else RemoveKeys?.Invoke(SelectedKeyViews.ToArray());
        }

        void RequestDuplicateSelected()
        {
            if (!SelectedKeyItems.Any())
                return;

            if (SelectedKeyItems.Any(item => item is KnotTreeViewKeyItemGroup))
            {
                var groupKeys = SelectedKeyItems.OfType<KnotTreeViewKeyItemGroup>().Select(g => g.Key);
                DuplicateKeys?.Invoke(AllKeyItems.Where(item => groupKeys.Any(key => item.Key.StartsWith(key))).OfType<TKeyView>().ToArray());
            }
            else DuplicateKeys?.Invoke(SelectedKeyItems.OfType<TKeyView>().ToArray());
        }

        protected override void KeyEvent()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.Delete:
                        RequestRemoveSelected();
                        Event.current.Use();
                        break;
                    case KeyCode.D:
                        if (Event.current.control)
                        {
                            RequestDuplicateSelected();
                            Event.current.Use();
                        }
                        break;
                }
            }

            base.KeyEvent();
        }

        protected sealed override bool DoesItemMatchSearch(TreeViewItem item, string search)
        {
            return item is TKeyView keyView && (keyView.IsMatchSearch(search));
        }


        protected override void DoubleClickedItem(int id)
        {
            var item = SelectedKeyItems.FirstOrDefault(i => i.id == id);
            if (item == null)
                return;

            if (item is KnotTreeViewKeyItemGroup)
                SetExpanded(id, !IsExpanded(id));
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            KeysSelected?.Invoke(SelectedKeyViews);
        }

        protected virtual bool AppendContextMenuItems(GenericMenu menu, TKeyView[] selectedKeyViews) => false;

        
        public virtual bool FrameKey(string key, bool select = false)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            var keyView = AllKeyItems.FirstOrDefault(item => item.Key == key);
            if (keyView == null)
                return false;

            FrameItem(keyView.id);

            if (select)
                SetSelection(new List<int> {keyView.id}, TreeViewSelectionOptions.FireSelectionChanged);

            return true;

        }

        public virtual bool FrameKeys(bool select, params string[] keys)
        {
            if (keys.Length == 0)
                return false;

            var keyViews = AllKeyItems.Where(keyView => keys.Contains(keyView.Key)).ToArray();
            if (!keyViews.Any())
                return false;
            
            FrameItem(keyViews.First().id);

            if (select)
                SetSelection(keyViews.Select(item => item.id).ToList(), TreeViewSelectionOptions.FireSelectionChanged);

            return true;
        }

        public virtual void Bind(List<TKeyView> keyViews)
        {
            _keyViews = keyViews;

            Reload();
        }
    }

    public abstract class KnotTreeViewKeyItem : TreeViewItem, IComparable<KnotTreeViewKeyItem>
    {
        public override string displayName
        {
            get
            {
                switch (KnotEditorUtils.UserSettings.KeysTreeViewMode)
                {
                    case KnotKeysTreeViewMode.List:
                        return Key;
                    default:
                        return Key.Split('.').Last();
                }
            }
        }

        public override int id => Key.GetHashCode();
        public override int depth
        {
            get
            {
                switch (KnotEditorUtils.UserSettings.KeysTreeViewMode)
                {
                    case KnotKeysTreeViewMode.Hierarchy:
                        return Mathf.Min(Key.Count(c => c == '.'), 10);
                    default:
                        return 0;
                }
            }
        }

        public abstract string Key { get; } 
        public virtual bool IsMatchSearch(string searchString) => true;

        public abstract KnotKeyData KeyData { get; }


        public virtual int CompareTo(KnotTreeViewKeyItem other) => EditorUtility.NaturalCompare(Key, other.Key);
    }

    public class KnotTreeViewKeyItemGroup : KnotTreeViewKeyItem
    {
        public override Texture2D icon => KnotEditorUtils.GetIcon("Folder Icon") as Texture2D;

        public override KnotKeyData KeyData => null;

        public override string Key => _key;
        private string _key;

        public KnotTreeViewKeyItemGroup(string key)
        {
            _key = key;
        }
    }

    [Serializable]
    public enum KnotKeysTreeViewMode
    {
        Hierarchy,
        List
    }
}
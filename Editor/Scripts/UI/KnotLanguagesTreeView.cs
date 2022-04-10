using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Knot.Localization.Data;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Knot.Localization.Editor
{
    public class KnotLanguagesTreeView : TreeView
    {
        internal static string DragAndDropDataName = $"{KnotEditorUtils.CorePrefix}.LanguageData";

        public event Action<int> Selected;
        public event Action<int> RequestRemove = id => { };
        public event Action<int, int> RequestMove = (id, targetId) => { };

        public KnotDatabase Database
        {
            get => _database;
            set
            {
                _database = value;
                Reload();
            }
        }
        private KnotDatabase _database;


        public int SelectedId
        {
            get => GetSelection().Any() ? GetSelection()[0] : -1;
            set
            {
                if (value >= 0 && value <= Database.Languages.Count - 1)
                    SetSelection(new List<int> {value}, TreeViewSelectionOptions.FireSelectionChanged);
            }
        }

        private List<KnotLanguageTreeViewItem> _items = new List<KnotLanguageTreeViewItem>();


        public KnotLanguagesTreeView(KnotDatabase dataBase, TreeViewState state) : base(state)
        {
            Database = dataBase;
        }


        protected override TreeViewItem BuildRoot()
        {
            TreeViewItem root = new TreeViewItem(-1, -1);

            _items.Clear();
            if (Database != null)
                for (int i = 0; i < Database.Languages.Count; i++)
                    _items.Add(new KnotLanguageTreeViewItem(i, Database.Languages[i]));

            SetupParentsAndChildrenFromDepths(root, _items.Cast<TreeViewItem>().ToList());

            return root;
        }
        
        protected override bool CanMultiSelect(TreeViewItem item) => false;

        protected override bool CanBeParent(TreeViewItem item) => false;

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            Selected(selectedIds.Any() ? selectedIds[0] : -1);
        }

        protected override void ContextClickedItem(int id)
        {
            GenericMenu menu = new GenericMenu();
            
            menu.AddItem(new GUIContent("Remove"), false, () => RequestRemove(id));
            menu.ShowAsContext();
        }

        protected override void KeyEvent()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.Delete && SelectedId >= 0)
                {
                    RequestRemove(SelectedId);
                    Event.current.Use();
                }
            }

            base.KeyEvent();
        }


        protected override bool CanStartDrag(CanStartDragArgs args) => true;

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.SetGenericData(DragAndDropDataName, args.draggedItemIDs[0]);
            DragAndDrop.StartDrag(DragAndDropDataName);
        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            if (DragAndDrop.GetGenericData(DragAndDropDataName) == null)
                return DragAndDropVisualMode.Rejected;

            if (args.dragAndDropPosition != DragAndDropPosition.BetweenItems)
                return DragAndDropVisualMode.Rejected;

            int dragId = DragAndDrop.GetGenericData(DragAndDropDataName) is int ? (int)DragAndDrop.GetGenericData(DragAndDropDataName) : -1;
            int dropId = args.insertAtIndex;

            if (dropId <= dragId - 1 || dropId >= dragId + 2)
            {
                if (args.performDrop)
                    RequestMove(dragId, dropId);

                return DragAndDropVisualMode.Move;
            }

            return DragAndDropVisualMode.Rejected;
        }


        public void SelectLast(bool frame = false)
        {
            SelectedId = _items.Count - 1;
            FrameItem(SelectedId);
        }
    }

    public class KnotLanguageTreeViewItem : TreeViewItem
    {
        public override Texture2D icon => KnotEditorUtils.GetIcon(KnotLanguagesTabPanel.LanguageIconName) as Texture2D;
        public override string displayName => Data.CultureInfo.GetDisplayName();

        public readonly KnotLanguageData Data;

        public KnotLanguageTreeViewItem(int id, KnotLanguageData data) : base(id, 0)
        {
            Data = data;
        }
    }
}


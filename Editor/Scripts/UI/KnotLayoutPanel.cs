using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Knot.Localization.Editor
{
    public abstract class KnotLayoutPanel : KnotEditorPanel
    {
        const float MIN_EDITOR_PANEL_WITH_PERCENT = 20f;
        const float MAX_EDITOR_PANEL_WITH_PERCENT = 80f;

        public readonly Toolbar Toolbar;
        public readonly ToolbarMenu TabOptionsMenu;
        public readonly VisualElement TabContentRoot;

        public VisualElement HorizontalSplitter =>
            _horizontalSplitter ?? (_horizontalSplitter = GetHorizontalSplitter());
        private VisualElement _horizontalSplitter;

        protected abstract VisualElement ItemEditorPanel { get; }

        private float _editorPanelStartDragWidth;
        

        protected KnotLayoutPanel(string className) : base(className)
        {
            Toolbar = Root.Q<Toolbar>(nameof(Toolbar));
            TabOptionsMenu = Toolbar.Q<ToolbarMenu>(nameof(TabOptionsMenu));
            BuildTabOptionsMenu(TabOptionsMenu.menu);
            
            TabContentRoot = Root.Q<VisualElement>(nameof(TabContentRoot));
            TabContentRoot.style.flexBasis = 149600;
            TabContentRoot.RegisterCallback(new EventCallback<GeometryChangedEvent>(evt =>
            {
                if (KnotEditorUtils.UserSettings.PanelLayoutMode == KnotPanelLayoutMode.Auto)
                    SetLayoutMode(GetAutoLayoutMode());
            }));
        }


        protected override void OnPanelAdded()
        {
            ReloadLayout();
            Undo.undoRedoPerformed += ReloadLayout;

            SetLayoutMode(KnotEditorUtils.UserSettings.PanelLayoutMode);
        }

        protected override void OnPanelRemoved()
        {
            Undo.undoRedoPerformed -= ReloadLayout;
        }


        KnotPanelLayoutMode GetAutoLayoutMode()
        {
            return TabContentRoot.contentRect.width > 600f
                ? KnotPanelLayoutMode.Horizontal
                : KnotPanelLayoutMode.Vertical;
        }

        VisualElement GetHorizontalSplitter()
        {
            var s = new VisualElement();
            s.AddToClassList(nameof(HorizontalSplitter));
            s.AddManipulator(new KnotSplitterManipulator(startPos =>
            {
                _editorPanelStartDragWidth = ItemEditorPanel == null ? 0 : ItemEditorPanel.style.width.value.value;
            }, delta =>
            {
                if (ItemEditorPanel == null)
                    return;

                float newWidth = Mathf.Clamp(_editorPanelStartDragWidth - delta.x, 
                    Root.contentRect.width * (MIN_EDITOR_PANEL_WITH_PERCENT / 100f), 
                    Root.contentRect.width * (MAX_EDITOR_PANEL_WITH_PERCENT / 100f));
                ItemEditorPanel.style.width = KnotEditorUtils.UserSettings.LayoutPanelEditorWidth = newWidth;
            }));

            return s;
        }


        protected virtual void SetLayoutMode(KnotPanelLayoutMode mode)
        {
            switch (mode)
            {
                case KnotPanelLayoutMode.Auto:
                    SetLayoutMode(GetAutoLayoutMode());
                    break;
                case KnotPanelLayoutMode.Horizontal:
                    TabContentRoot.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
                    if (ItemEditorPanel != null)
                    {
                        ItemEditorPanel.style.maxHeight = new StyleLength(Length.Percent(100));
                        ItemEditorPanel.style.minWidth = new StyleLength(Length.Percent(MIN_EDITOR_PANEL_WITH_PERCENT));
                        ItemEditorPanel.style.maxWidth = new StyleLength(Length.Percent(MAX_EDITOR_PANEL_WITH_PERCENT));
                        ItemEditorPanel.style.width = KnotEditorUtils.UserSettings.LayoutPanelEditorWidth;
                        ItemEditorPanel.style.borderTopWidth = 0;
                        ItemEditorPanel.style.borderLeftWidth = 1;

                        if (!ItemEditorPanel.Contains(HorizontalSplitter))
                            ItemEditorPanel.Add(HorizontalSplitter);
                    }
                    break;
                case KnotPanelLayoutMode.Vertical:
                    TabContentRoot.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);
                    if (ItemEditorPanel != null)
                    {
                        ItemEditorPanel.style.maxHeight = new StyleLength(Length.Percent(60));
                        ItemEditorPanel.style.maxWidth = ItemEditorPanel.style.minWidth = new StyleLength(Length.Percent(100));
                        ItemEditorPanel.style.borderTopWidth = 1;
                        ItemEditorPanel.style.borderLeftWidth = 0;

                        if (ItemEditorPanel.Contains(HorizontalSplitter))
                            ItemEditorPanel.Remove(HorizontalSplitter);
                    }
                    break;
            }
        }

        protected virtual void BuildTabOptionsMenu(DropdownMenu menu)
        {
            foreach (KnotPanelLayoutMode mode in Enum.GetValues(typeof(KnotPanelLayoutMode)))
            {
                menu.AppendAction($"Layout/{ObjectNames.NicifyVariableName(mode.ToString())}", action =>
                    {
                        SetLayoutMode(KnotEditorUtils.UserSettings.PanelLayoutMode = mode);
                    },
                    action => KnotEditorUtils.UserSettings.PanelLayoutMode == mode
                        ? DropdownMenuAction.Status.Checked
                        : DropdownMenuAction.Status.Normal);
            }
        }
    }

    [Serializable]
    public enum KnotPanelLayoutMode
    {
        Auto,
        Horizontal,
        Vertical
    }
}
using System;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Knot.Localization.Editor
{
    public class KnotKeySearchField<TKeyView> : KnotEditorPanel where TKeyView : KnotTreeViewKeyItem
    {
        private const long INPUT_DELAY = 200;

        public event Action<string> ValueChanged;
        public event Action<IKnotKeySearchFilter<TKeyView>> SearchFilterSelected;

        public IKnotKeySearchFilter<TKeyView> SelectedSearchFilter
        {
            get => _selectedSearchFilter;
            set => (SearchFilterNameLabel as INotifyValueChanged<string>).SetValueWithoutNotify((_selectedSearchFilter = value) == null ? string.Empty : value.Name);
        }
        private IKnotKeySearchFilter<TKeyView> _selectedSearchFilter;

        public new ToolbarPopupSearchField Root => base.Root as ToolbarPopupSearchField;
        public readonly TextField TextField;
        public readonly Label ResultsCountLabel;
        public readonly Label SearchFilterNameLabel;
        
        private IKnotKeySearchFilter<TKeyView>[] _searchFilters = new IKnotKeySearchFilter<TKeyView>[0];
        private IVisualElementScheduledItem _lastInputSchedule;


        public KnotKeySearchField(ToolbarPopupSearchField searchField, params IKnotKeySearchFilter<TKeyView>[] searchFilters) : base(searchField)
        {
            BuildSearchFilterMenu(Root.menu, searchFilters);

            TextField = Root.Q<TextField>();

            Root.RegisterCallback(new EventCallback<KeyDownEvent>(evt =>
            {
                if (evt.ctrlKey && evt.keyCode == KeyCode.DownArrow)
                    Root.ShowMenu();
            }));

            TextField.RegisterValueChangedCallback(evt =>
            {
                string value = evt.newValue;
                _lastInputSchedule?.Pause();
                if (string.IsNullOrEmpty(evt.newValue))
                    ValueChanged?.Invoke(value);
                else _lastInputSchedule = TextField.schedule.Execute(() => ValueChanged?.Invoke(value)).StartingIn(INPUT_DELAY);
            });

            TextField.Add(ResultsCountLabel = GetNewMiniLabel(nameof(ResultsCountLabel)));
            TextField.Add(SearchFilterNameLabel = GetNewMiniLabel(nameof(SearchFilterNameLabel)));
            ResultsCountLabel.BringToFront();
            SearchFilterNameLabel.SendToBack();

            if (Root.Q<Button>() != null)
                Root.Q<Button>().tooltip = "Search Filter";
        }


        void BuildSearchFilterMenu(DropdownMenu menu, IKnotKeySearchFilter<TKeyView>[] searchFilters)
        {
            menu.AppendAction("All", action => SearchFilterSelected?.Invoke(SelectedSearchFilter = null),
                action => SelectedSearchFilter == null ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
            menu.AppendSeparator();
            foreach (var filter in searchFilters.Where(f => f != null))
                menu.AppendAction(filter.Name, action => SearchFilterSelected?.Invoke(SelectedSearchFilter = filter),
                    action => filter == SelectedSearchFilter ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
        }

        Label GetNewMiniLabel(string name)
        {
            var label = new Label {name = name};
            label.AddToClassList("MiniLabel");
            label.style.alignSelf = new StyleEnum<Align>(Align.Center);

            return label;
        }


        public void SetResultsCount(int current, int total)
        {
            (ResultsCountLabel as INotifyValueChanged<string>).SetValueWithoutNotify($"{current}/{total}");
        }
    }
}
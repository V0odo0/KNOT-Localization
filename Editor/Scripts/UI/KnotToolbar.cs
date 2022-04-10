using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Knot.Localization.Editor
{
    public class KnotToolbar : KnotEditorPanel
    {
        public event Action<int> TabSelected;


        public int SelectedTabId
        {
            get => Mathf.Clamp(_selectedTabId, 0, Tabs.Count - 1);
            set
            {
                if (_selectedTabId == value)
                    return;

                _selectedTabId = Mathf.Clamp(value, 0, Tabs.Count - 1);
                UpdateSelectedTab();
            }
        }
        private int _selectedTabId;

        public ObservableCollection<GUIContent> Tabs
        {
            get => _tabs;
            set
            {
                if (value == _tabs)
                    return;

                if (value == null)
                    _tabs.Clear();
                else _tabs = value;

                BuildTabs();
            }
        }
        private ObservableCollection<GUIContent> _tabs;


        private List<Toggle> _toggles = new List<Toggle>();

        
        public KnotToolbar(Toolbar element, params GUIContent[] tabs) : base(element)
        {
            Tabs = new ObservableCollection<GUIContent>(tabs);
            Tabs.CollectionChanged += (sender, args) => { BuildTabs(); };
            
            BuildTabs();
        }

        
        ToolbarToggle GetNewToggle(int tabId)
        {
            var toggle = new ToolbarToggle();
            toggle.AddToClassList("ToolbarToggle");

            toggle.text = Tabs[tabId].text;


            if (Tabs[tabId].image == null)
            {
                if (toggle.Q("unity-checkmark") != null)
                    toggle.Q("unity-checkmark").style.maxWidth = 0f;
            }
            else toggle.SetIcon(Tabs[tabId].image);

            toggle.style.borderTopLeftRadius = tabId == 0 ? 3 : 0;
            toggle.style.borderTopRightRadius = tabId == Tabs.Count - 1 ? 3 : 0;

            toggle.RegisterValueChangedCallback(evt =>
            {
                if (!evt.newValue)
                    toggle.SetValueWithoutNotify(true);

                if (SelectedTabId == tabId)
                    return;

                SelectedTabId = tabId;
                TabSelected?.Invoke(tabId);
            });
            toggle.SetValueWithoutNotify(SelectedTabId == tabId);

            return toggle;
        }
        
        void BuildTabs()
        {
            foreach (var toggle in _toggles.Where(t => t != null && t.parent != null))
                toggle.parent.Remove(toggle);
            _toggles.Clear();

            for (int i = 0; i < Tabs.Count; i++)
            {
                var toggle = GetNewToggle(i);

                _toggles.Add(toggle);
                Root.Add(toggle);
            }

            UpdateSelectedTab();
        }

        void UpdateSelectedTab()
        {
            for (int i = 0; i < _toggles.Count; i++)
                _toggles[i]?.SetValueWithoutNotify(SelectedTabId == i);
        }
    }
}
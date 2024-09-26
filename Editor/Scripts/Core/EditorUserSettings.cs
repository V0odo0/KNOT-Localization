using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Knot.Localization.Editor
{
    [Serializable]
    internal class EditorUserSettings
    {
        internal const int Version = 1;
        internal static string ConfigValueName = $"{EditorUtils.CorePrefix}.UserSettings";

        public int CurrentVersion = Version;

        public string LastActiveDatabaseGuid = string.Empty;

        public KnotPanelLayoutMode PanelLayoutMode = KnotPanelLayoutMode.Auto;
        public KnotKeysTreeViewMode KeysTreeViewMode = KnotKeysTreeViewMode.Hierarchy;

        public float LayoutPanelEditorWidth
        {
            get => _layoutPanelEditorWidth;
            set => _layoutPanelEditorWidth = value;
        }
        [SerializeField] private float _layoutPanelEditorWidth = 300f;

        public TreeViewState LanguagesTreeViewState = new TreeViewState();
        public TreeViewState TextKeysTreeViewState = new TreeViewState();
        public TreeViewState AssetKeysTreeViewState = new TreeViewState();

        public KnotTypeReference EditorTabType => _editorTabType;
        [SerializeField] private KnotTypeReference _editorTabType = new KnotTypeReference();

        public KnotTypeReference TextKeySearchFilterType => _textKeySearchFilterType;
        [SerializeField] private KnotTypeReference _textKeySearchFilterType = new KnotTypeReference();

        public KnotTypeReference AssetKeySearchFilterType => _assetKeySearchFilterType;
        [SerializeField] private KnotTypeReference _assetKeySearchFilterType = new KnotTypeReference();

        public IReadOnlyList<string> ExpandedFoldouts => _expandedFoldouts;
        [SerializeField] private List<string> _expandedFoldouts = new List<string>();

        
        public void Save()
        {
            UnityEditor.EditorUserSettings.SetConfigValue(ConfigValueName, JsonUtility.ToJson(this));
        }


        public bool GetFoldoutState(string name)
        {
            return ExpandedFoldouts.Contains(name);
        }

        public void SetFoldoutState(string name, bool state)
        {
            if (state && !ExpandedFoldouts.Contains(name))
                _expandedFoldouts.Add(name);
            else if (!state && ExpandedFoldouts.Contains(name))
                _expandedFoldouts.Remove(name);
        }


        public static EditorUserSettings Load()
        {
            return string.IsNullOrEmpty(UnityEditor.EditorUserSettings.GetConfigValue(ConfigValueName)) ? 
                new EditorUserSettings() : 
                JsonUtility.FromJson<EditorUserSettings>(UnityEditor.EditorUserSettings.GetConfigValue(ConfigValueName));
        }
    }
}
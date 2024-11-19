using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Knot.Localization.Attributes;
using Knot.Localization.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Knot.Core.Editor;
using Object = UnityEngine.Object;


namespace Knot.Localization.Editor
{
    internal static class EditorUtils
    {
        public static Action DefaultDatabaseChanged;
        
        public const string CorePrefix = "KnotLocalization";
        public const string ToolsRootPath = "Tools/" + KnotLocalization.CoreName + "/";

        const string EditorStylesResourcesPath = "UI/KnotEditorStyles";
        
        public static IReadOnlyDictionary<KnotMetadataInfoAttribute.MetadataScope, EditorExtensions.TypeInfo[]> MetadataTypes
        {
            get
            {
                if (_metadataTypes == null)
                {
                    _metadataTypes = new Dictionary<KnotMetadataInfoAttribute.MetadataScope, EditorExtensions.TypeInfo[]>();

                    var allTypesInfo = typeof(IKnotMetadata).GetDerivedTypesInfo();
                    var allScopes = (KnotMetadataInfoAttribute.MetadataScope[]) Enum.GetValues(typeof(KnotMetadataInfoAttribute.MetadataScope));
                    foreach (var scope in allScopes)
                    {
                        List<EditorExtensions.TypeInfo> scopeTypesInfo = new List<EditorExtensions.TypeInfo>();

                        foreach (var typeInfo in allTypesInfo)
                        {
                            var metadataAttribute = typeInfo.Type.GetCustomAttribute<KnotMetadataInfoAttribute>();
                            if (metadataAttribute != null && metadataAttribute.Scope.HasFlag(scope))
                                scopeTypesInfo.Add(typeInfo);
                        }

                        _metadataTypes.Add(scope, scopeTypesInfo.ToArray());
                    }
                }
                return _metadataTypes;
            }
        }
        private static Dictionary<KnotMetadataInfoAttribute.MetadataScope, EditorExtensions.TypeInfo[]> _metadataTypes;
        

        public static EditorUserSettings UserSettings => 
            _userSettings ?? (_userSettings = EditorUserSettings.Load());
        private static EditorUserSettings _userSettings;


        public static Dictionary<string, VisualTreeAsset> EditorPanels
        {
            get
            {
                if (_editorPanels == null || !_editorPanels.Any())
                {
                    _editorPanels = new Dictionary<string, VisualTreeAsset>();
                    foreach (var visualTree in Resources.LoadAll<VisualTreeAsset>("UI").GroupBy(a => a.name).Select(g => g.First()))
                        _editorPanels.Add(visualTree.name, visualTree);
                }
                return _editorPanels;
            }
        }
        private static Dictionary<string, VisualTreeAsset> _editorPanels;

        public static StyleSheet EditorStyles => 
            _editorStyles == null ? _editorStyles = Resources.Load(EditorStylesResourcesPath) as StyleSheet : _editorStyles;
        private static StyleSheet _editorStyles;

        internal static Texture CoreIcon => Core.Editor.EditorUtils.GetIcon("KnotLocalization_icon");

        /// <summary>
        /// Current or last opened <see cref="KnotDatabase"/> in <see cref="KnotDatabaseEditorWindow"/>
        /// </summary>
        public static KnotDatabase ActiveDatabase
        {
            get => _activeDatabase == null ?
                _activeDatabase = AssetDatabase.LoadAssetAtPath<KnotDatabase>(AssetDatabase.GUIDToAssetPath(UserSettings.LastActiveDatabaseGuid)) :
                _activeDatabase;
            set
            {
                if (value == null || value == _activeDatabase)
                    return;

                UserSettings.LastActiveDatabaseGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value));
                _activeDatabase = value;
            }
        }
        private static KnotDatabase _activeDatabase;

        /// <summary>
        /// Cached list of all project's <see cref="KnotDatabase"/> assets
        /// </summary>
        public static KnotDatabase[] DatabaseAssets
        {
            get
            {
                if (_databaseAssets == null)
                    UpdateDatabaseAssets();

                return _databaseAssets != null && _databaseAssets.Any(db => db == null) ?
                    _databaseAssets = _databaseAssets.Where(db => db != null).ToArray() : _databaseAssets;
            }
        }
        private static KnotDatabase[] _databaseAssets;


        public static void UpdateDatabaseAssets()
        {
            _databaseAssets =
                AssetDatabase.FindAssets($"t:{nameof(KnotDatabase)}").
                    Select(AssetDatabase.GUIDToAssetPath).
                    Select(AssetDatabase.LoadAssetAtPath<KnotDatabase>).ToArray();
        }

        public static void OpenDatabaseEditor(KnotDatabase database = null)
        {
            if (database != null && database.IsPersistent())
                ActiveDatabase = database;

            KnotDatabaseEditorWindow.Open();
        }


        [InitializeOnLoadMethod]
        static void Init()
        {
            AssemblyReloadEvents.beforeAssemblyReload += SaveAllSettings;
            EditorApplication.quitting += SaveAllSettings;

            if (KnotLocalization.ProjectSettings == KnotProjectSettings.Empty)
            {
                //Ensure that Project Settings is created
            }
        }

        static void SaveAllSettings()
        {
            _userSettings?.Save();
        }

        public static bool RecordObjects(string commandName, Action postRecordAction = null, params Object[] obj)
        {
            if (obj == null || obj.Length == 0)
                return false;
            
            obj = obj.Where(o => o != null).ToArray();

            Undo.RecordObjects(obj, $"{CorePrefix}: {commandName}");
            postRecordAction?.Invoke();
            foreach (var o in obj)
                EditorUtility.SetDirty(o);

            return true;
        }

        public static bool RegisterCompleteObjects(string commandName, Action postRegisterAction = null, params Object[] obj)
        {
            if (obj == null || obj.Length == 0)
                return false;

            obj = obj.Where(o => o != null).ToArray();

            Undo.RegisterCompleteObjectUndo(obj, $"{CorePrefix}: {commandName}");
            postRegisterAction?.Invoke();
            foreach (var o in obj)
                EditorUtility.SetDirty(o);

            return true;
        }

        public static void PerformPlayModeLiveReload()
        {
            if (!Application.isPlaying)
                return;

            if (KnotLocalization.Manager.State == KnotManagerState.LanguageLoaded)
                KnotLocalization.Manager.LoadLanguage(KnotLocalization.Manager.Languages.FirstOrDefault());
        }
    }
}
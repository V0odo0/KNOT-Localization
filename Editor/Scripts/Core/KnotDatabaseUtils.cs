using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Knot.Localization.Data;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using Object = UnityEngine.Object;
using PlayerSettings = UnityEditor.PlayerSettings;

namespace Knot.Localization.Editor
{
    /// <summary>
    /// Various utilities for <see cref="KnotDatabase"/> asset
    /// </summary>
    public static class KnotDatabaseUtils
    {
        public static Action DefaultDatabaseChanged;
        
        /// <summary>
        /// Current or last opened <see cref="KnotDatabase"/> in <see cref="KnotDatabaseEditorWindow"/>
        /// </summary>
        public static KnotDatabase ActiveDatabase
        {
            get => _activeDatabase == null ? 
                _activeDatabase = AssetDatabase.LoadAssetAtPath<KnotDatabase>(AssetDatabase.GUIDToAssetPath(KnotEditorUtils.UserSettings.LastActiveDatabaseGuid)) : 
                _activeDatabase;
            set
            {
                if (value == null || value == _activeDatabase)
                    return;

                KnotEditorUtils.UserSettings.LastActiveDatabaseGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value));
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
    }
}
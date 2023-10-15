using System;
using System.Linq;
using Knot.Localization.Data;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Knot.Localization
{
    /// <summary>
    /// Core static class that keeps references to project wide <see cref="KnotProjectSettings"/> and application wide <see cref="IKnotManager"/>
    /// </summary>
    public static class KnotLocalization
    {
        internal const string CoreName = "KNOT Localization";
        internal const string CorePath = "KNOT/Localization/";
        
        public static KnotProjectSettings ProjectSettings =>
            _projectSettings ?? (_projectSettings = LoadProjectSettings());
        private static KnotProjectSettings _projectSettings;

        public static IKnotManager Manager => _manager ?? (_manager = ProjectSettings.Manager ?? new KnotManager());
        private static IKnotManager _manager;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Init()
        {
            if (ProjectSettings.LoadOnStartup && ProjectSettings.DefaultDatabase != KnotDatabase.Empty)
                Manager.SetDatabase(ProjectSettings.DefaultDatabase, true);
        }

        
        static KnotProjectSettings LoadProjectSettings()
        {
            KnotProjectSettings settings;

#if UNITY_EDITOR
            var allSettings =
                AssetDatabase.FindAssets($"t:{nameof(KnotProjectSettings)}").
                    Select(AssetDatabase.GUIDToAssetPath).
                    Select(AssetDatabase.LoadAssetAtPath<KnotProjectSettings>).ToArray();

            if (allSettings.Length == 0)
            {
                string path = $"Assets/{nameof(KnotProjectSettings)}.asset";
                settings = AssetDatabase.LoadAssetAtPath<KnotProjectSettings>(path);
                
                if (settings == null)
                    settings = PlayerSettings.GetPreloadedAssets().OfType<KnotProjectSettings>().FirstOrDefault();

                if (settings == null)
                {
                    var instance = ScriptableObject.CreateInstance<KnotProjectSettings>();
                    AssetDatabase.CreateAsset(instance, path);
                    AssetDatabase.SaveAssets();
                    settings = instance;

                    var preloadedAssets = PlayerSettings.GetPreloadedAssets();
                    PlayerSettings.SetPreloadedAssets(preloadedAssets.Append(settings).ToArray());
                }
            }
            else settings = allSettings.First();
#else
            settings = Resources.FindObjectsOfTypeAll<KnotProjectSettings>().FirstOrDefault();
#endif

            if (settings == null)
            {
                settings = KnotProjectSettings.Empty;
                Log("Unable to load or create Project Settings. Empty Project Settings will be assigned.", LogType.Warning);
            }
            return settings;
        }


        internal static void Log(object message, LogType type, Object context = null)
        {
            message = $"{CoreName}: {message}";

            switch (type)
            {
                default:
                    Debug.Log(message, context);
                    break;
                case LogType.Error:
                    Debug.LogError(message, context);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(message, context);
                    break;
            }
        }


        /// <summary>
        /// A shortcut for <see cref="IKnotManager.GetTextValue"/>. Returns text assigned to <paramref name="key"/> from <see cref="Manager"/>
        /// </summary>
        public static string GetText(string key) => Manager.GetTextValue(key)?.Value;

        /// <summary>
        /// A shortcut for <see cref="IKnotManager.GetAssetValue"/>. Returns asset assigned to <paramref name="key"/> from <see cref="Manager"/>
        /// </summary>
        public static Object GetAsset(string key) => Manager.GetAssetValue(key)?.Value;


        /// <summary>
        /// Registers <paramref name="textUpdated"/> callback that is invoked when text with given <paramref name="key"/> has been updated
        /// </summary>
        public static void RegisterTextUpdatedCallback(string key, Action<string> textUpdated) =>
            Manager.TextController.RegisterValueChangedCallback(key, textUpdated);

        /// <summary>
        /// Registers <paramref name="assetUpdated"/> callback that is invoked when asset with given <paramref name="key"/> has been updated
        /// </summary>
        public static void RegisterAssetUpdatedCallback(string key, Action<Object> assetUpdated) =>
            Manager.AssetController.RegisterValueChangedCallback(key, assetUpdated);

        /// <summary>
        /// Removes <paramref name="textUpdated"/> callback for given <paramref name="key"/>
        /// </summary>
        public static void UnRegisterTextUpdatedCallback(string key, Action<string> textUpdated) =>
            Manager.TextController.UnRegisterValueChangedCallback(key, textUpdated);

        /// <summary>
        /// Removes <paramref name="assetUpdated"/> callback for given <paramref name="key"/>
        /// </summary>
        public static void UnRegisterAssetUpdatedCallback(string key, Action<Object> assetUpdated) =>
            Manager.AssetController.UnRegisterValueChangedCallback(key, assetUpdated);
    }
}
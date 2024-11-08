using System;
using Knot.Core;
using Knot.Localization.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Knot.Localization
{
    /// <summary>
    /// Core static class that keeps references to project wide <see cref="KnotProjectSettings"/> and application wide <see cref="IKnotManager"/>
    /// </summary>
    public static class KnotLocalization
    {
        internal const string CoreName = "KNOT Localization";
        internal const string CorePath = Utils.EditorRootPath + "Localization/";

        internal static KnotProjectSettings ProjectSettings =>
            _projectSettings == null ? _projectSettings = Utils.GetProjectSettings<KnotProjectSettings>() : _projectSettings;
        private static KnotProjectSettings _projectSettings;

        public static IKnotManager Manager => _manager ?? (_manager = ProjectSettings.Manager ?? new KnotManager());
        private static IKnotManager _manager;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Init()
        {
            if (ProjectSettings.LoadOnStartup && ProjectSettings.DefaultDatabase != KnotDatabase.Empty)
                Manager.SetDatabase(ProjectSettings.DefaultDatabase, true);
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
        /// A shortcut for <see cref="IKnotManager.GetTextValue"/>. Returns current language  localized text assigned to <paramref name="key"/> from <see cref="Manager"/>
        /// </summary>
        public static string GetText(string key) => Manager.GetTextValue(key)?.Value;

        /// <summary>
        /// A shortcut for <see cref="IKnotManager.GetTextValue"/>. Returns current language  localized text assigned to <paramref name="key"/> from <see cref="Manager"/> with <see cref="KnotPluralForm"/>
        /// </summary>
        public static string GetText(string key, KnotPluralForm pluralForm) => Manager.GetTextValue(key, pluralForm)?.Value;

#if !KNOT_LOCALIZATION_DISABLE_EXTENSIONS
        /// <summary>
        /// Extensions method that returns current language localized text assigned to <paramref name="key"/> from <see cref="Manager"/>
        /// </summary>
        public static string Localize(this string key) => Manager.GetTextValue(key)?.Value;

        /// <summary>
        /// Extensions method that returns current language localized text assigned to <paramref name="key"/> from <see cref="Manager"/> for <see cref="KnotPluralForm"/>
        /// </summary>
        public static string Localize(this string key, KnotPluralForm pluralForm) => Manager.GetTextValue(key, pluralForm)?.Value;
#endif

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
using Knot.Core;
using Knot.Core.Editor;
using UnityEditor;

namespace Knot.Localization.Editor
{
    [CustomEditor(typeof(KnotProjectSettings))]
    internal class KnotProjectSettingsEditor : ProjectSettingsEditor<KnotProjectSettings>
    {
        public static string SettingsPath => GetSettingsPath("Localization");

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Target.LoadOnStartup)
                EditorGUILayout.HelpBox("KnotLocalization.Manager.SetDatabase() should be called manually", MessageType.Info);
        }


        [SettingsProvider]
        static SettingsProvider GetSettingsProvider()
        {
            return GetSettingsProvider(KnotLocalization.ProjectSettings, SettingsPath,
                typeof(KnotProjectSettingsEditor));
        }

        public static void Open()
        {
            SettingsService.OpenProjectSettings(SettingsPath);
        }
    }
}
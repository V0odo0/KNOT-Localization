using UnityEditor;
using UnityEngine;

namespace Knot.Localization.Editor
{
    [CustomEditor(typeof(KnotProjectSettings))]
    public class KnotProjectSettingsEditor : UnityEditor.Editor
    {
        internal static string SettingsPath = $"Project/{KnotLocalization.CoreName}";
        internal static string[] DefaultKeyWords = 
        {
            "knot",
            "default",
            "database",
            "localization",
            "language",
            "globalization"
        };


        public override void OnInspectorGUI()
        {
            if (target == null)
                return;

            serializedObject.Update();
            SerializedProperty property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                do
                {
                    if (property.name == "m_Script")
                        continue;

                    EditorGUILayout.PropertyField(serializedObject.FindProperty(property.name), true);
                }
                while (property.NextVisible(false));
            }
            serializedObject.ApplyModifiedProperties();

            GUILayout.Space(15);
            if (GUILayout.Button("Reset User Settings"))
            {
                if (EditorUtility.DisplayDialog("Reset User Settings",
                    "Are you sure you want to reset User Settings? You cannot undo this action.", "Yes", "No"))
                {
                    KnotEditorUtils.ResetUserSettings();
                }
            }
        }


        [SettingsProvider]
        static SettingsProvider GetSettingsProvider()
        {
            var provider = new SettingsProvider(SettingsPath, SettingsScope.Project, DefaultKeyWords);
            var editor = CreateEditor(KnotLocalization.ProjectSettings);
            provider.guiHandler += s =>
            {
                editor.OnInspectorGUI();
            };

            return provider;
        }


        public static void Open()
        {
            SettingsService.OpenProjectSettings(SettingsPath);
        }
    }
}
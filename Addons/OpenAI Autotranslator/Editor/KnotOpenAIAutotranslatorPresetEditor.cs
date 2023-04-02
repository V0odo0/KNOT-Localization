using System;
using System.Linq;
using Knot.Localization.Data;
using UnityEditor;
using UnityEngine;

namespace Knot.Localization.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(KnotOpenAIAutotranslatorPreset))]
    public class KnotOpenAIAutotranslatorPresetEditor : UnityEditor.Editor
    {
        private KnotOpenAIAutotranslatorPreset _preset;

        private string _apiKey;
        private bool _useCustomSettings;
        private KnotOpenAIAutotranslator.TranslationSettings _customSettings = new KnotOpenAIAutotranslator.TranslationSettings();


        void OnEnable()
        {
            if (targets.Length > 1)
                return;

            _apiKey = EditorPrefs.GetString($"{nameof(KnotOpenAIAutotranslatorPresetEditor)}.{nameof(_apiKey)}");
            _useCustomSettings = EditorPrefs.GetBool($"{nameof(KnotOpenAIAutotranslatorPresetEditor)}.{nameof(_useCustomSettings)}");
            var savedCustomSettings = JsonUtility.FromJson<KnotOpenAIAutotranslator.TranslationSettings>(
                EditorPrefs.GetString($"{nameof(KnotOpenAIAutotranslatorPresetEditor)}.{nameof(_customSettings)}"));
            if (savedCustomSettings != null)
                _customSettings = savedCustomSettings;
            
            _preset = target as KnotOpenAIAutotranslatorPreset;
        }

        void OnDisable()
        {
            if (targets.Length > 1)
                return;

            EditorPrefs.SetString($"{nameof(KnotOpenAIAutotranslatorPresetEditor)}.{nameof(_apiKey)}", _apiKey);
            EditorPrefs.SetBool($"{nameof(KnotOpenAIAutotranslatorPresetEditor)}.{nameof(_useCustomSettings)}", _useCustomSettings);
            EditorPrefs.SetString($"{nameof(KnotOpenAIAutotranslatorPresetEditor)}.{nameof(_customSettings)}", JsonUtility.ToJson(_customSettings));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (targets.Length > 1)
                return;

            DrawSeparator();

            _apiKey = EditorGUILayout.PasswordField("API Key", _apiKey);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(" ");
            EditorGUILayout.HelpBox("Your API key will be stored in EditorPrefs", MessageType.None);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(" ");
            if (GUILayout.Button("Manage my API keys", GUILayout.ExpandWidth(true)))
                Application.OpenURL(KnotOpenAIAutotranslator.ManagerMyApiKeysUrl);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            _useCustomSettings = EditorGUILayout.Toggle("Override Request Settings", _useCustomSettings);
            EditorGUI.BeginDisabledGroup(!_useCustomSettings);
            DrawSettings(_useCustomSettings ? _customSettings : KnotOpenAIAutotranslator.DefaultSettings);
            EditorGUI.EndDisabledGroup();
            
            DrawSeparator();

            bool canStartTranslation = !KnotOpenAIAutotranslator.IsTranslating &&
                                       _preset.TranslationSource.TextCollection != null &&
                                       !string.IsNullOrEmpty(_apiKey) &&
                                       _preset.TranslationSource.TextCollection.Count != 0 &&
                                       _preset.TranslationTargets.Any(t => t.TextCollection != null);

            
            EditorGUI.BeginDisabledGroup(!canStartTranslation);
            if (GUILayout.Button("START TRANSLATION", GUILayout.Height(30)))
            {
                var request = new KnotOpenAIAutotranslator.TranslationRequest(_apiKey, _preset, _useCustomSettings ? _customSettings : null);
                KnotOpenAIAutotranslator.StartTranslation(request);
            }
            EditorGUI.EndDisabledGroup();
        }

        void DrawSettings(KnotOpenAIAutotranslator.TranslationSettings settings)
        {
            EditorGUI.indentLevel += 1;
            settings.CompletionEndpointUrl = EditorGUILayout.TextField(ObjectNames.NicifyVariableName(nameof(settings.CompletionEndpointUrl)),
                settings.CompletionEndpointUrl);
            settings.CompletionModel = EditorGUILayout.TextField(ObjectNames.NicifyVariableName(nameof(settings.CompletionModel)), settings.CompletionModel);
            settings.CompletionPrompt = EditorGUILayout.TextField(ObjectNames.NicifyVariableName(nameof(settings.CompletionPrompt)), settings.CompletionPrompt);
            settings.RequestTimeout = EditorGUILayout.IntField(ObjectNames.NicifyVariableName(nameof(settings.RequestTimeout)), settings.RequestTimeout);
            settings.MaxCharactersPerRequest = EditorGUILayout.IntField(ObjectNames.NicifyVariableName(nameof(settings.MaxCharactersPerRequest)), settings.MaxCharactersPerRequest);

            EditorGUI.indentLevel -= 1;
        }

        void DrawSeparator()
        {
            EditorGUILayout.Separator();
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
            EditorGUILayout.Separator();
        }
    }
}
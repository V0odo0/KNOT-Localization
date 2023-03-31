using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Knot.Localization.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Knot.Localization.Editor
{
    public static class KnotOpenAIAutotranslator
    {
        internal const string CoreName = "Open AI Autotranslator";

        public static bool IsTranslating { get; private set; }
        public const string ManagerMyApiKeysUrl = "https://platform.openai.com/account/api-keys";
        public static readonly TranslationSettings DefaultSettings = new TranslationSettings();
        

        public static async void StartTranslation(TranslationRequest request)
        {
            request.Settings ??= DefaultSettings;

            var targets = request.Preset.TranslationTargets.Where(t => t.TextCollection != null && t.CultureInfo != null).ToArray();
            if (targets.Length == 0)
            {
                KnotLocalization.Log("No translation targets", LogType.Warning, request.Preset);
                return;
            }

            IsTranslating = true;

            var keyTextArrays = BuildKeyTextArrays(request.Preset);
            var keysToTranslate = new HashSet<string>(keyTextArrays.SelectMany(a => a.Entries).Select(e => e.Key));
            int totalKeysCount = keysToTranslate.Count;
            var results = new List<KeyValuePair<KnotOpenAIAutotranslatorPreset.TranslationTargetEntry, Dictionary<string, string>>>();
            var cancellationToken = new CancellationTokenSource();
            float progress = 0.1f;
            foreach (var target in targets)
            {
                try
                {
                    var progressBarMsg = $"Translating {totalKeysCount} keys from {request.Preset.TranslationSource.CultureInfo.EnglishName} to {target.CultureInfo.EnglishName}...";

                    var task = TranslateArrays(request, target, cancellationToken.Token, keyTextArrays);
                    while (!task.IsCompleted)
                    {
                        if (EditorUtility.DisplayCancelableProgressBar(CoreName, progressBarMsg, progress))
                        {
                            EditorUtility.ClearProgressBar();
                            cancellationToken.Cancel();
                        }

                        await Task.Delay(100, cancellationToken.Token);
                    }

                    if (task.Result != null)
                        results.Add(new KeyValuePair<KnotOpenAIAutotranslatorPreset.TranslationTargetEntry,
                            Dictionary<string, string>>(target, task.Result));

                    progress += 1f / targets.Length;
                }
                catch (Exception e)
                {
                    EditorUtility.ClearProgressBar();
                    cancellationToken.Cancel();
                    if (!(e is TaskCanceledException))
                        KnotLocalization.Log(e, LogType.Error, request.Preset);
                    break;
                }
            }

            IsTranslating = false;

            EditorUtility.ClearProgressBar();
            if (cancellationToken.IsCancellationRequested || !results.Any()) 
                return;

            ApplyTranslation(keysToTranslate, results);

            KnotLocalization.Log("Translation complete. You can undo this action.", LogType.Log, request.Preset);
        }

        static async Task<Dictionary<string, string>> TranslateArrays(TranslationRequest request, 
            KnotOpenAIAutotranslatorPreset.TranslationTargetEntry target, CancellationToken cancellationToken, 
            params KeyTextArray[] keyTextArrays)
        {
            if (keyTextArrays.Length == 0)
                return null;

            Dictionary<string, string> result = null;
            StringBuilder prompt = new StringBuilder();
            foreach (var keyTextArray in keyTextArrays)
            {
                prompt.Clear();
                prompt.Append(string.Format(request.Settings.CompletionPrompt,
                    request.Preset.TranslationSource.CultureName, target.CultureName));
                prompt.AppendLine(JsonUtility.ToJson(keyTextArray));
                if (!string.IsNullOrEmpty(target.TranslationExtraContext))
                {
                    prompt.AppendLine("Translation context:");
                    prompt.AppendLine(target.TranslationExtraContext);
                }

                var requestBody = new Request(prompt.ToString());
                var webRequest = BuildWebRequest(request.ApiKey, JsonUtility.ToJson(requestBody), request.Settings);
                var requestHandler = webRequest.SendWebRequest();
                requestHandler.completed += asyncOperation =>
                {
                    var op = (UnityWebRequestAsyncOperation)asyncOperation;
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    if (op.webRequest.result != UnityWebRequest.Result.Success)
                    {
                        KnotLocalization.Log(op.webRequest.error, LogType.Warning);
                        return;
                    }

                    var response = JsonUtility.FromJson<Response>(op.webRequest.downloadHandler.text);
                    if (response == null)
                    {
                        KnotLocalization.Log($"Unable to parse response for {target.CultureInfo.EnglishName}", LogType.Warning);
                        return;
                    }

                    result = new Dictionary<string, string>();
                    FillResultFromResponse(result, response, target);
                };

                while (!requestHandler.isDone)
                {
                    await Task.Delay(100);

                    if (cancellationToken.IsCancellationRequested)
                    {
                        webRequest.Abort();
                        webRequest.Dispose();
                        return null;
                    }
                }
            }

            return result;
        }

        static void FillResultFromResponse(Dictionary<string, string> result, Response response, KnotOpenAIAutotranslatorPreset.TranslationTargetEntry target)
        {
            if (response == null || response.Choices == null || response.Choices.Length == 0)
            {
                KnotLocalization.Log($"No response choices for {target.CultureInfo.EnglishName}", LogType.Warning);
                return;
            }

            var msg = response.Choices[0].Message;
            var jsonMatch = Regex.Match(msg, "\\{(.|\\s)*\\}");

            if (!jsonMatch.Success)
            {
                KnotLocalization.Log($"Could not extract JSON from response for {target.CultureInfo.EnglishName}", LogType.Warning);
                return;
            }

            var responseArray = JsonUtility.FromJson<KeyTextArray>(jsonMatch.ToString());
            if (responseArray == null || responseArray.Entries == null || responseArray.Entries.Count == 0)
            {
                KnotLocalization.Log($"Invalid JSON from response for {target.CultureInfo.EnglishName}", LogType.Warning);
                return;
            }

            foreach (var e in responseArray.Entries)
            {
                if (result.ContainsKey(e.Key))
                    result[e.Key] = e.Text;
                else result.Add(e.Key, e.Text);
            }
        }

        static void ApplyTranslation(HashSet<string> keysToTranslate, 
            List<KeyValuePair<KnotOpenAIAutotranslatorPreset.TranslationTargetEntry, Dictionary<string, string>>> results)
        {
            Undo.RegisterCompleteObjectUndo(results.Select(p => p.Key.TextCollection).ToArray(), CoreName);

            foreach (var r in results)
            {
                foreach (var t in r.Value)
                {
                    if (!keysToTranslate.Contains(t.Key))
                        continue;

                    var existingData = r.Key.TextCollection.FirstOrDefault(d => d.Key == t.Key);
                    if (existingData != null)
                        existingData.RawText = t.Value;
                    else r.Key.TextCollection.Add(new KnotTextData(t.Key, t.Value));
                }
            }

            if (EditorWindow.HasOpenInstances<KnotDatabaseEditorWindow>())
                EditorWindow.GetWindow<KnotDatabaseEditorWindow>().ReloadLayout();
        }

        static KeyTextArray[] BuildKeyTextArrays(KnotOpenAIAutotranslatorPreset preset)
        {
            List<KeyTextArray> arrays = new List<KeyTextArray>();

            var array = new KeyTextArray();
            var keys = new HashSet<string>();
            foreach (var text in preset.TranslationSource.TextCollection)
            {
                if (keys.Contains(text.Key) || preset.ExcludeKeys.Contains(text.Key) || string.IsNullOrEmpty(text.RawText))
                    continue;

                var entry = new KeyTextEntry(text.Key, text.RawText);
                array.Entries.Add(entry);
                keys.Add(entry.Key);
            }

            arrays.Add(array);

            return arrays.ToArray();
        }

        static UnityWebRequest BuildWebRequest(string apiKey, string body, TranslationSettings settings)
        {
            var webRequest = UnityWebRequest.Put(settings.CompletionEndpointUrl, body);
            webRequest.timeout = settings.RequestTimeout;
            webRequest.method = "POST";
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", $"Bearer {apiKey}");

            return webRequest;
        }


        public struct TranslationRequest
        {
            public readonly string ApiKey;
            public readonly KnotOpenAIAutotranslatorPreset Preset;
            public TranslationSettings Settings;

            public TranslationRequest(string apiKey, KnotOpenAIAutotranslatorPreset preset, TranslationSettings settings = null)
            {
                ApiKey = apiKey;
                Preset = preset;
                Settings = settings;
            }
        }

        [Serializable]
        public class TranslationSettings
        {
            public string CompletionEndpointUrl = "https://api.openai.com/v1/chat/completions";
            public string CompletionModel = "gpt-3.5-turbo";
            public string CompletionPrompt = "Translate the following JSON file from {0} to {1}. Do not translate JSON \"Key\" value and keep it as is.";

            public int RequestTimeout = 15;
            public int MaxSymbolsPerRequest = 5000;
        }

        [Serializable]
        public class KeyTextArray
        {
            public List<KeyTextEntry> Entries = new List<KeyTextEntry>();
        }

        [Serializable]
        public class KeyTextEntry
        {
            public string Key;
            public string Text;


            public KeyTextEntry() { }

            public KeyTextEntry(string key, string text)
            {
                Key = key;
                Text = text;
            }
        }


        [Serializable]
        public class Request
        {
            public string Model
            {
                get => model;
                set => model = value;
            }
            [SerializeField] private string model = "gpt-3.5-turbo";

            public MessageRequest[] Messages
            {
                get => messages;
                set => messages = value;
            }
            [SerializeField] private MessageRequest[] messages;

            public Request() { }

            public Request(string message)
            {
                messages = new MessageRequest[]
                {
                    new MessageRequest(message)
                };
            }
        }

        [Serializable]
        public class MessageRequest
        {
            public string Role
            {
                get => role;
                set => role = value;
            }
            [SerializeField] private string role = "user";

            public string Message
            {
                get => content;
                set => content = value;
            }
            [SerializeField] private string content;


            public MessageRequest() { }

            public MessageRequest(string role, string message)
            {
                this.role = message;
                this.content = message;
            }

            public MessageRequest(string message)
            {
                this.content = message;
            }
        }

        [Serializable]
        public class Response
        {
            public ResponseChoice[] Choices => choices;
            [SerializeField] private ResponseChoice[] choices;
        }

        [Serializable]
        public struct ResponseChoice
        {
            public string Message => message.Message;
            [SerializeField] private ResponseChoiceMessage message;
        }

        [Serializable]
        public struct ResponseChoiceMessage
        {
            public string Message => content;
            [SerializeField] private string content;
        }
    }
}

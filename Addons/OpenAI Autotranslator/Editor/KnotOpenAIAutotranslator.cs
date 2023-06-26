using System;
using System.Collections.Generic;
using System.Linq;
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
            if (IsTranslating)
                return;

            request.Settings ??= DefaultSettings;

            var targets = request.Preset.TranslationTargets.Where(t => t.TextCollection != null && t.CultureInfo != null).ToArray();
            if (targets.Length == 0)
            {
                KnotLocalization.Log("No translation targets", LogType.Warning, request.Preset);
                return;
            }

            IsTranslating = true;
            var cancelTokenSrc = new CancellationTokenSource();
            int progressId = Progress.Start(CoreName);
            Progress.RegisterCancelCallback(progressId, () =>
            {
                cancelTokenSrc.Cancel();
                return true;
            });

            try
            {
                var keyTextArray = BuildKeyTextArray(request.Preset, request.Settings);
                var results = new List<KeyValuePair<KnotOpenAIAutotranslatorPreset.TranslationTargetEntry, Dictionary<string, string>>>();
                foreach (var target in targets)
                {
                    if (!target.Enabled)
                        continue;

                    string progressBarMsg = string.Empty;
                    var textArrayProgress = new Vector2Int(1, 1);
                    var targetProgress = new Progress<Vector2Int>();

                    void UpdateProgressBarMessage()
                    {
                        progressBarMsg = "Translating keys from " +
                                         $"{request.Preset.TranslationSource.CultureInfo.EnglishName} " +
                                         $"to {target.CultureInfo.EnglishName}.  [{textArrayProgress.x} / {textArrayProgress.y}]";
                    }

                    targetProgress.ProgressChanged += (sender, f) =>
                    {
                        textArrayProgress = f;
                        UpdateProgressBarMessage();
                    };

                    var task = TranslateArray(request, target, cancelTokenSrc.Token, targetProgress, keyTextArray);
                    while (!task.IsCompleted)
                    {
                        Progress.SetDescription(progressId, progressBarMsg);
                        await Task.Delay(100, cancelTokenSrc.Token);
                    }

                    if (task.Result != null)
                        results.Add(new KeyValuePair<KnotOpenAIAutotranslatorPreset.TranslationTargetEntry,
                            Dictionary<string, string>>(target, task.Result));

                    if (!cancelTokenSrc.IsCancellationRequested) 
                        continue;

                    Progress.Finish(progressId, Progress.Status.Canceled);
                    return;
                }
                
                if (!results.Any())
                {
                    KnotLocalization.Log("Translation ended with no results.", LogType.Warning, request.Preset);
                    Progress.Remove(progressId);
                    return;
                }

                ApplyTranslation(results);
                Progress.Finish(progressId);
                KnotLocalization.Log("Translation complete. You can undo this action.", LogType.Log, request.Preset);
            }
            catch (Exception e)
            {
                Progress.Finish(progressId, Progress.Status.Failed);

                if (!(e is TaskCanceledException))
                    KnotLocalization.Log(e, LogType.Error, request.Preset);
            }
            finally
            {
                Progress.UnregisterCancelCallback(progressId);
                IsTranslating = false;
            }
        }

        static async Task<Dictionary<string, string>> TranslateArray(TranslationRequest request, 
            KnotOpenAIAutotranslatorPreset.TranslationTargetEntry target, CancellationToken cancellationToken, 
            IProgress<Vector2Int> progress, KeyTextArray keyTextArray)
        {
            if (keyTextArray.Entries.Count == 0)
                return null;

            switch (target.KeySelection)
            {
                case KnotOpenAIAutotranslatorPreset.TargetKeySelectionMode.MissingOnly:
                    var missingKeyTextArray = new KeyTextArray();
                    foreach (var e in keyTextArray.Entries)
                        if (!target.TextCollection.Any(d => d.Key.Equals(e.Key)))
                            missingKeyTextArray.Entries.Add(e);
                    keyTextArray = missingKeyTextArray;
                    break;
                case KnotOpenAIAutotranslatorPreset.TargetKeySelectionMode.ExistingOnly:
                    var existingKeyTextArray = new KeyTextArray();
                    foreach (var e in keyTextArray.Entries)
                        if (target.TextCollection.Any(d => d.Key.Equals(e.Key)))
                            existingKeyTextArray.Entries.Add(e);
                    keyTextArray = existingKeyTextArray;
                    break;
            }

            var keyTextArrays = keyTextArray.SplitMaxChars(request.Settings.MaxCharactersPerRequest);
            var result = new Dictionary<string, string>();
            StringBuilder prompt = new StringBuilder();
            int keyTextArrayId = 0;
            foreach (var array in keyTextArrays)
            {
                progress.Report(new Vector2Int(keyTextArrayId + 1, keyTextArrays.Count));

                prompt.Clear();
                prompt.Append(string.Format(request.Settings.CompletionPrompt,
                    request.Preset.TranslationSource.CultureName, target.CultureName));
                prompt.AppendLine(JsonUtility.ToJson(array));
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

                keyTextArrayId++;
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

        static void ApplyTranslation(List<KeyValuePair<KnotOpenAIAutotranslatorPreset.TranslationTargetEntry, Dictionary<string, string>>> results)
        {
            Undo.RegisterCompleteObjectUndo(results.Select(p => p.Key.TextCollection).ToArray(), CoreName);
            
            foreach (var r in results)
            {
                foreach (var t in r.Value)
                {
                    var existingData = r.Key.TextCollection.FirstOrDefault(d => d.Key == t.Key);
                    if (existingData != null)
                    {
                        existingData.RawText = t.Value;
                    }
                    else r.Key.TextCollection.Add(new KnotTextData(t.Key, t.Value));
                }
                EditorUtility.SetDirty(r.Key.TextCollection);
                AssetDatabase.SaveAssetIfDirty(r.Key.TextCollection);
            }

            if (EditorWindow.HasOpenInstances<KnotDatabaseEditorWindow>())
                EditorWindow.GetWindow<KnotDatabaseEditorWindow>().ReloadLayout();
        }

        static KeyTextArray BuildKeyTextArray(KnotOpenAIAutotranslatorPreset preset, TranslationSettings settings)
        {
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

            return array;
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
            public string CompletionModel = "gpt-3.5";
            public string CompletionPrompt = "Translate the following JSON file from {0} to {1}. Do not translate JSON \"Key\" value and keep it as is.";

            public int RequestTimeout = 20;
            public int MaxCharactersPerRequest = 1000;
        }

        [Serializable]
        public class KeyTextArray
        {
            public List<KeyTextEntry> Entries = new List<KeyTextEntry>();

            
            public List<KeyTextArray> SplitMaxChars(int maxChars)
            {
                List<KeyTextArray> arrays = new List<KeyTextArray>();
                KeyTextArray newArray = new KeyTextArray();

                int curCharsCount = 0;
                foreach (var e in Entries)
                {
                    if (curCharsCount >= maxChars)
                    {
                        arrays.Add(newArray);
                        newArray = new KeyTextArray();
                        curCharsCount = 0;
                    }

                    newArray.Entries.Add(e);
                    curCharsCount += e.Key.Length + e.Text.Length;
                }

                if (newArray.Entries.Count > 0)
                    arrays.Add(newArray);

                return arrays;
            }
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Knot.Localization.Attributes;
using Knot.Localization.Data;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace Knot.Localization.Demo
{
    [KnotTypeInfo("JSON from URL")]
    public class KnotJsonUrlCollectionProvider : IKnotRuntimeItemCollectionProvider
    {
        public string Url
        {
            get => _url;
            set => _url = value;
        }
        [SerializeField] private string _url;
        
        private KnotTextCollection _cachedTextCollection;


        public Task<KnotItemCollection> LoadAsync()
        {
            if (_cachedTextCollection == null)
                _cachedTextCollection = ScriptableObject.CreateInstance<KnotTextCollection>();
            _cachedTextCollection.Clear();

            TaskCompletionSource<KnotItemCollection> t = new TaskCompletionSource<KnotItemCollection>();
            if (!string.IsNullOrEmpty(Url))
            {
                var request = UnityWebRequest.Get(Url);
                var op = request.SendWebRequest();
                op.completed += operation =>
                {
                    string rawText = request.downloadHandler.text;

#if UNITY_2020_1_OR_NEWER
                    if (request.result == UnityWebRequest.Result.ConnectionError || string.IsNullOrEmpty(rawText))
#else
                    if (request.isNetworkError || string.IsNullOrEmpty(rawText))
#endif
                        KnotLocalization.Log($"Could not load Text Collection from URL: {Url}", LogType.Error);
                    else
                    {
                        try
                        {
                            var textCollection = JsonUtility.FromJson<JsonTextCollection>(rawText);
                            foreach (var item in textCollection.Items)
                            {
                                _cachedTextCollection.Add(item);
                            }
                        }
                        catch
                        {
                            KnotLocalization.Log($"Could not parse JSON from URL: {Url}", LogType.Error);
                        }
                    }

                    t.SetResult(_cachedTextCollection);
                    request.Dispose();
                };
            }
            else KnotLocalization.Log("Could not load Text Collection. Gist URL is empty", LogType.Error);

            return t.Task;
        }

        public void Unload()
        {
            if (_cachedTextCollection != null)
                Object.DestroyImmediate(_cachedTextCollection);
        }

        [Serializable]
        class JsonTextCollection
        {
            public List<KnotTextData> Items = new List<KnotTextData>();
        }
    }
}
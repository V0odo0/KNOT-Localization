using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Knot.Localization.Attributes;
using Knot.Localization.Data;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace Knot.Localization
{
    /// <summary>
    /// Default implementation of <see cref="IKnotManager"/>
    /// </summary>
    [Serializable]
    [KnotTypeInfo("Default Manager")]
    public class KnotManager : IKnotManager
    {
        public event Action<KnotManagerState> StateChanged;


        public KnotDatabase Database => _database ?? KnotDatabase.Empty;
        [NonSerialized] private KnotDatabase _database;

        public KnotManagerState State => _state;
        [NonSerialized] private KnotManagerState _state = KnotManagerState.LanguageNotLoaded;

        public KnotLanguageData SelectedLanguage => _selectedLanguage;
        [NonSerialized] private KnotLanguageData _selectedLanguage;

        public IList<KnotLanguageData> Languages => _languages ?? (_languages = new List<KnotLanguageData>());
        [NonSerialized] private List<KnotLanguageData> _languages;

        public IKnotTextController TextController => _textController ?? (_textController = new KnotTextController());
        [NonSerialized] private IKnotTextController _textController;

        public IKnotAssetController AssetController => _assetController ?? (_assetController = new KnotAssetController());
        [NonSerialized] private IKnotAssetController _assetController;


        [NonSerialized] private HashSet<IKnotRuntimeItemCollectionProvider> _loadedCollectionProviders;
        [NonSerialized] private KnotItemCollection[] _fallbackCollections;


        //bug: Unity refuses to serialize classes with no serializable fields, so we put a placeholder to fix it (Issue ID: 1183547)
        [SerializeField, HideInInspector] private bool _serializationPlaceholder;

        
        protected virtual async void LoadLanguageAsync(KnotLanguageData language)
        {
            _state = KnotManagerState.LoadingLanguage;
            StateChanged?.Invoke(_state);

            //Unload previously loaded collections
            if (_loadedCollectionProviders != null)
                foreach (var cp in _loadedCollectionProviders)
                    cp.Unload();
            _loadedCollectionProviders = new HashSet<IKnotRuntimeItemCollectionProvider>();
            _fallbackCollections = null;

            //Load target language collections
            var collections = (await LoadCollectionsAsync(language.CollectionProviders)).ToArray();

            foreach (var cp in language.CollectionProviders.OfType<IKnotRuntimeItemCollectionProvider>())
                _loadedCollectionProviders.Add(cp);

            //Get fallback language
            var fallbackMetadata = Database?.Settings.Metadata.OfType<KnotLanguageFallbackMetadata>().
                Union(language.Metadata.OfType<KnotLanguageFallbackMetadata>()) ?? new KnotLanguageFallbackMetadata[0];
            var fallbackLanguage = Database?.Languages.Where(ld => ld != language).
                FirstOrDefault(ld => fallbackMetadata.Any(fb => fb.CultureName == ld.CultureName));

            var textKeys = GetKeyDataFromCollections(Database?.TextKeyCollections ?? new List<KnotKeyCollection>());
            var assetKeys = GetKeyDataFromCollections(Database?.AssetKeyCollections ?? new List<KnotKeyCollection>());

            //Load all items from collections including missing keys from fallback language collections
            var textItems = await GetItemDataFromCollections<KnotTextData>(textKeys, fallbackLanguage, collections);
            var assetItems = await GetItemDataFromCollections<KnotAssetData>(assetKeys, fallbackLanguage, collections);

            //Build controllers
            await TextController.BuildAsync(new KnotControllerBuildData<KnotTextData>
            {
                Culture = language.CultureInfo,
                GlobalMetadata = Database?.Settings.Metadata.ToArray() ?? new IKnotMetadata[0],
                LanguageMetadata = language.Metadata.ToArray(),
                KeyData = textKeys.ToArray(),
                ItemData = textItems.ToArray(),
            });
            await AssetController.BuildAsync(new KnotControllerBuildData<KnotAssetData>
            {
                Culture = language.CultureInfo,
                GlobalMetadata = Database?.Settings.Metadata.ToArray() ?? new IKnotMetadata[0],
                LanguageMetadata = language.Metadata.ToArray(),
                KeyData = assetKeys.ToArray(),
                ItemData = assetItems.ToArray(),
            });

            //Check if the language has been changed during loading process and load selected language
            if (SelectedLanguage != language)
            {
                LoadLanguageAsync(language);
                return;
            }

            _state = KnotManagerState.LanguageLoaded;
            StateChanged?.Invoke(_state);
        }

        protected virtual List<KnotKeyData> GetKeyDataFromCollections(List<KnotKeyCollection> keyCollections)
        {
            HashSet<string> allKeys = new HashSet<string>();
            List<KnotKeyData> allKeyData = new List<KnotKeyData>();

            foreach (var keyData in keyCollections.Where(c => c != null).Distinct().SelectMany(c => c))
            {
                if (allKeys.Contains(keyData.Key))
                    continue;

                allKeyData.Add(keyData);
                allKeys.Add(keyData.Key);
            }

            return allKeyData;
        }

        protected virtual async Task<IEnumerable<KnotItemCollection>> LoadCollectionsAsync(IEnumerable<IKnotItemCollectionProvider> providers)
        {
            var tasks = providers.OfType<IKnotRuntimeItemCollectionProvider>().Select(p => p.LoadAsync()).ToArray();
            await Task.WhenAll(tasks);

            return tasks.Where(t => t.Result != null).Select(t => t.Result);
        }

        protected virtual async Task<IList<TItemData>> GetItemDataFromCollections<TItemData>(List<KnotKeyData> keys, 
            KnotLanguageData fallbackLanguage, params KnotItemCollection[] collections) 
            where TItemData : KnotItemData
        {
            var items = GetConcatItemCollection(collections.OfType<IKnotItemCollection<TItemData>>().ToArray()).ToList();
            if (keys.Count > 0 && keys.Count > items.Count && fallbackLanguage != null)
            {
                if (_fallbackCollections == null)
                {
                    var targetCollectionProviders = fallbackLanguage.CollectionProviders
                        .OfType<IKnotRuntimeItemCollectionProvider>().ToArray();

                    _fallbackCollections = (await LoadCollectionsAsync(targetCollectionProviders)).ToArray();
                    foreach (var fallbackCp in targetCollectionProviders)
                        _loadedCollectionProviders.Add(fallbackCp);
                }

                AppendMissingKeys(items, keys, GetConcatItemCollection(_fallbackCollections.OfType<IKnotItemCollection<TItemData>>().ToArray()));
            }

            return items;
        }

        protected virtual IEnumerable<TItemData> GetConcatItemCollection<TItemData>(params IKnotItemCollection<TItemData>[] collections) 
            where TItemData : KnotItemData
        {
            return collections.SelectMany(c => c).GroupBy(d => d.Key).Select(g => g.First());
        }

        protected virtual void AppendMissingKeys<TItemData>(List<TItemData> targetItems, List<KnotKeyData> keys, IEnumerable<TItemData> collection) where TItemData : KnotItemData
        {
            var missingKeys = keys.Select(d => d.Key).Except(targetItems.Select(d => d.Key)).ToArray();
            targetItems.AddRange(collection.Where(d => missingKeys.Contains(d.Key)));
        }

        protected virtual void TransferControllerValueChangedCallbacks<TItemData, TValue, TValueType>(
            IKnotController<TItemData, TValue, TValueType> from,
            IKnotController<TItemData, TValue, TValueType> to) where TItemData : KnotItemData where TValue : class, IKnotValue<TValueType>
        {
            if (from == null || to == null)
                return;

            foreach (var sourceCallback in from.ValueChangedCallbacks)
            {
                if (to.ValueChangedCallbacks.ContainsKey(sourceCallback.Key))
                    to.ValueChangedCallbacks[sourceCallback.Key] += sourceCallback.Value;
                else to.ValueChangedCallbacks.Add(sourceCallback.Key, sourceCallback.Value);
            }
        }

        
        public virtual void SetDatabase(KnotDatabase database, bool loadStartupLanguage = false)
        {
            _database = database;
            _languages = database.Languages.ToList();

            //Ensure that all subscribers of old database controllers are transferred to new database controllers
            if (_textController != null)
            {
                TransferControllerValueChangedCallbacks(_textController, Database.Settings.TextController);
                _textController.Dispose();
            }
            if (_assetController != null)
            {
                TransferControllerValueChangedCallbacks(_assetController, Database.Settings.AssetController);
                _assetController.Dispose();
            }

            _textController = database.Settings.TextController.Clone() as IKnotTextController;
            _assetController = database.Settings.AssetController.Clone() as  IKnotAssetController;

            if (loadStartupLanguage)
            {
                _selectedLanguage = Database.Settings.LanguageSelector.GetStartupLanguage(Languages.ToArray());
                LoadLanguage(SelectedLanguage);
            }
        }

        public virtual void LoadLanguage(KnotLanguageData languageData)
        {
            if (languageData == null)
                return;

            _selectedLanguage = languageData;
            Database?.Settings.LanguageSelector.SaveSelectedLanguage(_selectedLanguage);
            LoadLanguageAsync(languageData);
        }

        public virtual IKnotText GetTextValue(string key) => TextController.TryGetValue(key, out var value) ? value : new KnotText(TextController.GetFallbackValue(key));

        public virtual IKnotAsset GetAssetValue(string key) => AssetController.TryGetValue(key, out var value) ? value : new KnotAsset(AssetController.GetFallbackValue(key));

        public virtual void Dispose()
        {
            _languages?.Clear();

            if (_loadedCollectionProviders != null)
                foreach (var cp in _loadedCollectionProviders)
                    cp.Unload();

            _textController?.Dispose();
            _assetController?.Dispose();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Knot.Core;
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


        /// <summary>
        /// <see cref="Database"/> followed by all its <see cref="KnotDatabase.AdditionalDatabases"/>, ordered from the lowest to the highest priority
        /// </summary>
        protected IReadOnlyList<KnotDatabase> MergedDatabases => _mergedDatabases ?? (_mergedDatabases = Database.GetMergedDatabases());
        [NonSerialized] private List<KnotDatabase> _mergedDatabases;

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

            //Refresh merged databases so Additional Databases edited after SetDatabase (e.g. Live Reload) are respected
            _mergedDatabases = Database.GetMergedDatabases();

            //Load target language collections from all merged databases
            var collectionProviders = GetMergedCollectionProviders(language);
            var collections = (await LoadCollectionsAsync(collectionProviders)).ToArray();

            foreach (var cp in collectionProviders.OfType<IKnotRuntimeItemCollectionProvider>())
                _loadedCollectionProviders.Add(cp);

            var globalMetadata = GetMergedGlobalMetadata();
            var languageMetadata = GetMergedLanguageMetadata(language);

            //Get fallback language
            var fallbackMetadata = globalMetadata.OfType<KnotLanguageFallbackMetadata>().
                Union(languageMetadata.OfType<KnotLanguageFallbackMetadata>()).ToArray();
            var fallbackLanguage = Languages.Where(ld => ld.CultureName != language.CultureName).
                FirstOrDefault(ld => fallbackMetadata.Any(fb => fb.CultureName == ld.CultureName));

            //Collections are passed from the highest to the lowest priority database, so the first found key wins
            var textKeys = GetKeyDataFromCollections(MergedDatabases.Reverse().SelectMany(db => db.TextKeyCollections).ToList());
            var assetKeys = GetKeyDataFromCollections(MergedDatabases.Reverse().SelectMany(db => db.AssetKeyCollections).ToList());

            //Load all items from collections including missing keys from fallback language collections
            var textItems = await GetItemDataFromCollections<KnotTextData>(textKeys, fallbackLanguage, collections);
            var assetItems = await GetItemDataFromCollections<KnotAssetData>(assetKeys, fallbackLanguage, collections);

            //Build controllers
            await TextController.BuildAsync(new KnotControllerBuildData<KnotTextData>
            {
                Culture = language.CultureInfo,
                GlobalMetadata = globalMetadata,
                LanguageMetadata = languageMetadata,
                KeyData = textKeys.ToArray(),
                ItemData = textItems.ToArray(),
            });
            await AssetController.BuildAsync(new KnotControllerBuildData<KnotAssetData>
            {
                Culture = language.CultureInfo,
                GlobalMetadata = globalMetadata,
                LanguageMetadata = languageMetadata,
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

        /// <summary>
        /// Returns all <see cref="KnotLanguageData"/> with the same culture as <paramref name="language"/> from <see cref="MergedDatabases"/>, ordered from the highest to the lowest priority
        /// </summary>
        protected virtual List<KnotLanguageData> GetMergedLanguageData(KnotLanguageData language)
        {
            var result = MergedDatabases.Reverse()
                .SelectMany(db => db.Languages.Where(ld => ld != null && ld.CultureName == language.CultureName))
                .ToList();

            if (!result.Contains(language))
                result.Insert(0, language);

            return result;
        }

        /// <summary>
        /// Returns collection providers of <paramref name="language"/> culture from <see cref="MergedDatabases"/>, ordered from the highest to the lowest priority
        /// </summary>
        protected virtual List<IKnotItemCollectionProvider> GetMergedCollectionProviders(KnotLanguageData language)
        {
            return GetMergedLanguageData(language).SelectMany(ld => ld.CollectionProviders).Distinct().ToList();
        }

        protected virtual IKnotMetadata[] GetMergedLanguageMetadata(KnotLanguageData language)
        {
            var languages = GetMergedLanguageData(language);
            languages.Reverse();

            return languages.SelectMany(ld => ld.Metadata).Where(m => m != null).Distinct().ToArray();
        }

        protected virtual IKnotMetadata[] GetMergedGlobalMetadata()
        {
            return MergedDatabases.SelectMany(db => db.Settings.Metadata).Where(m => m != null).Distinct().ToArray();
        }

        protected virtual List<KnotLanguageData> GetMergedLanguages(IEnumerable<KnotDatabase> databases)
        {
            var languages = new List<KnotLanguageData>();
            foreach (var language in databases.SelectMany(db => db.Languages))
            {
                if (language != null && languages.All(ld => ld.CultureName != language.CultureName))
                    languages.Add(language);
            }

            return languages;
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
                    var targetCollectionProviders = GetMergedCollectionProviders(fallbackLanguage)
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
            _mergedDatabases = Database.GetMergedDatabases();
            _languages = GetMergedLanguages(_mergedDatabases);

            var oldTextController = _textController;
            var oldAssetController = _assetController;

            _textController = database.Settings.TextController.Clone() as IKnotTextController;
            _assetController = database.Settings.AssetController.Clone() as  IKnotAssetController;

            if (oldTextController != null)
            {
                TransferControllerValueChangedCallbacks(oldTextController, _textController);
                oldTextController.Dispose();
            }
            if (oldAssetController != null)
            {
                TransferControllerValueChangedCallbacks(oldAssetController, _assetController);
                oldAssetController.Dispose();
            }

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

            //Language may come from one of the additional databases, so the merged instance with the same culture is used
            if (!Languages.Contains(languageData))
                languageData = Languages.FirstOrDefault(ld => ld.CultureName == languageData.CultureName) ?? languageData;

            _selectedLanguage = languageData;
            Database?.Settings.LanguageSelector.SaveSelectedLanguage(_selectedLanguage);
            LoadLanguageAsync(languageData);
        }

        public virtual IKnotText GetTextValue(string key) => TextController.TryGetValue(key, out var value) ? value : new KnotText(TextController.GetFallbackValue(key));

        public virtual IKnotText GetTextValue(string key, KnotPluralForm pluralForm)
        {
            var pluralFormKey = $"{key}.{pluralForm.ToString()}";
            if (TextController.ContainsKey(pluralFormKey))
            {
                if (TextController.TryGetValue(pluralFormKey, out var value))
                    return value;

                if (pluralForm != KnotPluralForm.One && TextController.TryGetValue($"{key}.{KnotPluralForm.Many}", out var manyValue))
                    return manyValue;
            }

            return GetTextValue(key);
        }

        public virtual IKnotAsset GetAssetValue(string key) => AssetController.TryGetValue(key, out var value) ? value : new KnotAsset(AssetController.GetFallbackValue(key));

        public virtual void Dispose()
        {
            _languages?.Clear();
            _mergedDatabases = null;

            if (_loadedCollectionProviders != null)
                foreach (var cp in _loadedCollectionProviders)
                    cp.Unload();

            _textController?.Dispose();
            _assetController?.Dispose();
        }
    }
}
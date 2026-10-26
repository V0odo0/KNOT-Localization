using System.Collections.Generic;
using System.Linq;
using Knot.Core;
using UnityEngine;

namespace Knot.Localization.Data
{
    /// <summary>
    /// General asset that stores all reference to localization data
    /// </summary>
    [CreateAssetMenu(fileName = "KnotDatabase", menuName = KnotLocalization.CorePath + "Database")]
    public class KnotDatabase : ScriptableObject
    {
        internal static KnotDatabase Empty
        {
            get
            {
                if (_empty == null)
                {
                    _empty = CreateInstance<KnotDatabase>();
                    _empty.name = $"Empty {nameof(KnotDatabase)}";
                }

                return _empty;
            }
        }
        private static KnotDatabase _empty;


        public List<KnotLanguageData> Languages
        {
            get => _languages;
            set
            {
                if (value == null)
                    _languages.Clear();
                else _languages = value;
            }
        }
        [SerializeField] private List<KnotLanguageData> _languages = new List<KnotLanguageData>();

        public List<KnotKeyCollection> TextKeyCollections => _textKeyCollections ?? (_textKeyCollections = new List<KnotKeyCollection>());
        [SerializeField, KnotCreateAssetField(typeof(KnotKeyCollection))] private List<KnotKeyCollection> _textKeyCollections;

        public List<KnotKeyCollection> AssetKeyCollections => _assetKeyCollections ?? (_assetKeyCollections = new List<KnotKeyCollection>());
        [SerializeField, KnotCreateAssetField(typeof(KnotKeyCollection))] private List<KnotKeyCollection> _assetKeyCollections;

        public KnotDatabaseSettings Settings
        {
            get => _settings;
            set
            {
                if (value == null)
                    return;

                _settings = value;
            }
        }
        [SerializeField] private KnotDatabaseSettings _settings = new KnotDatabaseSettings();

        /// <summary>
        /// Databases merged into this one when it is loaded by <see cref="IKnotManager"/>.
        /// Their languages, keys and values override the ones of this database, later entries override earlier ones
        /// </summary>
        public List<KnotDatabase> AdditionalDatabases => _additionalDatabases ?? (_additionalDatabases = new List<KnotDatabase>());
        [SerializeField] private List<KnotDatabase> _additionalDatabases;


        /// <summary>
        /// Returns this database followed by all <see cref="AdditionalDatabases"/> resolved recursively, ordered from the lowest to the highest priority.
        /// Each database appears once, so self, circular and duplicate references are skipped
        /// </summary>
        public List<KnotDatabase> GetMergedDatabases()
        {
            var result = new List<KnotDatabase>();
            var visited = new HashSet<KnotDatabase>();
            var stack = new Stack<KnotDatabase>();
            stack.Push(this);

            while (stack.Count > 0)
            {
                var database = stack.Pop();
                if (database == null || !visited.Add(database))
                    continue;

                result.Add(database);

                if (database._additionalDatabases == null)
                    continue;

                for (int i = database._additionalDatabases.Count - 1; i >= 0; i--)
                    stack.Push(database._additionalDatabases[i]);
            }

            return result;
        }

        /// <summary>
        /// Returns <see cref="GetMergedDatabases"/> ordered from the highest to the lowest priority
        /// </summary>
        public IEnumerable<KnotDatabase> GetMergedDatabasesByPriority()
        {
            var databases = GetMergedDatabases();
            databases.Reverse();
            return databases;
        }

        /// <summary>
        /// Returns text <see cref="KnotKeyCollection"/>s of this database and all <see cref="AdditionalDatabases"/>, ordered from the highest to the lowest priority
        /// </summary>
        public IEnumerable<KnotKeyCollection> GetMergedTextKeyCollections() =>
            GetMergedDatabasesByPriority().SelectMany(db => db.TextKeyCollections);

        /// <summary>
        /// Returns asset <see cref="KnotKeyCollection"/>s of this database and all <see cref="AdditionalDatabases"/>, ordered from the highest to the lowest priority
        /// </summary>
        public IEnumerable<KnotKeyCollection> GetMergedAssetKeyCollections() =>
            GetMergedDatabasesByPriority().SelectMany(db => db.AssetKeyCollections);


#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (_additionalDatabases == null)
                return;

            if (_additionalDatabases.Contains(this))
            {
                _additionalDatabases.RemoveAll(db => db == this);
                KnotLocalization.Log($"{name} can't reference itself as Additional Database", LogType.Warning, this);
            }

            foreach (var db in _additionalDatabases.Where(db => db != null).Distinct())
            {
                if (db.GetMergedDatabases().Contains(this))
                    KnotLocalization.Log($"{name} and {db.name} reference each other. The circular reference is ignored when the database is loaded", LogType.Warning, this);
            }
        }
#endif
    }
}
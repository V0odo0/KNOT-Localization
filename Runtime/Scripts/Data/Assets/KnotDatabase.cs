using System.Collections.Generic;
using Knot.Localization.Attributes;
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
    }
}
using System;
using System.Collections.Generic;
using Knot.Localization.Attributes;
using Knot.Localization.Data;
using UnityEngine;

namespace Knot.Localization
{
    //[KnotTypeInfo("CSV Import Export Preset")]
    //[CreateAssetMenu(fileName = "KnotCsvImportExportPreset", menuName = KnotLocalization.CoreName + "/Import Export/CSV Preset", order = 50)]
    public class KnotCsvImportExportPreset : ScriptableObject
    {
        public KnotDatabase Database => _database;
        [SerializeField] private KnotDatabase _database;
        
        public ImportExportSettings Settings => _settings;
        [SerializeField] private ImportExportSettings _settings;

        public List<ImportExportTarget> Targets => _targets ?? (_targets = new List<ImportExportTarget>());
        [SerializeField] private List<ImportExportTarget> _targets;


        [Serializable]
        public class ImportExportSettings
        {
            public string ExportDelimiter
            {
                get => _exportDelimiter;
                set => _exportDelimiter = value;
            }
            [SerializeField] private string _exportDelimiter;

            public string ImportDelimiter
            {
                get => _importDelimiter;
                set => _importDelimiter = value;
            }
            [SerializeField] private string _importDelimiter;
        }

        [Serializable]
        public class ImportExportTarget
        {
            public KnotTextCollection TextCollection
            {
                get => _textCollection;
                set => _textCollection = value;
            }
            [SerializeField] private KnotTextCollection _textCollection;

            public TextAsset TextAsset
            {
                get => _textAsset;
                set => _textAsset = value;
            }
            [SerializeField] private TextAsset _textAsset;

        }
    }
}

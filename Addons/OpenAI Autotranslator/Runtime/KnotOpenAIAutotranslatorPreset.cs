using System;
using System.Collections.Generic;
using System.Globalization;
using Knot.Localization.Attributes;
using UnityEngine;

namespace Knot.Localization.Data
{
    [KnotTypeInfo("OpenAI Autotranslator Preset")]
    [CreateAssetMenu(fileName = "KnotOpenAIAutotranslatorPreset", menuName = KnotLocalization.CorePath + "Addons/OpenAI Autotranslator Preset", order = 50)]
    public class KnotOpenAIAutotranslatorPreset : ScriptableObject
    {
        public TranslationEntry TranslationSource
        {
            get => _translationSource;
            set => _translationSource = value;
        }
        [SerializeField] private TranslationEntry _translationSource;

        public List<TranslationTargetEntry> TranslationTargets => _translationTargets ?? (_translationTargets = new List<TranslationTargetEntry>());
        [SerializeField] private List<TranslationTargetEntry> _translationTargets;

        public List<string> ExcludeKeys => _excludeKeys ?? (_excludeKeys = new List<string>());
        [SerializeField, KnotTextKeyPicker] private List<string> _excludeKeys;
        

        [Serializable]
        public class TranslationEntry
        {
            public CultureInfo CultureInfo => CultureInfo.GetCultureInfo(CultureName);

            public string CultureName
            {
                get => _cultureName;
                set => _cultureName = value;
            }
            [SerializeField, KnotCultureNamePicker] private string _cultureName;

            public KnotTextCollection TextCollection
            {
                get => _textCollection;
                set => _textCollection = value;
            }
            [SerializeField] private KnotTextCollection _textCollection;
        }

        [Serializable]
        public class TranslationTargetEntry : TranslationEntry
        {
            public bool Enabled
            {
                get => _enabled;
                set => _enabled = value;
            }
            [SerializeField] private bool _enabled = true;

            public string TranslationExtraContext
            {
                get => _translationExtraContext;
                set => _translationExtraContext = value;
            }
            [SerializeField, TextArea] private string _translationExtraContext;

            public TargetKeySelectionMode KeySelection
            {
                get => _keySelection;
                set => _keySelection = value;
            }
            [SerializeField] private TargetKeySelectionMode _keySelection = TargetKeySelectionMode.All;
        }

        [Serializable]
        public enum TargetKeySelectionMode
        {
            All,
            MissingOnly,
            ExistingOnly
        }
    }
}

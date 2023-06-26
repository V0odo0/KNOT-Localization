using Knot.Localization.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Knot.Localization.Data
{
    [Serializable]
    [KnotMetadataInfo("Localized Text Override", KnotMetadataInfoAttribute.MetadataScope.Text, AllowMultipleInstances = false)]
    public class KnotLocalizedTextOverrideMetadata : IKnotTextFormatterMetadata, IKnotCultureSpecificMetadata
    {
        public List<LocalizedTextEntry> LocalizedTexts => _localizedTexts ?? (_localizedTexts = new List<LocalizedTextEntry>());
        [SerializeField] private List<LocalizedTextEntry> _localizedTexts = new List<LocalizedTextEntry>();

        [NonSerialized] private CultureInfo _activeCulture;


        public KnotLocalizedTextOverrideMetadata() { }

        public KnotLocalizedTextOverrideMetadata(params KeyValuePair<string, string>[] customLocalizedTexts)
        {
            
        }

        public object Clone() => (MemberwiseClone() as KnotLocalizedTextOverrideMetadata) ?? new KnotLocalizedTextOverrideMetadata();
        
        
        public void SetCulture(CultureInfo cultureInfo)
        {
            _activeCulture = cultureInfo;
        }

        public void Format(StringBuilder sb)
        {
            var targetEntry = LocalizedTexts.FirstOrDefault(c => CultureInfo.GetCultureInfo(c.CultureName).Equals(_activeCulture));
            if (targetEntry != null)
            {
                sb.Clear();
                sb.Append(targetEntry.Text);
            }
        }

        [Serializable]
        public class LocalizedTextEntry
        {
            [KnotCultureNamePicker] public string CultureName;
            [TextArea(1, 3)] public string Text;


            public LocalizedTextEntry() { }

            public LocalizedTextEntry(string cultureName, string text)
            {
                CultureName = cultureName;
                Text = text;
            }
        }
    }
}

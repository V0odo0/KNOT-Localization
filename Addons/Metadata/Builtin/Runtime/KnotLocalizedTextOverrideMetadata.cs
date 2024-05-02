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
    public class KnotLocalizedTextOverrideMetadata : IKnotTextFormatterMetadata
    {
        public List<LocalizedTextEntry> LocalizedTexts => _localizedTexts ?? (_localizedTexts = new List<LocalizedTextEntry>());
        [SerializeField] private List<LocalizedTextEntry> _localizedTexts = new List<LocalizedTextEntry>();
        

        public KnotLocalizedTextOverrideMetadata() { }

        public KnotLocalizedTextOverrideMetadata(params LocalizedTextEntry[] entries)
        {
            LocalizedTexts.AddRange(entries);
        }

        public object Clone() => (MemberwiseClone() as KnotLocalizedTextOverrideMetadata) ?? new KnotLocalizedTextOverrideMetadata();
        
        
        public void Format(StringBuilder sb)
        {
            Format(sb, CultureInfo.GetCultureInfo(LocalizedTexts.FirstOrDefault()?.CultureName ?? CultureInfo.DefaultThreadCurrentCulture.Name));
        }

        public void Format(StringBuilder sb, CultureInfo cultureInfo)
        {
            var targetEntry = LocalizedTexts.FirstOrDefault(c => CultureInfo.GetCultureInfo(c.CultureName).Equals(cultureInfo));
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

using System;
using Knot.Localization.Attributes;
using UnityEngine;

namespace Knot.Localization.Data
{
    /// <summary>
    /// <see cref="IKnotMetadata"/> used by <see cref="KnotManager"/> to additively load <see cref="KnotLanguageData"/>
    /// with the same <see cref="CultureName"/> if current <see cref="KnotItemCollection"/> has missing keys
    /// </summary>
    [Serializable]
    [KnotMetadataInfo("Fallback Language", KnotMetadataInfoAttribute.MetadataScope.Language | KnotMetadataInfoAttribute.MetadataScope.Database, true)]
    public class KnotLanguageFallbackMetadata : IKnotMetadata
    {
        public string CultureName
        {
            get => _cultureName;
            set => _cultureName = value;
        }
        [SerializeField, KnotCultureNamePicker] private string _cultureName;


        public KnotLanguageFallbackMetadata() { }

        public KnotLanguageFallbackMetadata(string cultureName)
        {
            _cultureName = cultureName;
        }

        public object Clone() => new KnotLanguageFallbackMetadata(CultureName);
    }
}
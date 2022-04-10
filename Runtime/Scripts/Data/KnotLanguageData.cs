using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Knot.Localization.Attributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Knot.Localization.Data
{
    /// <summary>
    /// General class for Language related data.
    /// </summary>
    [Serializable]
    public partial class KnotLanguageData
    {
        internal static CultureInfo DefaultCultureInfo => CultureInfo.GetCultureInfo("en-US");


        public CultureInfo CultureInfo => CultureInfo.GetCultureInfo(CultureName);

        /// <summary>
        /// CultureInfo name e.g. "en-US".
        /// </summary>
        public string CultureName
        {
            get => _cultureName;
            set => _cultureName = value;
        }
        [SerializeField, KnotCultureNamePicker] private string _cultureName = "en-US";

        public SystemLanguage SystemLanguage
        {
            get => _systemLanguage;
            set => _systemLanguage = value;
        }
        [SerializeField] private SystemLanguage _systemLanguage = SystemLanguage.English;

        public string NativeName
        {
            get => _nativeName;
            set => _nativeName = value;
        }
        [SerializeField] private string _nativeName = DefaultCultureInfo.NativeName;

        public List<IKnotItemCollectionProvider> CollectionProviders
        {
            get => _collectionProviders.Contains(null) ? _collectionProviders = _collectionProviders.Where(p => p != null).ToList() : _collectionProviders;
            set
            {
                if (value == null)
                    _collectionProviders.Clear();
                else _collectionProviders = value;
            }
        }
        [SerializeReference] private List<IKnotItemCollectionProvider> _collectionProviders = new List<IKnotItemCollectionProvider>();

        public KnotMetadataContainer Metadata => _metadata;
        [SerializeField] private KnotMetadataContainer _metadata = new KnotMetadataContainer();


        public KnotLanguageData() { }

        public KnotLanguageData(string cultureName, string nativeName = "")
        {
            _cultureName = cultureName;
            _systemLanguage = GetSystemLanguageFromCulture(CultureInfo.GetCultureInfo(_cultureName));
            _nativeName = string.IsNullOrEmpty(nativeName) ? CultureInfo.GetCultureInfo(_cultureName).NativeName : nativeName;
        }

        public KnotLanguageData(SystemLanguage systemLanguage, string nativeName = "")
        {
            CultureInfo cultureInfo = GetCultureFromSystemLanguage(systemLanguage);

            _cultureName = cultureInfo.Name;
            _systemLanguage = systemLanguage;
            _nativeName = string.IsNullOrEmpty(nativeName) ? CultureInfo.GetCultureInfo(_cultureName).NativeName : nativeName;
        }
        

        /// <summary>
        /// Converts <see cref="System.Globalization.CultureInfo"/> to <see cref="UnityEngine.SystemLanguage"/>
        /// </summary>
        public static SystemLanguage GetSystemLanguageFromCulture(CultureInfo cultureInfo)
        {
            if (cultureInfo == null)
                return SystemLanguage.Unknown;

            string sysLangName = Enum.GetNames(typeof(SystemLanguage))
                .FirstOrDefault(s => cultureInfo.EnglishName.ToLower().Contains(s.ToLower()));

            return Enum.TryParse(sysLangName, true, out SystemLanguage lang) ? lang : SystemLanguage.Unknown;
        }

        /// <summary>
        /// Converts <see cref="UnityEngine.SystemLanguage"/> to <see cref="System.Globalization.CultureInfo"/>
        /// </summary>
        public static CultureInfo GetCultureFromSystemLanguage(SystemLanguage systemLanguage)
        {
            if (systemLanguage == SystemLanguage.Unknown)
                return CultureInfo.InvariantCulture;

            CultureInfo culture = CultureInfo.GetCultures(CultureTypes.NeutralCultures)
                .FirstOrDefault(c => c.EnglishName.ToLower().Contains(systemLanguage.ToString().ToLower()));

            return culture ?? DefaultCultureInfo;
        }
    }
}
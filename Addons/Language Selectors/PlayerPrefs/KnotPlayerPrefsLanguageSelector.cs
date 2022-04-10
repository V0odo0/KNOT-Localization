using System;
using System.Globalization;
using System.Linq;
using Knot.Localization.Attributes;
using Knot.Localization.Data;
using UnityEngine;

namespace Knot.Localization
{
    /// <summary>
    /// Selects language based on key stored in PlayerPrefs. if app runs for the first time and has no PlayerPrefs key assigned,
    /// language is selected from <see cref="CultureInfo.CurrentUICulture"/> as primary and <see cref="Application.systemLanguage"/> as secondary.
    /// </summary>
    [Serializable]
    [KnotTypeInfo("PlayerPrefs")]
    public class KnotPlayerPrefsLanguageSelector : IKnotLanguageSelector
    {
        /// <summary>
        /// <see cref="PlayerPrefs"/> key
        /// </summary>
        public string Key => _key;
        [SerializeField, Tooltip("PlayerPrefs key")] private string _key = "KnotSelectedLanguage";


        public KnotLanguageData GetStartupLanguage(params KnotLanguageData[] availableLanguageData)
        {
            bool hasKey = PlayerPrefs.HasKey(Key);
            string cultureName = PlayerPrefs.GetString(Key, string.Empty);

            var targetLanguageData =
                //Try get language from cultureName in PlayerPrefs
                (availableLanguageData.FirstOrDefault(data => data.CultureName == cultureName) ??
                 //Try get language from system current UI culture
                 availableLanguageData.FirstOrDefault(data => data.CultureName == CultureInfo.CurrentUICulture.Name)) ??
                //Try get language from Application.systemLanguage
                availableLanguageData.FirstOrDefault(data => data.SystemLanguage == Application.systemLanguage) ??
                //Fallback to default language
                availableLanguageData.FirstOrDefault();

            //Remember cultureName if app runs for the first time
            if (!hasKey && targetLanguageData != null)
                PlayerPrefs.SetString(Key, targetLanguageData.CultureName);

            return targetLanguageData;
        }

        public void SaveSelectedLanguage(KnotLanguageData selectedLanguageData)
        {
            if (selectedLanguageData != null)
                PlayerPrefs.SetString(Key, selectedLanguageData.CultureName);
        }
    }
}
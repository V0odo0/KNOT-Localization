using System;
using System.Linq;
using Knot.Localization.Attributes;
using Knot.Localization.Data;
using UnityEngine;

namespace Knot.Localization
{
    /// <summary>
    /// Simple implementation of <see cref="IKnotLanguageSelector"/> without <see cref="SaveSelectedLanguage"/> implementation
    /// </summary>
    [Serializable]
    [KnotTypeInfo("Default Language")]
    public class KnotDefaultLanguageSelector : IKnotLanguageSelector
    {
        //bug: Unity refuses to serialize classes with no serializable fields, so we put a placeholder to fix it (Issue ID: 1183547)
        [SerializeField, HideInInspector] private bool _serializationPlaceholder;

        /// <summary>
        /// Returns first entry from <paramref name="availableLanguageData"/>
        /// </summary>
        public KnotLanguageData GetStartupLanguage(params KnotLanguageData[] availableLanguageData)
        {
            return availableLanguageData.FirstOrDefault();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public void SaveSelectedLanguage(KnotLanguageData selectedLanguageData) { }
    }
}

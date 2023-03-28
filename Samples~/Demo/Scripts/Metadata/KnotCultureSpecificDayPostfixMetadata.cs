using System;
using System.Globalization;
using System.Text;
using Knot.Localization.Attributes;
using Knot.Localization.Data;
using UnityEngine;

namespace Knot.Localization.Demo
{
    [KnotMetadataInfo("Postfix day of the week", KnotMetadataInfoAttribute.MetadataScope.Text)]
    [Serializable]
    public class KnotCultureSpecificDayPostfixMetadata : IKnotCultureSpecificMetadata, IKnotTextFormatterMetadata
    {
        public object Clone() => new KnotCultureSpecificDayPostfixMetadata();

        [NonSerialized]
        private CultureInfo _currentCulture = CultureInfo.InvariantCulture;

        //bug: Unity refuses to serialize classes with no serializable fields, so we put a placeholder to fix it (Issue ID: 1183547)
        [SerializeField, HideInInspector] private bool _serializationPlaceholder;


        public void SetCulture(CultureInfo cultureInfo) => _currentCulture = cultureInfo;

        public void Format(StringBuilder sb)
        {
            sb.Insert(sb.Length, _currentCulture.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek));
        }
    }
}
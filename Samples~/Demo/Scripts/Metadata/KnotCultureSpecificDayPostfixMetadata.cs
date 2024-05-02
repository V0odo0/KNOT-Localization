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
    public class KnotCultureSpecificDayPostfixMetadata : IKnotTextFormatterMetadata
    {
        //bug: Unity refuses to serialize classes with no serializable fields, so we put a placeholder to fix it (Issue ID: 1183547)
        [SerializeField, HideInInspector] private bool _serializationPlaceholder;

        public object Clone() => new KnotCultureSpecificDayPostfixMetadata();


        public void Format(StringBuilder sb, CultureInfo cultureInfo)
        {
            sb.Insert(sb.Length, cultureInfo.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek));
        }
    }
}
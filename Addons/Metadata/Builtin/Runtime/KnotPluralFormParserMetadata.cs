#if KNOT_LOCALIZATION_EXPERIMENTAL
using Knot.Localization.Attributes;
using System;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace Knot.Localization.Data
{
    [Serializable]
    [KnotMetadataInfo("Plural Form Parser", KnotMetadataInfoAttribute.MetadataScope.Text, AllowMultipleInstances = false)]
    public class KnotPluralFormParserMetadata : IKnotPluralFormParserMetadata
    {
        public object Clone() => new KnotPluralFormParserMetadata();

        public void Format(StringBuilder sb, CultureInfo cultureInfo) =>
            TryFormat(sb, cultureInfo, KnotPluralForm.One);

        public void Format(StringBuilder sb) =>
            TryFormat(sb, CultureInfo.DefaultThreadCurrentCulture, KnotPluralForm.One);

        public bool TryFormat(StringBuilder sb, CultureInfo cultureInfo, KnotPluralForm pluralForm)
        {
            return true;
        }
    }
}
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Knot.Localization.Data;
using UnityEngine;

namespace Knot.Localization
{
    public interface IKnotPluralFormParserMetadata : IKnotTextFormatterMetadata
    {
        bool TryFormat(StringBuilder sb, CultureInfo cultureInfo, KnotPluralForm pluralForm);
    }
}

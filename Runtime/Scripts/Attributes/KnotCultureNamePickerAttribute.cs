using System;
using System.Globalization;
using UnityEngine;

namespace Knot.Localization.Attributes
{
    /// <summary>
    /// Special attribute for <see cref="string"/> type fields that allows to pick <see cref="CultureInfo.Name"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class KnotCultureNamePickerAttribute : PropertyAttribute
    {

    }
}
using System;
using UnityEngine;

namespace Knot.Localization.Attributes
{
    /// <summary>
    /// Special attribute for <see cref="string"/> type fields or arrays that allows to pick text key
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class KnotTextKeyPickerAttribute : PropertyAttribute
    {

    }
}
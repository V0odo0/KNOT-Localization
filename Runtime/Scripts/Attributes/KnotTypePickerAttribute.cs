using System;
using UnityEngine;

namespace Knot.Localization.Attributes
{
    /// <summary>
    /// Special attribute for <see cref="Type"/> dependent types and fields
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class KnotTypePickerAttribute : PropertyAttribute
    {
        public readonly Type BaseType;

        public KnotTypePickerAttribute(Type baseType)
        {
            BaseType = baseType;
        }
    }
}
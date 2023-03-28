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
        public readonly bool AllowSameTypeInArray;


        public KnotTypePickerAttribute(Type baseType, bool allowMultipleTypesInArray = true)
        {
            BaseType = baseType;
            AllowSameTypeInArray = allowMultipleTypesInArray;
        }
    }
}
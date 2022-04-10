using System;
using UnityEngine;

namespace Knot.Localization.Attributes
{
    /// <summary>
    /// Special attribute for <see cref="string"/> type fields used to ensure that inspector editing string field
    /// will always have at least one character or equal to <see cref="FallbackString"/> 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class KnotNotNullStringAttribute : PropertyAttribute
    {
        public readonly string FallbackString = "Empty";


        public KnotNotNullStringAttribute() { }

        public KnotNotNullStringAttribute(string fallbackString = "Empty")
        {
            FallbackString = fallbackString;
        }
    }
}
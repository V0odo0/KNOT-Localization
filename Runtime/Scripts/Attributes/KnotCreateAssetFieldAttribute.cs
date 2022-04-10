using System;
using UnityEngine;

namespace Knot.Localization.Attributes
{
    /// <summary>
    /// Special attribute for <see cref="UnityEngine.Object"/> type fields that allows to create and assign assets derived from <see cref="AssetType"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class KnotCreateAssetFieldAttribute : PropertyAttribute
    {
        public Type AssetType;


        public KnotCreateAssetFieldAttribute(Type assetType)
        {
            AssetType = assetType;
        }
    }
}
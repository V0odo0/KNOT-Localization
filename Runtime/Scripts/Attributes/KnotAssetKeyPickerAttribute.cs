using System;

namespace Knot.Localization.Attributes
{
    /// <summary>
    /// Special attribute for <see cref="string"/> type fields or arrays that allows to pick asset key in inspector
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class KnotAssetKeyPickerAttribute : KnotTypePickerAttribute
    {
        public KnotAssetKeyPickerAttribute() : base(typeof(UnityEngine.Object)) { }

        public KnotAssetKeyPickerAttribute(Type assetType) : base(assetType) { }
    }
}
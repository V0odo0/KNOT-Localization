using System;
using Knot.Localization.Attributes;
using UnityEngine;

namespace Knot.Localization.Data
{
    /// <summary>
    /// Editor-only <see cref="IKnotMetadata"/> for <see cref="KnotAssetData"/>. Used to restrict asset type that can be assigned to <see cref="KnotAssetData.Asset"/>
    /// </summary>
    [Serializable]
    [KnotMetadataInfo("Asset Type Restriction", KnotMetadataInfoAttribute.MetadataScope.Asset, false, true)]
    public class KnotAssetTypeRestrictionMetadata : IKnotMetadata
    {
        /// <summary>
        /// <see cref="UnityEngine.Object"/> derived type. Returns <see cref="UnityEngine.Object"/> if <see cref="AssetType"/>
        /// is not assigned or not derived from <see cref="UnityEngine.Object"/>
        /// </summary>
        public Type AssetType
        {
            get
            {
                if (string.IsNullOrEmpty(_assemblyQualifiedTypeName))
                    return typeof(UnityEngine.Object);

                var type = Type.GetType(_assemblyQualifiedTypeName);
                if (type == null || !type.IsSubclassOf(typeof(UnityEngine.Object)))
                    return typeof(UnityEngine.Object);

                return type;
            }
            set => _assemblyQualifiedTypeName = value == null ? string.Empty : value.AssemblyQualifiedName;
        }
        [SerializeField] private string _assemblyQualifiedTypeName;


        public KnotAssetTypeRestrictionMetadata() { }

        public KnotAssetTypeRestrictionMetadata(Type type)
        {
            AssetType = type;
        }


        public object Clone() => new KnotAssetTypeRestrictionMetadata(AssetType);
    }
}
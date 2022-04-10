using System;
using Knot.Localization.Data;

namespace Knot.Localization.Attributes
{
    /// <summary>
    /// Special attribute that should be applied to <see cref="IKnotMetadata"/> derived types in order to limit <see cref="MetadataScope"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class KnotMetadataInfoAttribute : KnotTypeInfoAttribute
    {
        /// <summary>
        /// Defines <see cref="IKnotMetadata"/> scope
        /// </summary>
        public MetadataScope Scope = MetadataScope.All;

        /// <summary>
        /// If enabled, only one instance of <see cref="IKnotMetadata"/> can be added to <see cref="KnotMetadataContainer"/>
        /// </summary>
        public bool AllowMultipleInstances = true;

        /// <summary>
        /// If enabled, <see cref="IKnotMetadata"/> can be added only to
        /// <see cref="KnotMetadataContainer.Editor"/> collection and will be excluded from build
        /// </summary>
        public bool IsEditorOnly;


        public KnotMetadataInfoAttribute(string displayName) : base(displayName) { }

        public KnotMetadataInfoAttribute(string displayName, MetadataScope scope = MetadataScope.All, bool allowMultipleInstances = false, bool isEditorOnly = false) : base(displayName)
        {
            Scope = scope;
            AllowMultipleInstances = allowMultipleInstances;
            IsEditorOnly = isEditorOnly;
        }

        /// <summary>
        /// <see cref="IKnotMetadata"/> scope
        /// </summary>
        [Flags]
        public enum MetadataScope
        {
            None = 0,
            /// <summary>
            /// <see cref="IKnotMetadata"/> related to <see cref="KnotLanguageData"/>
            /// </summary>
            Language = 1,
            /// <summary>
            /// <see cref="IKnotMetadata"/> related to <see cref="KnotTextData"/>
            /// </summary>
            Text = 2,
            /// <summary>
            /// <see cref="IKnotMetadata"/> related to <see cref="KnotAssetData"/>
            /// </summary>
            Asset = 4,
            /// <summary>
            /// <see cref="IKnotMetadata"/> related to <see cref="KnotDatabase"/>
            /// </summary>
            Database = 8,
            All = ~0
        }
    }
}

using System;

namespace Knot.Localization.Data
{
    /// <summary>
    /// Base interface for all kind of Metadata. Apply <see cref="Attributes.KnotMetadataInfoAttribute"/> attribute to restrict metadata scope.
    /// <see cref="ICloneable.Clone"/> should be implemented.
    /// </summary>
    public interface IKnotMetadata : ICloneable
    {
        
    }
}
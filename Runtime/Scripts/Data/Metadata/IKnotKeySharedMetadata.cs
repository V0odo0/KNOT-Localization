namespace Knot.Localization.Data
{
    /// <summary>
    /// An interface that is used by <see cref="KnotController{TItemData,TValue,TValueType}"/> to share this <see cref="IKnotMetadata"/> between all keys
    /// if it scoped to <see cref="Attributes.KnotMetadataInfoAttribute.MetadataScope.Database"/>
    /// or <see cref="Attributes.KnotMetadataInfoAttribute.MetadataScope.Language"/>
    /// </summary>
    public interface IKnotKeySharedMetadata : IKnotMetadata
    {
        
    }
}
using Object = UnityEngine.Object;

namespace Knot.Localization.Data
{
    /// <summary>
    /// An interface that should implement asset <see cref="Format"/> logic.
    /// <see cref="Format"/> is called by <see cref="KnotAsset"/> to update <see cref="KnotAsset.Value"/>
    /// </summary>
    public interface IKnotAssetFormatterMetadata : IKnotMetadata
    {
        Object Format(Object value);
    }
}
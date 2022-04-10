namespace Knot.Localization.Data
{
    /// <summary>
    /// An interface that should be implemented as addition to <see cref="IKnotRuntimeItemCollectionProvider"/>
    /// in order to be accessible by Database Editor and Key Reference Picker.
    /// </summary>
    public interface IKnotPersistentItemCollectionProvider : IKnotItemCollectionProvider
    {
        /// <summary>
        /// Direct reference to <see cref="KnotItemCollection"/> asset
        /// </summary>
        KnotItemCollection Collection { get; }
    }
}
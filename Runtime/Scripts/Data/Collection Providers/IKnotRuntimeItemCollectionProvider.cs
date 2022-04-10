using System.Threading.Tasks;

namespace Knot.Localization.Data
{
    /// <summary>
    /// An interface that should implement runtime <see cref="IKnotItemCollection{TItemData}"/> loading / unloading behaviour
    /// </summary>
    public interface IKnotRuntimeItemCollectionProvider: IKnotItemCollectionProvider
    {
        /// <summary>
        /// Loads <see cref="IKnotItemCollection{TItemData}"/> asynchronously. Used by <see cref="IKnotManager"/> to load language data
        /// </summary>
        Task<KnotItemCollection> LoadAsync();

        /// <summary>
        /// Unloads <see cref="IKnotItemCollection{TItemData}"/>. Used by <see cref="IKnotManager"/> to unload language data
        /// </summary>
        void Unload();
    }
}
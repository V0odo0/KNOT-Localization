using System;
using System.Threading.Tasks;
using Knot.Localization.Attributes;
using UnityEngine;

namespace Knot.Localization.Data
{
    /// <summary>
    /// Loads <see cref="KnotItemCollection"/> from <see cref="Resources"/> from relative <see cref="Path"/>
    /// </summary>
    [Serializable]
    [KnotTypeInfo("Resource")]
    public sealed class KnotResourceCollectionProvider : 
        IKnotRuntimeItemCollectionProvider,
        IKnotPersistentItemCollectionProvider
    {
        /// <summary>
        /// Determines if asset should be loaded asynchronously via <see cref="Resources"/>.<see cref="Resources.LoadAsync{T}"/>
        /// </summary>
        [Tooltip("Determines if asset should be loaded asynchronously via Resources.LoadAsync")]
        public bool AsyncLoad = true;

        /// <summary>
        /// Path to asset relative to <see cref="Resources"/> folder e.g. TextCollections/KnoTextCollection_en
        /// </summary>
        [Tooltip("Path to asset relative to Resources folder e.g. \"TextCollections/KnotTextCollection\"")]
        public string Path;

        public KnotItemCollection Collection => _cachedCollection = Resources.Load<KnotItemCollection>(Path);
        private KnotItemCollection _cachedCollection;


        public KnotResourceCollectionProvider() {  }
        
        public KnotResourceCollectionProvider(string path, bool asyncLoad = true)
        {
            Path = path;
            AsyncLoad = asyncLoad;
        }


        public Task<KnotItemCollection> LoadAsync()
        {
            if (!AsyncLoad)
                return Task.FromResult(_cachedCollection = Resources.Load<KnotItemCollection>(Path));

            var task = new TaskCompletionSource<KnotItemCollection>();
            var request = Resources.LoadAsync<KnotItemCollection>(Path);
            request.completed += operation =>
            {
                task.SetResult(_cachedCollection = request.asset as KnotItemCollection);
            };

            return task.Task;
        }

        public void Unload()
        {
            if (_cachedCollection != null)
            {
                Resources.UnloadAsset(_cachedCollection);
                _cachedCollection = null;
            }
        }
    }
}
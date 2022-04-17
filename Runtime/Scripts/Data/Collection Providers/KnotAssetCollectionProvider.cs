using System;
using System.Threading.Tasks;
using Knot.Localization.Attributes;
using UnityEngine;

namespace Knot.Localization.Data
{
    /// <summary>
    /// An implementation of <see cref="IKnotItemCollectionProvider"/> with direct reference to <see cref="KnotItemCollection"/> asset.
    /// </summary>
    [Serializable]
    [KnotTypeInfo("Asset")]
    public class KnotAssetCollectionProvider : IKnotRuntimeItemCollectionProvider, IKnotPersistentItemCollectionProvider
    {
        /// <summary>
        /// Direct reference to <see cref="KnotItemCollection"/>
        /// </summary>
        public KnotItemCollection Collection => _collection;
        [SerializeField, KnotCreateAssetField(typeof(KnotItemCollection))] private KnotItemCollection _collection;

        
        public KnotAssetCollectionProvider() { }

        public KnotAssetCollectionProvider(KnotItemCollection collection)
        {
            _collection = collection;
        }
        

        public Task<KnotItemCollection> LoadAsync() => Task.FromResult(_collection);

        public void Unload() { }
    }
}
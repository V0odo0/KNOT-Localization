#if KNOT_ADDRESSABLES
using System;
using System.Threading.Tasks;
using Knot.Localization.Attributes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Knot.Localization.Data
{
    /// <summary>
    /// Loads <see cref="KnotItemCollection"/> trough <see cref="Addressables"/>
    /// </summary>
    [Serializable]
    [KnotTypeInfo("Addressable")]
    public sealed class KnotAddressableCollectionProvider : 
        IKnotRuntimeItemCollectionProvider,
        IKnotPersistentItemCollectionProvider
    {
        public KnotItemCollection Collection => _assetReference?.editorAsset;
        [SerializeField] private KnotAddressableItemCollectionReference _assetReference;

        [NonSerialized] private AsyncOperationHandle<KnotItemCollection> _asyncOperationHandle;


        public async Task<KnotItemCollection> LoadAsync()
        {
            _asyncOperationHandle = _assetReference.LoadAssetAsync();
            await _asyncOperationHandle.Task;
            return _asyncOperationHandle.Result;
        }

        public void Unload()
        {
            Addressables.Release(_asyncOperationHandle);
        }
    }
}
#endif
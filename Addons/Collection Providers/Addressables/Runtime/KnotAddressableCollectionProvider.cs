#if KNOT_ADDRESSABLES
#pragma warning disable CS0649
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
        public KnotItemCollection Collection
        {
            get
            {
#if UNITY_EDITOR
                return AssetReference.editorAsset;
#else
                return _asyncOperationHandle.Result;
#endif
            }
        }

        public KnotAddressableItemCollectionReference AssetReference;

        [NonSerialized] private AsyncOperationHandle<KnotItemCollection> _asyncOperationHandle;


        public async Task<KnotItemCollection> LoadAsync()
        {
            if (AssetReference.RuntimeKeyIsValid())
            {
                _asyncOperationHandle = AssetReference.LoadAssetAsync();
                await _asyncOperationHandle.Task;
                return _asyncOperationHandle.Result;
            }

            return null;
        }

        public void Unload()
        {
			if (_asyncOperationHandle.IsValid())
				Addressables.Release(_asyncOperationHandle);
        }
    }
}
#endif
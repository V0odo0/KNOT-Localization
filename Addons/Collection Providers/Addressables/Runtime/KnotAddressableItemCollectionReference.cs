#if KNOT_ADDRESSABLES
using System;
using UnityEngine.AddressableAssets;

namespace Knot.Localization.Data
{
    [Serializable]
    public class KnotAddressableItemCollectionReference : AssetReferenceT<KnotItemCollection>
    {
        public KnotAddressableItemCollectionReference(string guid) : base(guid)
        {
        }
    }
}
#endif
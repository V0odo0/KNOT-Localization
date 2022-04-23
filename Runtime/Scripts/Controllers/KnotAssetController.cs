using System;
using Knot.Localization.Attributes;
using Knot.Localization.Data;
using Object = UnityEngine.Object;

namespace Knot.Localization
{
    [Serializable]
    [KnotTypeInfo("Default Asset Controller")]
    public class KnotAssetController : KnotController<KnotAssetData, IKnotAsset, Object>, IKnotAssetController
    {
        protected override IKnotAsset CreateValueFromItemData(KnotAssetData itemData, params IKnotMetadata[] metadata)
            => new KnotAsset(itemData?.Asset, metadata);

        protected override IKnotAsset CreateEmptyValue(string key, params IKnotMetadata[] metadata)
            => new KnotAsset(null, metadata);

        protected override IKnotAsset CreateValue(Object value, params IKnotMetadata[] metadata)
            => new KnotAsset(value, metadata);

        
        public override Object GetFallbackValue(string key) => null;

        public override object Clone() => new KnotAssetController();
    }
}
using System;
using System.Collections.Generic;
using Knot.Localization.Data;
using Object = UnityEngine.Object;

namespace Knot.Localization
{
    [Serializable]
    public class KnotAssetKeyReference : KnotKeyReference<Object>
    {
        public override Object Value => KnotLocalization.GetAsset(Key);
        public override IEnumerable<IKnotMetadata> Metadata => KnotLocalization.Manager.GetAssetValue(Key)?.Metadata ?? Array.Empty<IKnotMetadata>();


        public KnotAssetKeyReference() { }

        public KnotAssetKeyReference(string key) : base(key) { }


        protected override void RegisterValueUpdatedCallback(string key, Action<Object> valueUpdated)
        {
            KnotLocalization.RegisterAssetUpdatedCallback(Key, valueUpdated);
        }

        protected override void UnRegisterValueUpdatedCallback(string key, Action<Object> valueUpdated)
        {
            KnotLocalization.UnRegisterAssetUpdatedCallback(Key, valueUpdated);
        }
    }
}
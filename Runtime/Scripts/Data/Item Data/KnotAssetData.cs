using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Knot.Localization.Data
{
    /// <summary>
    /// Base class that is used to store reference to <see cref="Asset"/>
    /// </summary>
    [Serializable]
    public partial class KnotAssetData : KnotItemData
    {
        public Object Asset
        {
            get => _asset;
            set => _asset = value;
        }
        [SerializeField] private Object _asset;


        public KnotAssetData() { }

        public KnotAssetData(string key) : base(key) { }

        public KnotAssetData(string key, Object asset) : base(key)
        {
            _asset = asset;
        }

        public KnotAssetData(KnotAssetData other)
        {
            _key = other._key;
            _asset = other._asset;
        }

        public KnotAssetData(string key, KnotAssetData other)
        {
            _key = key;
            _asset = other._asset;
        }


        public override object Clone() => new KnotAssetData(_key, _asset);
    }
}

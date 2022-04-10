using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Knot.Localization.Data
{
    [Serializable]
    public partial class KnotKeyData
    {
        public string Key
        {
            get => _key;
            set => _key = value;
        }
        [SerializeField] private string _key = string.Empty;
        
        public KnotMetadataContainer Metadata => _metadata;
        [SerializeField] private KnotMetadataContainer _metadata = new KnotMetadataContainer();


        public KnotKeyData() { }

        public KnotKeyData(string key)
        {
            _key = key;
        }

        public KnotKeyData(string key, KnotKeyData other)
        {
            _key = key;
            _metadata = new KnotMetadataContainer(other.Metadata);
        }

        public KnotKeyData(string key, params IKnotMetadata[] runtimeMetadata)
        {
            _key = key;
            _metadata.Runtime.AddRange(runtimeMetadata);
        }

        public KnotKeyData(KnotKeyData other)
        {
            _key = other.Key;
            _metadata = new KnotMetadataContainer(other.Metadata);
        }
    }
}


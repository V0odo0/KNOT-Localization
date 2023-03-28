using Knot.Localization.Attributes;
using System;
using System.Text;
using UnityEngine;

namespace Knot.Localization.Data
{
    [Serializable]
    [KnotMetadataInfo("Prefix", KnotMetadataInfoAttribute.MetadataScope.Text, AllowMultipleInstances = true)]
    public class KnotPrefixMetadata : IKnotTextFormatterMetadata
    {
        public string Prefix
        {
            get => _prefix;
            set => _prefix = value;
        }
        [SerializeField] private string _prefix;


        public KnotPrefixMetadata() { }

        public KnotPrefixMetadata(string prefix)
        {
            _prefix = prefix;
        }

        public object Clone() => new KnotPrefixMetadata(Prefix);

        public void Format(StringBuilder sb)
        {
            sb.Insert(0, Prefix);
        }
    }
}

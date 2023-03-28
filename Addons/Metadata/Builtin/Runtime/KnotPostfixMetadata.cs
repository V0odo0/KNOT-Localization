using Knot.Localization.Attributes;
using System;
using System.Text;
using UnityEngine;

namespace Knot.Localization.Data
{
    [Serializable]
    [KnotMetadataInfo("Postfix", KnotMetadataInfoAttribute.MetadataScope.Text, AllowMultipleInstances = true)]
    public class KnotPostfixMetadata : IKnotTextFormatterMetadata
    {
        public string Postfix
        {
            get => _postfix;
            set => _postfix = value;
        }
        [SerializeField] private string _postfix;


        public KnotPostfixMetadata() { }

        public KnotPostfixMetadata(string postfix)
        {
            _postfix = postfix;
        }

        public object Clone() => new KnotPostfixMetadata(Postfix);

        public void Format(StringBuilder sb)
        {
            sb.Append(Postfix);
        }
    }
}

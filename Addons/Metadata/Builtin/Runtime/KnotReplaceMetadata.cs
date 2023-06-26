using Knot.Localization.Attributes;
using System;
using System.Text;
using UnityEngine;

namespace Knot.Localization.Data
{
    [Serializable]
    [KnotMetadataInfo("Replace", KnotMetadataInfoAttribute.MetadataScope.Text, AllowMultipleInstances = true)]
    public class KnotReplaceMetadata : IKnotTextFormatterMetadata
    {
        public string OldValue
        {
            get => _oldValue;
            set => _oldValue = value;
        }
        [SerializeField] private string _oldValue;

        public string NewValue
        {
            get => _newValue;
            set => _newValue = value;
        }
        [SerializeField] private string _newValue;


        public KnotReplaceMetadata() { }

        public KnotReplaceMetadata(string oldValue, string newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public object Clone() => new KnotReplaceMetadata(OldValue, NewValue);

        public void Format(StringBuilder sb)
        {
            if (string.IsNullOrEmpty(OldValue))
                return;

            sb.Replace(OldValue, NewValue);
        }
    }
}

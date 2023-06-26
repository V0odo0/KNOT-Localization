using Knot.Localization.Attributes;
using System;
using System.Text;
using UnityEngine;

namespace Knot.Localization.Data
{
    [Serializable]
    [KnotMetadataInfo("Replace with Localized Text", KnotMetadataInfoAttribute.MetadataScope.Text, AllowMultipleInstances = false)]
    public class KnotReplaceWithLocalizedTextMetadata : IKnotTextFormatterMetadata
    {
        public string OldValue
        {
            get => _oldValue;
            set => _oldValue = value;
        }
        [SerializeField] private string _oldValue;

        public KnotTextKeyReference NewValue
        {
            get => _newValue;
            set => _newValue = value;
        }
        [SerializeField] private KnotTextKeyReference _newValue;


        public KnotReplaceWithLocalizedTextMetadata() { }

        public KnotReplaceWithLocalizedTextMetadata(string oldValue, string newValueKey)
        {
            _oldValue = oldValue;
            _newValue = new KnotTextKeyReference(newValueKey);
        }


        public object Clone() => (MemberwiseClone() as KnotReplaceWithLocalizedTextMetadata) ?? new KnotReplaceWithLocalizedTextMetadata();
        
        public void Format(StringBuilder sb)
        {
            if (string.IsNullOrEmpty(OldValue))
                return;

            sb.Replace(OldValue, NewValue.Value);
        }
    }
}

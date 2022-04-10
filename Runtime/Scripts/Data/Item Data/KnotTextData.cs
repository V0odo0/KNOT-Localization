using System;
using UnityEngine;

namespace Knot.Localization.Data
{
    /// <summary>
    /// Base class that is used to store raw text value
    /// </summary>
    [Serializable]
    public partial class KnotTextData : KnotItemData
    {
        public virtual string RawText
        {
            get => _rawText;
            set => _rawText = value;
        }
        [SerializeField] private string _rawText = string.Empty;


        public KnotTextData() { }

        public KnotTextData(string key) : base(key) { }

        public KnotTextData(string key, string rawText = "") : base(key)
        {
            _rawText = rawText;
        }

        public KnotTextData(KnotTextData other)
        {
            _key = other._key;
            _rawText = other._rawText;
        }

        public KnotTextData(string key, KnotTextData other)
        {
            _key = key;
            _rawText = other._rawText;
        }


        public override object Clone() => new KnotTextData(_key, _rawText);
    }
}
using System;
using UnityEngine;

namespace Knot.Localization.Data
{
    public abstract partial class KnotItemData : ICloneable, IComparable<KnotItemData>
    {
        public string Key
        {
            get => _key;
            set => _key = value;
        }
        [SerializeField] protected string _key = string.Empty;


        protected KnotItemData() { }

        protected KnotItemData(string key = "")
        {
            _key = key;
        }


        public abstract object Clone();

        public virtual int CompareTo(KnotItemData other)
            => other == null ? 0 : string.Compare(Key, other.Key, StringComparison.Ordinal);
    }
}


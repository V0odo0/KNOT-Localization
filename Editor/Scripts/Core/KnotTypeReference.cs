using System;
using UnityEngine;

namespace Knot.Localization.Editor
{
    [Serializable]
    public class KnotTypeReference
    {
        public Type Type
        {
            get => string.IsNullOrEmpty(_typeFullName) ? null : Type.GetType(_typeFullName);
            set => _typeFullName = value == null ? string.Empty : value.FullName;
        }
        [SerializeField] private string _typeFullName;


        public KnotTypeReference() { }

        public KnotTypeReference(Type type)
        {
            Type = type;
        }

        public override int GetHashCode() => Type.GetHashCode();

        public override string ToString() => Type?.ToString();
    }
}
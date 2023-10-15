using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Knot.Localization.Data
{
    /// <summary>
    /// Asset that is used to store <see cref="KnotKeyData"/> collection
    /// </summary>
    [CreateAssetMenu(fileName = "KeyCollection", menuName = KnotLocalization.CorePath + "Key Collection", order = 50)]
    public partial class KnotKeyCollection : ScriptableObject, IList<KnotKeyData>
    {
        internal static KnotKeyCollection Empty => _empty ?? (_empty = CreateInstance<KnotKeyCollection>());
        private static KnotKeyCollection _empty;


        public virtual KnotKeyData this[int index]
        {
            get => _keyData[index];
            set => _keyData[index] = value;
        }
        public virtual KnotKeyData this[string key]
        {
            get => _keyData.FirstOrDefault(d => d.Key == key);
            set => _keyData[_keyData.IndexOf(_keyData.FirstOrDefault(d => d.Key == key))] = value;
        }

        public virtual bool IsReadOnly => false;
        public virtual int Count => _keyData.Count;


        [SerializeField] private List<KnotKeyData> _keyData = new List<KnotKeyData>();


        IEnumerator IEnumerable.GetEnumerator() => _keyData.GetEnumerator();

        
        public virtual void Add(KnotKeyData item) => _keyData.Add(item);
        
        public virtual void Clear() => _keyData.Clear();

        public virtual bool Contains(KnotKeyData item) => _keyData.Contains(item);

        public virtual bool ContainsKey(string key) => _keyData.Exists(d => d.Key == key);

        public virtual void CopyTo(KnotKeyData[] array, int arrayIndex) => _keyData.CopyTo(array, arrayIndex);

        public virtual bool Remove(KnotKeyData item) => _keyData.Remove(item);

        public virtual bool RemoveKey(string key) => ContainsKey(key) && Remove(this[key]);

        public virtual int IndexOf(KnotKeyData item) => _keyData.IndexOf(item);

        public virtual void Insert(int index, KnotKeyData item) => _keyData.Insert(index, item);

        public virtual void RemoveAt(int index) => _keyData.RemoveAt(index);

        public virtual IEnumerator<KnotKeyData> GetEnumerator() => _keyData.GetEnumerator();
    }
}
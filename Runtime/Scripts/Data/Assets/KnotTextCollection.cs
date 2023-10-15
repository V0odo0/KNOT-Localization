using System.Collections;
using System.Collections.Generic;
using Knot.Localization.Attributes;
using UnityEngine;

namespace Knot.Localization.Data
{
    [KnotTypeInfo("Text Collection")]
    [CreateAssetMenu(fileName = "KnotTextCollection", menuName = KnotLocalization.CorePath + "Text Collection", order = 50)]
    public class KnotTextCollection : KnotItemCollection, IKnotItemCollection<KnotTextData>
    {
        public KnotTextData this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        public int Count => _items.Count;

        public bool IsReadOnly => false;


        [SerializeField] private List<KnotTextData> _items = new List<KnotTextData>();


        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<KnotTextData> GetEnumerator() => _items.GetEnumerator();

        public void Add(KnotTextData itemData) => _items.Add(itemData);

        public void Clear() => _items.Clear();

        public bool Contains(KnotTextData itemData) => _items.Contains(itemData);

        public void CopyTo(KnotTextData[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

        public bool Remove(KnotTextData itemData) => _items.Remove(itemData);


        public int IndexOf(KnotTextData itemData) => _items.IndexOf(itemData);

        public void Insert(int index, KnotTextData itemData) => _items.Insert(index, itemData);

        public void RemoveAt(int index) => _items.RemoveAt(index);
    }
}
using System.Collections;
using System.Collections.Generic;
using Knot.Localization.Attributes;
using UnityEngine;

namespace Knot.Localization.Data
{
    [KnotTypeInfo("Asset Collection")]
    [CreateAssetMenu(fileName = "KnotAssetCollection", menuName = KnotLocalization.CorePath + "Asset Collection", order = 50)]
    public class KnotAssetCollection : KnotItemCollection, IKnotItemCollection<KnotAssetData>
    {
        public KnotAssetData this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        public int Count => _items.Count;
        public bool IsReadOnly => false;

        [SerializeField] private List<KnotAssetData> _items = new List<KnotAssetData>();


        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<KnotAssetData> GetEnumerator() => _items.GetEnumerator();

        public void Add(KnotAssetData itemData) => _items.Add(itemData);

        public void Clear() => _items.Clear();

        public bool Contains(KnotAssetData itemData) => _items.Contains(itemData);

        public void CopyTo(KnotAssetData[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

        public bool Remove(KnotAssetData itemData) => _items.Remove(itemData);


        public int IndexOf(KnotAssetData itemData) => _items.IndexOf(itemData);

        public void Insert(int index, KnotAssetData itemData) => _items.Insert(index, itemData);

        public void RemoveAt(int index) => _items.RemoveAt(index);
    }
}

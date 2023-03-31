using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Knot.Localization.Data;
using UnityEngine;

namespace Knot.Localization
{
    /// <summary>
    /// Base implementation of <see cref="IKnotController{TItemData,TValue,TValueType}"/>
    /// </summary>
    public abstract class KnotController<TItemData, TValue, TValueType> : IKnotController<TItemData, TValue, TValueType>
        where TItemData : KnotItemData
        where TValue : class, IKnotValue<TValueType>
    {
        public virtual TValue this[string key] => TryGetValue(key, out var value) ? value : null;
        public virtual int Count => BaseValues.Count + Overrides.Count;

        public virtual IEnumerable<string> Keys => BaseValues.Keys.Union(Overrides.Keys);
        public virtual IEnumerable<TValue> Values => BaseValues.Values.Union(Overrides.Values);

        public virtual IReadOnlyDictionary<string, TValue> BaseValues => _baseValues ?? (_baseValues = new Dictionary<string, TValue>());
        [NonSerialized] private Dictionary<string, TValue> _baseValues;

        public virtual IReadOnlyDictionary<string, TValue> Overrides => _overrides ?? (_overrides = new Dictionary<string, TValue>());
        [NonSerialized] private Dictionary<string, TValue> _overrides;

        public virtual IDictionary<string, Action<TValueType>> ValueChangedCallbacks =>
            _valueChangedCallbacks ?? (_valueChangedCallbacks = new Dictionary<string, Action<TValueType>>());
        [NonSerialized] private Dictionary<string, Action<TValueType>> _valueChangedCallbacks;

        protected virtual KnotControllerBuildData<TItemData> CurrentBuildData
        {
            get => _currentBuildData;
            set => _currentBuildData = value;
        }
        [NonSerialized] private KnotControllerBuildData<TItemData> _currentBuildData;

        //bug: Unity refuses to serialize classes with no serializable fields, so we put a placeholder to fix it (Issue ID: 1183547)
        [SerializeField, HideInInspector] private bool _serializationPlaceholder;


        public Task BuildAsync(KnotControllerBuildData<TItemData> buildData)
        {
            _currentBuildData = buildData;

            if (_baseValues == null)
                _baseValues = new Dictionary<string, TValue>();
            else _baseValues.Clear();
            
            IKnotMetadata[] keySharedMetadata = buildData.GlobalMetadata.OfType<IKnotKeySharedMetadata>().
                Union(buildData.LanguageMetadata.OfType<IKnotKeySharedMetadata>()).ToArray();
            UpdateCultureSpecificMetadata(keySharedMetadata);

            foreach (var data in GetCombinedKeyData(buildData).AsParallel())
            {
                var keyMetadata = data.Value.KeyData?.Metadata.ToArray() ?? Array.Empty<IKnotMetadata>();
                UpdateCultureSpecificMetadata(keyMetadata);

                var metadata = keySharedMetadata.Union(keyMetadata).ToArray();

                var value = data.Value.ItemData != null
                    ? CreateValueFromItemData(data.Value.ItemData, metadata)
                    : CreateEmptyValue(data.Key, metadata);

                _baseValues.Add(data.Key, value);
            }

            InvokeValueChangedCallbacks(ValueChangedCallbacks.Keys.ToArray());

            return Task.CompletedTask;
        }


        protected abstract TValue CreateValueFromItemData(TItemData itemData, params IKnotMetadata[] metadata);

        protected abstract TValue CreateEmptyValue(string key, params IKnotMetadata[] metadata);

        protected abstract TValue CreateValue(TValueType value, params IKnotMetadata[] metadata);

        protected virtual Dictionary<string, CombinedKeyData> GetCombinedKeyData(KnotControllerBuildData<TItemData> buildData)
        {
            Dictionary<string, CombinedKeyData> combinedData = new Dictionary<string, CombinedKeyData>();

            foreach (var keyData in buildData.KeyData)
            {
                if (!combinedData.ContainsKey(keyData.Key))
                    combinedData.Add(keyData.Key, new CombinedKeyData(keyData));
                else combinedData[keyData.Key].KeyData = keyData;
            }

            foreach (var itemData in buildData.ItemData)
            {
                if (!combinedData.ContainsKey(itemData.Key))
                    combinedData.Add(itemData.Key, new CombinedKeyData(itemData));
                else combinedData[itemData.Key].ItemData = itemData;
            }

            return combinedData;
        }

        protected virtual void UpdateCultureSpecificMetadata(params IKnotMetadata[] metadata)
        {
            if (CurrentBuildData == null)
                return;

            foreach (var cultureSpecificMetadata in metadata.OfType<IKnotCultureSpecificMetadata>())
                cultureSpecificMetadata.SetCulture(CurrentBuildData.Culture);
        }
        
        protected virtual void InvokeValueChangedCallbacks(params string[] keys)
        {
            foreach (var key in keys)
                if (ValueChangedCallbacks.ContainsKey(key))
                    ValueChangedCallbacks[key]?.Invoke(ContainsKey(key) ? this[key].Value : GetFallbackValue(key));
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        public virtual IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            return Overrides.Concat(BaseValues).GroupBy(p => p.Key)
                .ToDictionary(key => key.Key, value => value.First().Value).GetEnumerator();
        }

        public virtual bool ContainsKey(string key) => Overrides.ContainsKey(key) || BaseValues.ContainsKey(key);

        public virtual bool TryGetValue(string key, out TValue value) => Overrides.TryGetValue(key, out value) || BaseValues.TryGetValue(key, out value);
        
        public abstract TValueType GetFallbackValue(string key);

        
        ///<inheritdoc/>
        /// <remarks>Note: <see cref="IKnotValue{TValue}.ForceUpdateValue"/> will be called to ensure that all <see cref="IKnotCultureSpecificMetadata"/> is applied</remarks>
        public virtual void AddOverride(string key, TValue value)
        {
            if (string.IsNullOrEmpty(key) || value == null)
                return;

            if (_currentBuildData != null)
            {
                UpdateCultureSpecificMetadata(value.Metadata.ToArray());
                value.ForceUpdateValue();
            }

            if (Overrides.ContainsKey(key))
                _overrides[key] = value;
            else _overrides.Add(key, value);

            InvokeValueChangedCallbacks(key);
        }

        ///<inheritdoc/>
        public virtual void AddOverride(string key, TValueType value, params IKnotMetadata[] metadata)
        {
            UpdateCultureSpecificMetadata(metadata);
            AddOverride(key, CreateValue(value, metadata));
        }

        ///<inheritdoc/>
        public virtual bool RemoveOverride(string key)
        {
            if (!Overrides.ContainsKey(key))
                return false;

            _overrides.Remove(key);
            InvokeValueChangedCallbacks(key);

            return true;
        }

        ///<inheritdoc/>
        public virtual void ClearOverrides()
        {
            if (Overrides.Count == 0)
                return;

            var keys = _overrides.Keys.ToArray();
            _overrides.Clear();

            InvokeValueChangedCallbacks(keys);
        }

        public virtual void Dispose()
        {
            _baseValues?.Clear();
            _overrides?.Clear();
            _valueChangedCallbacks?.Clear();
            _currentBuildData = null;
        }

        public abstract object Clone();


        public virtual void RegisterValueChangedCallback(string key, Action<TValueType> valueChanged)
        {
            if (string.IsNullOrEmpty(key))
                return;

            if (!ValueChangedCallbacks.ContainsKey(key))
                _valueChangedCallbacks.Add(key, valueChanged);
            else _valueChangedCallbacks[key] += valueChanged;
        }

        public virtual void UnRegisterValueChangedCallback(string key, Action<TValueType> valueChanged)
        {
            if (string.IsNullOrEmpty(key))
                return;

            if (!ValueChangedCallbacks.ContainsKey(key))
                return;

            _valueChangedCallbacks[key] -= valueChanged;
            if (ValueChangedCallbacks[key] == null || ValueChangedCallbacks[key].GetInvocationList().Length == 0)
                _valueChangedCallbacks.Remove(key);
        }


        protected class CombinedKeyData
        {
            public KnotKeyData KeyData;
            public TItemData ItemData;


            public CombinedKeyData(KnotKeyData keyData)
            {
                KeyData = keyData;
            }

            public CombinedKeyData(TItemData itemData)
            {
                ItemData = itemData;
            }
        }
    }
}
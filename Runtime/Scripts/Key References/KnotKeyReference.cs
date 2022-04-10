using System;
using System.Collections.Generic;
using Knot.Localization.Data;
using UnityEngine;

namespace Knot.Localization
{
    /// <summary>
    /// Base class for accessing <see cref="IKnotValue{TValue}"/>.<see cref="IKnotValue{TValue}.Value"/> trough <see cref="IKnotController{TItemData,TValue,TValueType}"/> during runtime
    /// </summary>
    public abstract class KnotKeyReference<TValueType>
    {
        /// <summary>
        /// Called whenever <see cref="IKnotValue{TValue}"/> has been updated
        /// </summary>
        public event Action<TValueType> ValueUpdated
        {
            add
            {
                RegisterValueUpdatedCallback(Key, value);
                _valueUpdated += value;
            }
            remove
            {
                UnRegisterValueUpdatedCallback(Key, value);
                _valueUpdated -= value;
            }
        }
        protected Action<TValueType> _valueUpdated;

        /// <summary>
        /// Key used to retrieve <see cref="IKnotValue{TValue}"/>.<see cref="IKnotValue{TValue}.Value"/> from <see cref="IKnotController{TItemData,TValue,TValueType}"/>.
        /// </summary>
        public string Key
        {
            get => _key;
            set
            {
                if (_key == value)
                    return;
                
                _key = value;
                OnKeyChanged(_key, value);
            }
        }
        [SerializeField] private string _key;

        /// <summary>
        /// Reference to <see cref="IKnotValue{TValue}.Value"/>.
        /// </summary>
        public abstract TValueType Value { get; }

        /// <summary>
        /// Reference to <see cref="IKnotValue{TValue}.Metadata"/>.
        /// </summary>
        public abstract IEnumerable<IKnotMetadata> Metadata { get; }


        protected KnotKeyReference() { }

        protected KnotKeyReference(string key) => _key = key;


        protected abstract void RegisterValueUpdatedCallback(string key, Action<TValueType> valueUpdated);

        protected abstract void UnRegisterValueUpdatedCallback(string key, Action<TValueType> valueUpdated);

        protected virtual void OnKeyChanged(string oldKey, string newKey)
        {
            UnRegisterValueUpdatedCallback(oldKey, _valueUpdated);
            RegisterValueUpdatedCallback(newKey, _valueUpdated);
            _valueUpdated?.Invoke(Value);
        }
    }
}
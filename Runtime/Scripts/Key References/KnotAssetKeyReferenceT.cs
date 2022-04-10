using System;
using Object = UnityEngine.Object;

namespace Knot.Localization
{
    public abstract class KnotAssetKeyReferenceT<TValueType> : KnotAssetKeyReference where TValueType : Object
    {
        public new event Action<TValueType> ValueUpdated
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
        protected new Action<TValueType> _valueUpdated;

        public new TValueType Value => base.Value as TValueType;


        void OnValueUpdatedBase(Object obj)
        {
            _valueUpdated?.Invoke(Value);
            base._valueUpdated?.Invoke(Value);
        }

        protected void RegisterValueUpdatedCallback(string key, Action<TValueType> valueUpdated)
        {
            KnotLocalization.RegisterAssetUpdatedCallback(Key, OnValueUpdatedBase);
        }

        protected void UnRegisterValueUpdatedCallback(string key, Action<TValueType> valueUpdated)
        {
            KnotLocalization.UnRegisterAssetUpdatedCallback(Key, OnValueUpdatedBase);
        }

        protected override void OnKeyChanged(string oldKey, string newKey)
        {
            UnRegisterValueUpdatedCallback(oldKey, _valueUpdated);
            RegisterValueUpdatedCallback(newKey, _valueUpdated);
            OnValueUpdatedBase(Value);
        }
    }
}
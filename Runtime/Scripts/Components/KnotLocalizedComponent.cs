using UnityEngine;

namespace Knot.Localization.Components
{
    /// <summary>
    /// Base component that handles <see cref="KnotKeyReference{TValueType}.Value"/>
    /// update in <see cref="OnValueUpdated"/>.
    /// </summary>
    public abstract partial class KnotLocalizedComponent<TKeyReference, TValueType> : MonoBehaviour
        where TKeyReference : KnotKeyReference<TValueType>
    {
        public TKeyReference KeyReference
        {
            get => _keyReference;
            set
            {
                if (value == null)
                    _keyReference.Key = string.Empty;
                else _keyReference = value;
            }
        }
        [SerializeField] private TKeyReference _keyReference;


        protected virtual void OnEnable()
        {
            if (KnotLocalization.Manager.State == KnotManagerState.LanguageLoaded)
                OnValueUpdated(KeyReference.Value);

            KeyReference.ValueUpdated += OnValueUpdated;
        }

        protected virtual void OnDisable()
        {
            KeyReference.ValueUpdated -= OnValueUpdated;
        }

        /// <summary>
        /// Called whenever <see cref="KnotKeyReference{TValueType}.Value"/> has been updated.
        /// </summary>
        /// <param name="value"></param>
        protected abstract void OnValueUpdated(TValueType value);

        /// <summary>
        /// Forces <see cref="OnValueUpdated"/> to be called.
        /// <remarks>Used by Property Drawer to apply changes after <see cref="KeyReference"/> key has been changed.</remarks>
        /// </summary>
        public virtual void ForceUpdateValue()
        {
            if (enabled && Application.IsPlaying(this))
                OnValueUpdated(KeyReference.Value);
        }
    }
}
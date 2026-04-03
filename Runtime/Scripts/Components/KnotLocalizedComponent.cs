using UnityEngine;

namespace Knot.Localization.Components
{
    /// <summary>
    /// Base class for implementing custom localizable components
    /// </summary>
    public abstract partial class KnotLocalizedComponent<TKeyReference, TValueType> : MonoBehaviour
        where TKeyReference : KnotKeyReference<TValueType>
    {
        public TKeyReference KeyReference
        {
            get => _keyReference;
            set => _keyReference = value;
        }
        [SerializeField] private TKeyReference _keyReference;

        private bool _onEnableReached;


        protected virtual void Awake()
        {
            if (KnotLocalization.Manager.State == KnotManagerState.LanguageLoaded)
                OnValueUpdated(KeyReference.Value);
        }
        
        protected virtual void Start()
        {

        }

        protected virtual void OnEnable()
        {
            if (_onEnableReached)
            {
                if (KnotLocalization.Manager.State == KnotManagerState.LanguageLoaded)
                    OnValueUpdated(KeyReference.Value);
            }

            KeyReference.ValueUpdated += OnValueUpdated;
            
            _onEnableReached = true;
        }

        protected virtual void OnDisable()
        {
            KeyReference.ValueUpdated -= OnValueUpdated;
        }

        protected virtual void OnDestroy()
        {

        }

        protected virtual void OnValidate()
        {

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
using UnityEngine;

namespace Knot.Localization.Components
{
    /// <summary>
    /// Base component that handles <see cref="KnotKeyReference{TValueType}.Value"/>
    /// update in <see cref="OnValueUpdated"/> using <see cref="System.Reflection"/>
    /// </summary>
    public class KnotLocalizedMethod<TKeyReference, TValueType, TTargetMethod> : KnotLocalizedComponent<TKeyReference, TValueType>
        where TKeyReference : KnotKeyReference<TValueType>
        where TTargetMethod : KnotTargetMethod<TValueType>
    {
        /// <summary>
        /// Pointer to target method or property
        /// </summary>
        public TTargetMethod Target
        {
            get => _target;
            set => _target = value;
        }
        [SerializeField] private TTargetMethod _target;


        protected override void OnValueUpdated(TValueType value)
        {
            Target?.TrySetValue(value);
        }
    }
}
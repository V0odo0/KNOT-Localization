using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Knot.Localization.Data;

namespace Knot.Localization
{
    /// <summary>
    /// Base interface that should convert <see cref="TItemData"/> values and <see cref="KnotKeyData"/> metadata collection passed from <see cref="KnotControllerBuildData{TItemData}"/>
    /// to key-<see cref="IKnotValue{TValue}"/> dictionary.
    /// </summary>
    public interface IKnotController<TItemData, TValue, TValueType> : IReadOnlyDictionary<string, TValue>, IDisposable, ICloneable
        where TItemData : KnotItemData
        where TValue : class, IKnotValue<TValueType>
    {
        /// <summary>
        /// Runtime key-<see cref="IKnotValue{TValue}"/> dictionary.
        /// </summary>
        IReadOnlyDictionary<string, TValue> Overrides { get; }

        /// <summary>
        /// Dictionary used to store all listeners passed to <see cref="RegisterValueChangedCallback"/>.
        /// </summary>
        IDictionary<string, Action<TValueType>> ValueChangedCallbacks { get; }


        /// <summary>
        /// Builds controller from passed <see cref="KnotControllerBuildData{TItemData}"/>.
        /// </summary>
        Task BuildAsync(KnotControllerBuildData<TItemData> buildData);

        /// <summary>
        /// Replaces or adds new <see cref="IKnotValue{TValue}"/> with given <paramref name="key"/> to <see cref="Overrides"/>
        /// </summary>
        void AddOverride(string key, TValue value);

        /// <summary>
        /// Replaces or adds new value provided with <see cref="IKnotMetadata"/> collection to <see cref="Overrides"/>
        /// </summary>
        void AddOverride(string key, TValueType value, params IKnotMetadata[] metadata);

        /// <summary>
        /// Removes key from <see cref="Overrides"/>.
        /// </summary>
        bool RemoveOverride(string key);

        /// <summary>
        /// Clears <see cref="Overrides"/>
        /// </summary>
        void ClearOverrides();

        /// <summary>
        /// Returns <see cref="IKnotValue{TValue}"/> instance with fallback value for given <paramref name="key"/>
        /// </summary>
        TValueType GetFallbackValue(string key);

        /// <summary>
        /// Registers <paramref name="valueChanged"/> callback that will be invoked when <see cref="IKnotValue{TValue}"/> with given <paramref name="key"/> has been updated
        /// </summary>
        void RegisterValueChangedCallback(string key, Action<TValueType> valueChanged);

        /// <summary>
        /// UnRegisters <paramref name="valueChanged"/> callback with given <paramref name="key"/>
        /// </summary>
        void UnRegisterValueChangedCallback(string key, Action<TValueType> valueChanged);
    }
}

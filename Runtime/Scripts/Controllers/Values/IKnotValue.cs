using System.Collections.Generic;
using Knot.Localization.Data;

namespace Knot.Localization
{
    /// <summary>
    /// Base interface used to store runtime values and <see cref="IKnotMetadata"/> collection. Used by <see cref="IKnotController{TItemData,TValue,TValueType}"/>
    /// to store <see cref="Value"/> from <see cref="KnotItemData"/> and <see cref="IKnotMetadata"/> collection from <see cref="KnotKeyData"/>
    /// </summary>
    public interface IKnotValue<out TValue>
    {
        TValue Value { get; }
        IList<IKnotMetadata> Metadata { get; }


        /// <summary>
        /// Updates and applies all modifications to <see cref="Value"/>
        /// </summary>
        void ForceUpdateValue();
    }
}

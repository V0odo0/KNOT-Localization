using System.Collections.Generic;

namespace Knot.Localization.Data
{
    /// <summary>
    /// Base interface that should implement access to <see cref="KnotItemData"/> collection.
    /// </summary>
    public interface IKnotItemCollection<TItemData> : IList<TItemData> where TItemData : KnotItemData
    {
        
    }
}
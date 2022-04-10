using UnityEngine;

namespace Knot.Localization.Data
{
    /// <summary>
    /// Base asset class that should implement <see cref="IKnotItemCollection{TItem}"/> in order to be accessible by Database Editor and <see cref="IKnotManager"/>
    /// </summary>
    public abstract class KnotItemCollection : ScriptableObject
    {
        internal virtual string Name => name;
    }
}
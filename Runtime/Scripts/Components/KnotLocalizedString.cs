using UnityEngine;

namespace Knot.Localization.Components
{
    /// <summary>
    /// Base component class for localizing <see cref="string"/> properties and methods trough <see cref="System.Reflection"/>
    /// </summary>
    [AddComponentMenu(KnotLocalization.CorePath + "Localized String", 1000)]
    [DisallowMultipleComponent]
    public partial class KnotLocalizedString : KnotLocalizedMethod<KnotTextKeyReference, string, KnotStringTargetMethod>
    {
        
    }
}
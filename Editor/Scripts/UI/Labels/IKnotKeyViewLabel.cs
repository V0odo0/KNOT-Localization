using UnityEngine;

namespace Knot.Localization.Editor
{
    public interface IKnotKeyViewLabel<in TKeyView>
    {
        int Order { get; }

        bool IsAssignableTo(TKeyView keyView);

        GUIContent GetLabelContent(TKeyView keyView);
    }
}
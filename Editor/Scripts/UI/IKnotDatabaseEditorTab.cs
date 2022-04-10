using UnityEngine.UIElements;

namespace Knot.Localization.Editor
{
    public interface IKnotDatabaseEditorTab
    {
        VisualElement RootVisualElement { get; }

        void ReloadLayout();
    }
}

using UnityEditor;
using UnityEngine;

namespace Knot.Localization.Editor
{
    public class KnotAboutEditorWindow : EditorWindow
    {
        public KnotAboutEditorPanel AboutEditorPanel =>
            _aboutEditorPanel ?? (_aboutEditorPanel = new KnotAboutEditorPanel());
        private KnotAboutEditorPanel _aboutEditorPanel;


        void OnEnable()
        {
            ReloadLayout();
        }


        public void ReloadLayout()
        {
            rootVisualElement.Clear();
            rootVisualElement.styleSheets.Add(KnotEditorUtils.EditorStyles);

            rootVisualElement.Add(AboutEditorPanel);
        }


        [MenuItem(KnotEditorUtils.ToolsRootPath + "About", false, 2000)]
        public static void Open()
        {
            var window = GetWindow<KnotAboutEditorWindow>(true, $"About {KnotLocalization.CoreName}");
            window.minSize = new Vector2(385, 175);
            window.maxSize = window.minSize;
        }
    }
}
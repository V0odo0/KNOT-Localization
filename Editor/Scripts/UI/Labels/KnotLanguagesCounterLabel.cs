using UnityEngine;

namespace Knot.Localization.Editor
{
    /*
    public class KnotLanguagesCounterLabel : IKnotKeyViewLabel<KnotTextKeyView>, IKnotKeyViewLabel<KnotAssetKeyView>
    {
        public int Order => 1000;

        protected static GUIContent Label =>
            _label ?? (_label = new GUIContent(KnotEditorUtils.GetIcon(KnotLanguagesTabPanel.LanguageIconName), "Localized values"));
        private static GUIContent _label;


        GUIContent GetLabelContent(int languagesCount)
        {
            Label.text = languagesCount == 0 ? "-" : languagesCount.ToString();
            return Label;
        }


        public bool IsAssignableTo(KnotTextKeyView keyView) => true;

        public GUIContent GetLabelContent(KnotTextKeyView keyView) => GetLabelContent(keyView.LanguageItems.Count);

        public bool IsAssignableTo(KnotAssetKeyView keyView) => true;

        public GUIContent GetLabelContent(KnotAssetKeyView keyView) => GetLabelContent(keyView.LanguageItems.Count);
    }
    */
}
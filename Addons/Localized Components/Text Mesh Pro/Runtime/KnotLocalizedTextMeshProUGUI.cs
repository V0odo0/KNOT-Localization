#if KNOT_TEXTMESHPRO
using TMPro;
using UnityEngine;

namespace Knot.Localization.Components
{
    [AddComponentMenu(KnotLocalization.CoreName + "/Localized Text Mesh PRO", 1000)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public partial class KnotLocalizedTextMeshProUGUI : KnotLocalizedComponent<KnotTextKeyReference, string>
    {
        protected TextMeshProUGUI TextMeshPro => _textMeshPro ?? (_textMeshPro = GetComponent<TextMeshProUGUI>());
        private TextMeshProUGUI _textMeshPro;


        //todo: Font Metadata

        protected override void OnValueUpdated(string value)
        {
            if (TextMeshPro == null)
                return;

            TextMeshPro.text = value;
        }
    }
}
#endif
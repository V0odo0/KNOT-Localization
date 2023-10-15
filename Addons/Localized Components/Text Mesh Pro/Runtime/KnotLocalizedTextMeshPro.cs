#if KNOT_TEXTMESHPRO
using System.Linq;
using Knot.Localization.Data;
using TMPro;
using UnityEngine;

namespace Knot.Localization.Components
{
    [AddComponentMenu(KnotLocalization.CorePath + "Localized Text Mesh PRO", 1000)]
    [DisallowMultipleComponent]
    public partial class KnotLocalizedTextMeshPro : KnotLocalizedComponent<KnotTextKeyReference, string>
    {
        protected TextMeshPro TextMeshPro => _textMeshPro ?? (_textMeshPro = GetComponent<TextMeshPro>());
        private TextMeshPro _textMeshPro;

        protected TextMeshProUGUI TextMeshProUGUI => _textMeshProUGUI ?? (_textMeshProUGUI = GetComponent<TextMeshProUGUI>());
        private TextMeshProUGUI _textMeshProUGUI;


        protected override void OnValueUpdated(string value)
        {
            if (TextMeshPro != null)
            {
                var fontMetadata = KeyReference.Metadata.OfType<KnotTextMeshProCustomFontMetadata>().LastOrDefault();
                if (fontMetadata?.Font != null && TextMeshPro.font != fontMetadata.Font)
                    TextMeshPro.font = fontMetadata.Font;

                TextMeshPro.text = KeyReference.Value;
            }

            if (TextMeshProUGUI != null)
            {
                var fontMetadata = KeyReference.Metadata.OfType<KnotTextMeshProCustomFontMetadata>().LastOrDefault();
                if (fontMetadata?.Font != null && TextMeshProUGUI.font != fontMetadata.Font)
                    TextMeshProUGUI.font = fontMetadata.Font;

                TextMeshProUGUI.text = KeyReference.Value;
            }
        }
    }
}

#endif
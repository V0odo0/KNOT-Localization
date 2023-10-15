#if KNOT_TEXTMESHPRO
using System.Linq;
using Knot.Localization.Data;
using TMPro;
using UnityEngine;

namespace Knot.Localization.Components
{
    [AddComponentMenu(KnotLocalization.CorePath + "Localized Text Mesh PRO (UI)", 1000)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public partial class KnotLocalizedTextMeshProUGUI : KnotLocalizedComponent<KnotTextKeyReference, string>
    {
        protected TextMeshProUGUI TextMeshPro => _textMeshPro ?? (_textMeshPro = GetComponent<TextMeshProUGUI>());
        private TextMeshProUGUI _textMeshPro;

        
        protected virtual void UpdateFont()
        {
            var fontMetadata = KeyReference.Metadata.OfType<KnotTextMeshProCustomFontMetadata>().LastOrDefault();
            if (fontMetadata?.Font != null && TextMeshPro.font != fontMetadata.Font)
                TextMeshPro.font = fontMetadata.Font;
        }

        protected override void OnValueUpdated(string value)
        {
            if (TextMeshPro == null)
                return;
            
            UpdateFont();
            TextMeshPro.text = KeyReference.Value;
        }
    }
}
#endif
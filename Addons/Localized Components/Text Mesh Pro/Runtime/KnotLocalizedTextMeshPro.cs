#if KNOT_TEXTMESHPRO
using TMPro;
using UnityEngine;

namespace Knot.Localization.Components
{
    [AddComponentMenu(KnotLocalization.CoreName + "/Localized Text Mesh PRO", 1000)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshPro))]
    public partial class KnotLocalizedTextMeshPro : KnotLocalizedComponent<KnotTextKeyReference, string>
    {
        protected TextMeshPro TextMeshPro => _textMeshPro ?? (_textMeshPro = GetComponent<TextMeshPro>());
        private TextMeshPro _textMeshPro;


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
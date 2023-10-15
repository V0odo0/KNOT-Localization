using System.Linq;
using Knot.Localization.Data;
using UnityEngine;

namespace Knot.Localization.Components
{
    [AddComponentMenu(KnotLocalization.CorePath + "Localized Text Mesh", 1000)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMesh))]
    public class KnotLocalizedTextMesh : KnotLocalizedComponent<KnotTextKeyReference, string>
    {
        protected TextMesh TextMesh => _textMesh ?? (_textMesh = GetComponent<TextMesh>());
        private TextMesh _textMesh;


        protected virtual void UpdateFont()
        {
            var fontMetadata = KeyReference.Metadata.OfType<KnotCustomFontMetadata>().LastOrDefault();
            if (fontMetadata?.Font != null && TextMesh.font != fontMetadata.Font)
                TextMesh.font = fontMetadata.Font;
        }

        protected override void OnValueUpdated(string value)
        {
            if (TextMesh == null)
                return;

            UpdateFont();
            TextMesh.text = KeyReference.Value;
        }
    }
}